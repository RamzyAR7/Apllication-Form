
namespace Application_Form.Domain.Entities
{
    public class Client
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }

        // Navigation property
        public ICollection<ApplicationForm> Applications { get; set; } = new HashSet<ApplicationForm>();
    }
}
