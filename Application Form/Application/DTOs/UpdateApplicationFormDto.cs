using System;
using System.ComponentModel.DataAnnotations;

namespace Application_Form.Application.DTOs
{
    public class UpdateApplicationFormDto
    {
        /// <summary>
        /// Required
        /// Label: Application Name
        /// Description: The display name of the application.
        /// Type: string
        /// Max Length: 100
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Required
        /// Label: Application Description
        /// Description: Short description of the application's purpose or functionality.
        /// Type: string
        /// Max Length: 1000
        /// </summary>
        public string ApplicationDescription { get; set; }
        /// <summary>
        /// Required
        /// Label: Contact Email Address
        /// Description: Email for notifications and support.
        /// Type: string
        /// Max Length: 255
        /// Validation: Must be a valid email
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Required
        /// Label: Organization Name
        /// Description: Name of the organization or individual that owns the application.
        /// Type: string
        /// Max Length: 150
        /// </summary>
        public string OrganizationName { get; set; }
        /// <summary>
        /// Optional
        /// Label: Country
        /// Description: Country identifier associated with the application.
        /// Type: int?
        /// </summary>
        public int? CountryId { get; set; }
        /// <summary>
        /// Required
        /// Label: Application Type
        /// Description: Type or category (e.g., Web, Mobile, Service).
        /// Type: string
        /// Max Length: 50
        /// </summary>
        public string ApplicationType { get; set; }
        /// <summary>
        /// Optional
        /// Label: Redirect URI
        /// Description: Redirect URI for OAuth or callback flows.
        /// Type: string?
        /// Max Length: 500
        /// Validation: Must be a valid URL if provided
        /// </summary>
        public string? RedirectUri { get; set; }
        /// <summary>
        /// Required
        /// Label: Environment
        /// Description: Deployment environment (Sandbox, Production, Both).
        /// Type: string
        /// Max Length: 50
        /// Validation: Must be one of: Sandbox, Production, Both
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// Optional
        /// Label: Expected Request Volume
        /// Description: Approximate expected request volume (for capacity planning).
        /// Type: int?
        /// Validation: Must be non-negative if provided
        /// </summary>
        public int? ExpectedRequestVolume { get; set; }
        /// <summary>
        /// Optional
        /// Label: Privacy Policy URL
        /// Description: URL to the application's privacy policy.
        /// Type: string?
        /// Max Length: 300
        /// Validation: Must be a valid URL if provided
        /// </summary>
        public string? PrivacyPolicyUrl { get; set; }
        /// <summary>
        /// Optional
        /// Label: Data Retention Description
        /// Description: Description of the application's data retention policy.
        /// Type: string?
        /// Max Length: 500
        /// </summary>
        public string? DataRetentionDescription { get; set; }
        /// <summary>
        /// Optional
        /// Label: Technical Contact Name
        /// Description: Name of the technical contact for the application.
        /// Type: string?
        /// Max Length: 100
        /// </summary>
        public string? TechnicalContactName { get; set; }
        /// <summary>
        /// Optional
        /// Label: Technical Contact Email
        /// Description: Email address of the technical contact.
        /// Type: string?
        /// Max Length: 150
        /// Validation: Must be a valid email if provided
        /// </summary>
        public string? TechnicalContactEmail { get; set; }
        /// <summary>
        /// Required
        /// Label: Client ID
        /// Description: The client identifier (long) associated with this application.
        /// Type: long
        /// </summary>
        public long ClientId { get; set; }
    }
}
