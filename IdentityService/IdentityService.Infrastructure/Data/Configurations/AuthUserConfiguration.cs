using IdentityService.Domain.Entities;
using IdentityService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityService.Infrastructure.Data.Configurations;

public class AuthUserConfiguration : IEntityTypeConfiguration<AuthUser>
{
    public void Configure(EntityTypeBuilder<AuthUser> builder)
    {
        builder.ToTable("AuthUsers", "dbo");
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Id)
            .HasDefaultValueSql("NEWSEQUENTIALID()");

        builder.Property(u => u.Ativo)
            .HasDefaultValue(TipoUserAtivo.Ativo)
            .HasSentinel((TipoUserAtivo)(-1))
            .IsRequired();

        builder.Property(u => u.TenancyAtiva)
            .HasDefaultValue(TipoTenancyAtivo.Ativo)
            .HasSentinel((TipoTenancyAtivo)(-1))
            .IsRequired();

        builder.Property(u => u.UserName)
            .HasMaxLength(100)
            .IsRequired();
        builder.HasIndex(u => u.UserName).IsUnique();

        builder.Property(m => m.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(u => u.PrimeiroAcesso);

        builder.Property(u => u.PasswordHash)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(u => u.Role)
            .IsRequired();

        builder.Property(u => u.DataInclusao)
            .IsRequired();

        builder.Property(u => u.DataAlteracao);

        builder.Property(m => m.TenancyId)
            .HasColumnType("bigint")
            .IsRequired();

        builder.HasOne(m => m.Tenancy)
            .WithMany(e => e.AuthUsers)
            .HasForeignKey(m => m.TenancyId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
