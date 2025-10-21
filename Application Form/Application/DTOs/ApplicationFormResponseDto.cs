using System.Text.Json.Serialization;
using Application_Form.Domain.Constant;

namespace Application_Form.Application.DTOs
{
    public class ApplicationFormResponseDto
    {
        /// <summary>
        /// Required
        /// Label: Application ID
        /// Description: Unique identifier for the application.
        /// Type: long
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Required
        /// Label: Application Name
        /// Description: The display name of the application.
        /// Type: string
        /// </summary>
        public string ApplicationName { get; set; }
        /// <summary>
        /// Required
        /// Label: Application Description
        /// Description: Short description of the application's purpose or functionality.
        /// Type: string
        /// </summary>
        public string ApplicationDescription { get; set; }
        /// <summary>
        /// Required
        /// Label: Contact Email Address
        /// Description: Email for notifications and support.
        /// Type: string
        /// </summary>
        public string EmailAddress { get; set; }
        /// <summary>
        /// Required
        /// Label: Organization Name
        /// Description: Name of the organization or individual that owns the application.
        /// Type: string
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
        /// </summary>
        public string ApplicationType { get; set; }
        /// <summary>
        /// Optional
        /// Label: Redirect URI
        /// Description: Redirect URI for OAuth or callback flows.
        /// Type: string?
        /// </summary>
        public string? RedirectUri { get; set; }
        /// <summary>
        /// Required
        /// Label: Environment
        /// Description: Deployment environment (Sandbox, Production, Both).
        /// Type: string
        /// </summary>
        public string Environment { get; set; }
        /// <summary>
        /// Optional
        /// Label: Expected Request Volume
        /// Description: Approximate expected request volume (for capacity planning).
        /// Type: int?
        /// </summary>
        public int? ExpectedRequestVolume { get; set; }
        /// <summary>
        /// Optional
        /// Label: Privacy Policy URL
        /// Description: URL to the application's privacy policy.
        /// Type: string?
        /// </summary>
        public string? PrivacyPolicyUrl { get; set; }
        /// <summary>
        /// Optional
        /// Label: Data Retention Description
        /// Description: Description of the application's data retention policy.
        /// Type: string?
        /// </summary>
        public string? DataRetentionDescription { get; set; }
        /// <summary>
        /// Optional
        /// Label: Technical Contact Name
        /// Description: Name of the technical contact for the application.
        /// Type: string?
        /// </summary>
        public string? TechnicalContactName { get; set; }
        /// <summary>
        /// Optional
        /// Label: Technical Contact Email
        /// Description: Email address of the technical contact.
        /// Type: string?
        /// </summary>
        public string? TechnicalContactEmail { get; set; }

        // System fields
        /// <summary>
        /// Required
        /// Label: Approval Status
        /// Description: Current approval status (e.g., Approved, Pending, Rejected, Revoked).
        /// Type: string
        /// </summary>
        public string ApprovalStatus { get; set; }
        /// <summary>
        /// Required
        /// Label: Is Active
        /// Description: Indicates whether the application is active.
        /// Type: bool
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// Required
        /// Label: Created At
        /// Description: The UTC date and time when the application was created.
        /// Type: DateTime
        /// </summary>
        public DateTime CreatedAt { get; set; }
        /// <summary>
        /// Required
        /// Label: Last Modified
        /// Description: The UTC date and time when the application was last modified.
        /// Type: DateTime
        /// </summary>
        public DateTime LastModified { get; set; }
        /// <summary>
        /// Required
        /// Label: Expiration Date
        /// Description: Expiration date for the application's credentials or access.
        /// Type: DateOnly
        /// </summary>
        public DateOnly ExpirationDate { get; set; }
        /// <summary>
        /// Optional
        /// Label: API Key
        /// Description: API key issued to the application (if any).
        /// Type: string?
        /// </summary>
        public string? ApiKey { get; set; }
        /// <summary>
        /// Optional
        /// Label: API Client ID
        /// Description: API client identifier string associated with the application.
        /// Type: string?
        /// </summary>
        public string? ApiClientId { get; set; }
        /// <summary>
        /// Optional
        /// Label: API Client Secret
        /// Description: API client secret associated with the application (sensitive).
        /// Type: string?
        /// </summary>
        public string? ApiClientSecret { get; set; }
        /// <summary>
        /// Optional
        /// Label: Admin Notes
        /// Description: Administrative notes related to the application's approval or management.
        /// Type: string?
        /// </summary>
        public string? AdminNotes { get; set; }
        /// <summary>
        /// Required
        /// Label: Client ID
        /// Description: Identifier of the client that owns the application.
        /// Type: long
        /// </summary>
        public long ClientId { get; set; }
        /// <summary>
        /// Required
        /// Label: Client Name
        /// Description: Display name of the client that owns the application.
        /// Type: string
        /// </summary>
        public string ClientName { get; set; }
    }
}
