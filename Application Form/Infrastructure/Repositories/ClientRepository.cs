using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Entities;
using Application_Form.Infrastructure.Data;

namespace Application_Form.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task<Client> GetByIdAsync(Guid clientId)
        {
            var client =  await _context.Clients.FindAsync(clientId);
            return client;
        }
    }
}
