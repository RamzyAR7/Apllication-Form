using Application_Form.Domain.Constant;
using Application_Form.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Application_Form.Infrastructure.Configurations
{
    public class ApplicationFormConfiguration : IEntityTypeConfiguration<ApplicationForm>
    {
        public void Configure(EntityTypeBuilder<ApplicationForm> builder)
        {
            builder.ToTable("ApplicationForms");
            builder.HasKey(af => af.Id);
            builder.Property(af => af.Id)
                .ValueGeneratedOnAdd()
                .HasColumnType("bigint");

            #region Required Fields
            builder.Property(af => af.ApplicationName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(af => new { af.ApplicationName, af.ClientId})
                .HasDatabaseName("IX_ApplicationForm_Name_ClientId")
                .IsUnique();

            builder.Property(af => af.ApplicationDescription)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(af => af.ApplicationType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(af => af.EmailAddress)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(af => af.Environment)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(af => af.AcceptTerms)
                .IsRequired();

            builder.Property(af => af.ClientId)
                .IsRequired()
                .HasColumnType("bigint");

            // Indexes  
            builder.HasIndex(af => af.ClientId);
            builder.HasIndex(af => af.ApprovalStatus);
            builder.HasIndex(af => af.ExpirationDate);
            builder.HasIndex(af => af.IsActive);
            builder.HasIndex(af => af.IsDeleted);
            builder.HasIndex(af => af.CreatedAt);

            #endregion

            #region Optional Fields
            builder.Property(af => af.OrganizationName).HasMaxLength(150);
            builder.Property(af => af.RedirectUri).HasMaxLength(500);
            builder.Property(af => af.PrivacyPolicyUrl).HasMaxLength(300);
            builder.Property(af => af.DataRetentionDescription).HasMaxLength(500);
            builder.Property(af => af.TechnicalContactName).HasMaxLength(100);
            builder.Property(af => af.TechnicalContactEmail).HasMaxLength(150);
            builder.Property(af => af.AdminNotes).HasMaxLength(500);
            #endregion

            #region System Fields
            builder.Property(af => af.ApprovalStatus)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue(Status.Pending.ToString());

            builder.Property(af => af.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.Property(af => af.IsDeleted)
                .HasDefaultValue(false);
            builder.Property(af => af.IsActive)
                .HasDefaultValue(false);
            #endregion

            // Relationships
            builder.HasOne(af => af.Client)
                .WithMany(c => c.Applications)
                .HasForeignKey(af => af.ClientId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}