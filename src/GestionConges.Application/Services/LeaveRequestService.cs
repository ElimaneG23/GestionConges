using GestionConges.Application.Common;
using GestionConges.Application.DTOs.LeaveRequest;
using GestionConges.Application.Interfaces;
using GestionConges.Domain.Entities;
using GestionConges.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace GestionConges.Application.Services;

/// <summary>
/// Cœur métier : cycle de vie d'une demande de congé.
/// Sprint 4 (créer/consulter/annuler) + Sprint 5 (validation manager) + Sprint 7 (notifications).
/// </summary>
public class LeaveRequestService : ILeaveRequestService
{
    private readonly IAppDbContext _db;
    private readonly INotificationService _notificationService;

    public LeaveRequestService(IAppDbContext db, INotificationService notificationService)
    {
        _db = db;
        _notificationService = notificationService;
    }

    public async Task<Result<LeaveRequestDto>> CreateAsync(Guid userId, Guid tenantId, CreateLeaveRequestDto dto)
    {
        if (dto.EndDate < dto.StartDate)
            return Result<LeaveRequestDto>.Fail("La date de fin doit être postérieure à la date de début.");

        var leaveType = await _db.LeaveTypes.FirstOrDefaultAsync(l => l.Id == dto.LeaveTypeId && l.TenantId == tenantId);
        if (leaveType is null || !leaveType.IsActive)
            return Result<LeaveRequestDto>.Fail("Type de congé invalide.");

        if (leaveType.RequiresJustification && string.IsNullOrWhiteSpace(dto.Reason))
            return Result<LeaveRequestDto>.Fail("Ce type de congé nécessite un motif.");

        var numberOfDays = ComputeBusinessDays(dto.StartDate, dto.EndDate);

        // Vérifie le solde disponible (créé à la volée s'il n'existe pas encore pour l'année en cours)
        var year = dto.StartDate.Year;
        var balance = await _db.LeaveBalances.FirstOrDefaultAsync(b =>
            b.UserId == userId && b.LeaveTypeId == dto.LeaveTypeId && b.Year == year);

        if (balance is null)
        {
            balance = new LeaveBalance
            {
                TenantId = tenantId,
                UserId = userId,
                LeaveTypeId = dto.LeaveTypeId,
                Year = year,
                TotalDays = leaveType.DefaultDaysPerYear,
                UsedDays = 0
            };
            await _db.LeaveBalances.AddAsync(balance);
        }

        if (balance.RemainingDays < numberOfDays)
            return Result<LeaveRequestDto>.Fail(
                $"Solde insuffisant : {balance.RemainingDays} jour(s) restant(s) pour {numberOfDays} jour(s) demandé(s).");

        var request = new LeaveRequest
        {
            TenantId = tenantId,
            UserId = userId,
            LeaveTypeId = dto.LeaveTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            NumberOfDays = numberOfDays,
            Reason = dto.Reason,
            Status = LeaveRequestStatus.Pending
        };
        await _db.LeaveRequests.AddAsync(request);
        await _db.SaveChangesAsync();

        // Notifie le manager (Sprint 7 - US7.1)
        var employee = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (employee?.ManagerId is not null)
        {
            await _notificationService.NotifyAsync(
                tenantId, employee.ManagerId.Value,
                "Nouvelle demande de congé",
                $"{employee.FullName} a soumis une demande du {dto.StartDate:dd/MM/yyyy} au {dto.EndDate:dd/MM/yyyy}.",
                NotificationType.LeaveRequestCreated, request.Id);
        }

        return Result<LeaveRequestDto>.Ok(await ToDtoAsync(request));
    }

