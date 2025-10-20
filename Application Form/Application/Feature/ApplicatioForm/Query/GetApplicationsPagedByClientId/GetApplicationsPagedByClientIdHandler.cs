using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Models;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId
{
    public class GetApplicationsPagedByClientIdHandler : IRequestHandler<GetApplicationsPagedByClientIdQuery, Result<PaginatedList<ApplicationFormListResponseDto>>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IClientRepository _clientRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApplicationsPagedByClientIdHandler> _logger;

        public GetApplicationsPagedByClientIdHandler(IApplicationFormRepository repository, IClientRepository clientRepository, IMapper mapper, ILogger<GetApplicationsPagedByClientIdHandler> logger)
        {
            _repository = repository;
            _clientRepository = clientRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<ApplicationFormListResponseDto>>> Handle(GetApplicationsPagedByClientIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving paged applications for client {ClientId}: page {Page}, pageSize {PageSize}, status {Status}", request.ClientId, request.Page, request.PageSize, request.Status);
            try
            {
                if (await _clientRepository.GetByIdAsync(request.ClientId) == null)
                {
                    _logger.LogWarning("Client {ClientId} not found", request.ClientId);
                    return Result<PaginatedList<ApplicationFormListResponseDto>>.Failure("Client not found");
                }

                var result = await _repository.GetPagedByClientIdAsync(
                    request.ClientId,
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortOrder,
                    request.Status);



                foreach (var app in result.Items)
                {
                    if (app.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow) && app.IsActive == true)
                    {
                        app.IsActive = false;
                        app.ApprovalStatus = Status.Expired.ToString();
                        _repository.Update(app);
                        _logger.LogInformation("Application {AppId} has expired. Updated status to Expired and set IsActive to false.", app.Id);
                        await _repository.SaveChangesAsync();
                    }
                }

                var dtoList = new PaginatedList<ApplicationFormListResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ApplicationFormListResponseDto>>(result.Items),
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount
                };

                _logger.LogInformation("Returning {Count} applications for client {ClientId} (total {Total})", dtoList.Items.Count(), request.ClientId, dtoList.TotalCount);
                return Result<PaginatedList<ApplicationFormListResponseDto>>.SuccessResult(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applications for client {ClientId}", request.ClientId);
                return Result<PaginatedList<ApplicationFormListResponseDto>>.Failure($"Error retrieving applications: {ex.Message}");
            }
        }
    }
}
