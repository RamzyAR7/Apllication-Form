using Application_Form.Domain.Constant;
using System.ComponentModel.DataAnnotations;

namespace Application_Form.Application.DTOs
{
    public class ChangeApplicationStatusDto
    {
        /// <summary>
        /// Required
        /// Label: New Status
        /// Description: The new approval status to set for the application (e.g., Approved, Rejected, Revoked).
        /// Type: string
        /// </summary>
        public string NewStatus { get; set; }
        /// <summary>
        /// Optional
        /// Label: Admin Notes
        /// Description: Administrative notes explaining the status change.
        /// Type: string?
        /// </summary>
        public string? AdminNotes { get; set; }
        /// <summary>
        /// Optional
        /// Label: Expiration Date
        /// Description: New expiration date to set when changing status.
        /// Type: DateOnly?
        /// </summary>
        public DateOnly? ExpirationDate { get; set; }
    }
}
