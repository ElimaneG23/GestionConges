using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionConges.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.HasIndex(u => u.Email).IsUnique();
        builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(30);

        builder.HasOne(u => u.Manager).WithMany(u => u.Subordinates)
            .HasForeignKey(u => u.ManagerId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.LeaveRequests).WithOne(r => r.User)
            .HasForeignKey(r => r.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.LeaveBalances).WithOne(b => b.User)
            .HasForeignKey(b => b.UserId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(u => u.Notifications).WithOne(n => n.User)
            .HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Restrict);
    }
}
