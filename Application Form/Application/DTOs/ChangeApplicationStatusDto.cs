using Application_Form.Domain.Constant;
using System.ComponentModel.DataAnnotations;

namespace Application_Form.Application.DTOs
{
    public class ChangeApplicationStatusDto
    {
        public string NewStatus { get; set; }
        public string? AdminNotes { get; set; }
        public DateOnly? ExpirationDate { get; set; }
    }
}
