using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionConges.Infrastructure.Persistence.Configurations;

public class LeaveTypeConfiguration : IEntityTypeConfiguration<LeaveType>
{
    public void Configure(EntityTypeBuilder<LeaveType> builder)
    {
        builder.ToTable("LeaveTypes");
        builder.HasKey(l => l.Id);
        builder.Property(l => l.Name).IsRequired().HasMaxLength(150);

        builder.HasMany(l => l.LeaveRequests).WithOne(r => r.LeaveType)
            .HasForeignKey(r => r.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(l => l.LeaveBalances).WithOne(b => b.LeaveType)
            .HasForeignKey(b => b.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
    }
}