    public async Task<Result<List<LeaveRequestDto>>> GetMyRequestsAsync(Guid userId)
    {
        var requests = await _db.LeaveRequests
            .Include(r => r.User).Include(r => r.LeaveType).Include(r => r.ProcessedByUser)
            .Where(r => r.UserId == userId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Result<List<LeaveRequestDto>>.Ok(requests.Select(MapDto).ToList());
    }

    public async Task<Result<bool>> CancelAsync(Guid requestId, Guid userId)
    {
        var request = await _db.LeaveRequests.FirstOrDefaultAsync(r => r.Id == requestId && r.UserId == userId);
        if (request is null) return Result<bool>.Fail("Demande introuvable.");

        if (request.Status != LeaveRequestStatus.Pending)
            return Result<bool>.Fail("Seule une demande en attente peut être annulée.");

        request.Status = LeaveRequestStatus.Cancelled;
        request.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync();
        return Result<bool>.Ok(true);
    }

    public async Task<Result<List<LeaveRequestDto>>> GetPendingForManagerAsync(Guid managerId)
    {
        var requests = await _db.LeaveRequests
            .Include(r => r.User).Include(r => r.LeaveType).Include(r => r.ProcessedByUser)
            .Where(r => r.Status == LeaveRequestStatus.Pending && r.User!.ManagerId == managerId)
            .OrderBy(r => r.StartDate)
            .ToListAsync();

        return Result<List<LeaveRequestDto>>.Ok(requests.Select(MapDto).ToList());
    }

    public async Task<Result<LeaveRequestDto>> ProcessAsync(Guid requestId, Guid managerId, ProcessLeaveRequestDto dto)
    {
        var request = await _db.LeaveRequests
            .Include(r => r.User).Include(r => r.LeaveType)
            .FirstOrDefaultAsync(r => r.Id == requestId);

        if (request is null) return Result<LeaveRequestDto>.Fail("Demande introuvable.");
        if (request.User?.ManagerId != managerId)
            return Result<LeaveRequestDto>.Fail("Vous n'êtes pas autorisé à traiter cette demande.");
        if (request.Status != LeaveRequestStatus.Pending)
            return Result<LeaveRequestDto>.Fail("Cette demande a déjà été traitée.");

        request.Status = dto.Approve ? LeaveRequestStatus.Approved : LeaveRequestStatus.Rejected;
        request.ManagerComment = dto.ManagerComment;
        request.ProcessedByUserId = managerId;
        request.ProcessedAt = DateTime.UtcNow;
        request.UpdatedAt = DateTime.UtcNow;

        // Si approuvée, on décrémente le solde (Sprint 5 - US5.2)
        if (dto.Approve)
        {
            var balance = await _db.LeaveBalances.FirstOrDefaultAsync(b =>
                b.UserId == request.UserId && b.LeaveTypeId == request.LeaveTypeId && b.Year == request.StartDate.Year);
            if (balance is not null)
                balance.UsedDays += request.NumberOfDays;
        }

        await _db.SaveChangesAsync();

        // Notifie l'employé (Sprint 7 - US7.2 / US7.3)
        await _notificationService.NotifyAsync(
            request.TenantId!.Value, request.UserId,
            dto.Approve ? "Demande approuvée" : "Demande refusée",
            dto.Approve
                ? $"Votre demande du {request.StartDate:dd/MM/yyyy} au {request.EndDate:dd/MM/yyyy} a été approuvée."
                : $"Votre demande du {request.StartDate:dd/MM/yyyy} au {request.EndDate:dd/MM/yyyy} a été refusée." +
                  (string.IsNullOrWhiteSpace(dto.ManagerComment) ? "" : $" Motif : {dto.ManagerComment}"),
            dto.Approve ? NotificationType.LeaveRequestApproved : NotificationType.LeaveRequestRejected,
            request.Id);

        return Result<LeaveRequestDto>.Ok(await ToDtoAsync(request));
    }

    /// <summary>Nombre de jours ouvrés (lundi-vendredi) entre deux dates, bornes incluses.</summary>
    private static decimal ComputeBusinessDays(DateTime start, DateTime end)
    {
        var days = 0;
        for (var d = start.Date; d <= end.Date; d = d.AddDays(1))
        {
            if (d.DayOfWeek != DayOfWeek.Saturday && d.DayOfWeek != DayOfWeek.Sunday)
                days++;
        }
        return days;
    }

    private async Task<LeaveRequestDto> ToDtoAsync(LeaveRequest r)
    {
        r.User ??= await _db.Users.FirstOrDefaultAsync(u => u.Id == r.UserId);
        r.LeaveType ??= await _db.LeaveTypes.FirstOrDefaultAsync(l => l.Id == r.LeaveTypeId);
        if (r.ProcessedByUserId is not null)
            r.ProcessedByUser ??= await _db.Users.FirstOrDefaultAsync(u => u.Id == r.ProcessedByUserId);
        return MapDto(r);
    }

    private static LeaveRequestDto MapDto(LeaveRequest r) => new(
        r.Id, r.UserId, r.User?.FullName ?? "", r.LeaveTypeId, r.LeaveType?.Name ?? "",
        r.StartDate, r.EndDate, r.NumberOfDays, r.Reason, r.Status.ToString(),
        r.ManagerComment, r.ProcessedByUser?.FullName, r.ProcessedAt, r.CreatedAt);
}
