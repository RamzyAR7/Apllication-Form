using Application_Form.Domain.Entities;

namespace Application_Form.Application.Interfaces.Repositories
{
    public interface IClientRepository
    {
        Task<Client> GetByIdAsync(Guid clientId);
    }
}
