using GestionConges.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionConges.Infrastructure.Persistence.Configurations;

public class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.ToTable("Tenants");
        builder.HasKey(t => t.Id);
        builder.Property(t => t.Name).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Subdomain).IsRequired().HasMaxLength(100);
        builder.HasIndex(t => t.Subdomain).IsUnique();
        builder.Property(t => t.PrimaryColor).HasMaxLength(20);
        builder.Property(t => t.SecondaryColor).HasMaxLength(20);

        builder.HasMany(t => t.Users).WithOne(u => u.Tenant)
            .HasForeignKey(u => u.TenantId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.LeaveTypes).WithOne(l => l.Tenant)
            .HasForeignKey(l => l.TenantId).OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(t => t.Holidays).WithOne(h => h.Tenant)
            .HasForeignKey(h => h.TenantId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(t => t.Settings).WithOne(s => s!.Tenant)
            .HasForeignKey<TenantSettings>(s => s.TenantId).OnDelete(DeleteBehavior.Cascade);
    }
}
