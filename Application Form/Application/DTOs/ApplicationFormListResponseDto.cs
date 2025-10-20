namespace Application_Form.Application.DTOs
{
    public class ApplicationFormListResponseDto
    {
        /// <summary>
        /// Required
        /// Label: Application ID
        /// Description: Unique identifier for the application.
        /// Type: Guid
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Required
        /// Label: Application Name
        /// Description: The display name of the application.
        /// Type: string
        /// </summary>
        public string ApplicationName { get; set; }
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
        /// Required
        /// Label: Client ID
        /// Description: Identifier of the client that owns the application.
        /// Type: Guid
        /// </summary>
        public Guid ClientId { get; set; }
        /// <summary>
        /// Required
        /// Label: Client Name
        /// Description: Display name of the client that owns the application.
        /// Type: string
        /// </summary>
        public string ClientName { get; set; }
    }
}
