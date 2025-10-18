using System.Data;
using System;
using Application_Form.Domain.Constant;

namespace Application_Form.Domain.Entities
{
    public class ApplicationForm
    {
        // Properties
        public Guid Id { get; set; } = Guid.NewGuid();
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public string EmailAddress { get; set; }
        public string OrganizationName { get; set; }
        public int? CountryId { get; set; }
        public string ApplicationType { get; set; }
        public string? RedirectUri { get; set; }
        public string Environment { get; set; }
        public int? ExpectedRequestVolume { get; set; }
        public bool AcceptTerms { get; set; }
        public string? PrivacyPolicyUrl { get; set; }
        public string? DataRetentionDescription { get; set; }
        public string? TechnicalContactName { get; set; }
        public string? TechnicalContactEmail { get; set; }
        public string ApprovalStatus { get; set; } = Status.Pending.ToString();
        public string? ApiKey { get; set; }
        public string? ApiClientId { get; set; }
        public string? ApiClientSecret { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastModified { get; set; }
        public DateOnly? ExpirationDate { get; set; }
        public Guid ClientId { get; set; }
        public string? AdminNotes { get; set; }

        // Navigation property
        public Client? Client { get; set; }
    }
}
