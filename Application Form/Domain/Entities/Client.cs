namespace Application_Form.Domain.Entities
{
    public class Client
    {
        public long Id { get; set; }
        public string Name { get; set; }

        // Navigation property
        public ICollection<ApplicationForm> Applications { get; set; } = new HashSet<ApplicationForm>();
    }
}
