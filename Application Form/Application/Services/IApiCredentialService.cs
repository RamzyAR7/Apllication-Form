using System.Threading.Tasks;

namespace Application_Form.Application.Services
{
    public record ApiCredentials(string ApiKey, string ApiClientId, string ApiClientSecret);

    public interface IApiCredentialService
    {
        Task<ApiCredentials> GenerateAsync(CancellationToken cancellationToken = default);
    }
}
