using System;

namespace Application_Form.Application.DTOs
{
    public class UpdateApplicationFormDto
    {
        public string ApplicationName { get; set; }
        public string ApplicationDescription { get; set; }
        public string EmailAddress { get; set; }
        public string OrganizationName { get; set; }
        public int? CountryId { get; set; }
        public string ApplicationType { get; set; }
        public string? RedirectUri { get; set; }
        public string Environment { get; set; }
        public int? ExpectedRequestVolume { get; set; }
        public string? PrivacyPolicyUrl { get; set; }
        public string? DataRetentionDescription { get; set; }
        public string? TechnicalContactName { get; set; }
        public string? TechnicalContactEmail { get; set; }
    }
}
