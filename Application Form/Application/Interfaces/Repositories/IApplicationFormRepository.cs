using Application_Form.Application.Models;
using Application_Form.Domain.Entities;

namespace Application_Form.Application.Interfaces.Repositories
{
    public interface IApplicationFormRepository
    {
        // Queries
        Task<ApplicationForm?> GetByIdAsync(Guid id);
        Task<PaginatedList<ApplicationForm>> GetPagedApplicationsAsync(
            int page,
            int pageSize,
            string sortBy,
            string sortOrder,
            string status);
        Task<PaginatedList<ApplicationForm>> GetPagedByClientIdAsync(Guid ClientId,
            int page,
            int pageSize,
            string sortBy,
            string sortOrder,
            string status);

        // Commands
        Task AddAsync(ApplicationForm form);
        void Update(ApplicationForm form);
        Task<int> SaveChangesAsync();
    }
}
