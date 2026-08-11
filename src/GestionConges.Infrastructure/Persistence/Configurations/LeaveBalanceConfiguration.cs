using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionConges.Infrastructure.Persistence.Configurations;

public class LeaveBalanceConfiguration : IEntityTypeConfiguration<LeaveBalance>
{
    public void Configure(EntityTypeBuilder<LeaveBalance> builder)
    {
        builder.ToTable("LeaveBalances");
        builder.HasKey(b => b.Id);
        builder.Property(b => b.TotalDays).HasColumnType("decimal(5,1)");
        builder.Property(b => b.UsedDays).HasColumnType("decimal(5,1)");
        builder.Ignore(b => b.RemainingDays);
        builder.HasIndex(b => new { b.UserId, b.LeaveTypeId, b.Year }).IsUnique();
    }
}
