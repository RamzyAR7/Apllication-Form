namespace Application_Form.Application.DTOs
{
    public class RenewApplicationExpirationDateDto
    {
        /// <summary>
        /// Required
        /// Label: New Expiration Date
        /// Description: The new expiration date to apply to the application credentials/access.
        /// Type: DateOnly
        /// </summary>
        public DateOnly NewExpirationDate { get; set; } 
    }
}
