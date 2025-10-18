using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Application_Form.Application.Models;

namespace Application_Form.Infrastructure.Repositories
{
    public class ApplicationFormRepository : IApplicationFormRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationFormRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Query Methods
        public async Task<ApplicationForm?> GetByIdAsync(Guid id)
        {
            return await _context.ApplicationForms
                .Include(a => a.Client)
                .FirstOrDefaultAsync(a => a.Id == id && !a.IsDeleted);
        }

        public async Task<PaginatedList<ApplicationForm>> GetPagedApplicationsAsync(int page, int pageSize, string sortBy, string sortOrder, string status)
        {
            var query = _context.ApplicationForms
               .Include(a => a.Client)
               .Where(a => !a.IsDeleted)
               .AsQueryable();

            if(!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                query = query.Where(a => a.ApprovalStatus.ToLower() == status.ToLower());

            var totalCount = query.Count();

            query = query.OrderBy($"{sortBy} {sortOrder}");

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<ApplicationForm>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<PaginatedList<ApplicationForm>> GetPagedByClientIdAsync(Guid ClientId, int page, int pageSize, string sortBy, string sortOrder, string status)
        {
            var query = _context.ApplicationForms
              .Include(a => a.Client)
              .Where(a => a.ClientId == ClientId && !a.IsDeleted)
              .AsQueryable();
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
                query = query.Where(a => a.ApprovalStatus.ToLower() == status.ToLower());

            var totalCount = query.Count();

            query = query.OrderBy($"{sortBy} {sortOrder}");

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedList<ApplicationForm>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

        }
        #endregion
        #region Command Methods
        public async Task AddAsync(ApplicationForm form)
        {
            await _context.ApplicationForms.AddAsync(form);
        }
        public void Update(ApplicationForm form)
        {
            _context.ApplicationForms.Update(form);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
        #endregion
    }
}
