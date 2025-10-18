using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Models;
using Application_Form.Domain.Common;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId
{
    public class GetApplicationsPagedByClientIdHandler : IRequestHandler<GetApplicationsPagedByClientIdQuery, Result<PaginatedList<ApplicationFormResponseDto>>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApplicationsPagedByClientIdHandler> _logger;

        public GetApplicationsPagedByClientIdHandler(IApplicationFormRepository repository, IMapper mapper, ILogger<GetApplicationsPagedByClientIdHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<ApplicationFormResponseDto>>> Handle(GetApplicationsPagedByClientIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving paged applications for client {ClientId}: page {Page}, pageSize {PageSize}, status {Status}", request.ClientId, request.Page, request.PageSize, request.Status);
            try
            {
                var result = await _repository.GetPagedByClientIdAsync(
                    request.ClientId,
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortOrder,
                    request.Status);

                var dtoList = new PaginatedList<ApplicationFormResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ApplicationFormResponseDto>>(result.Items),
                    Page = result.Page,
                    PageSize = result.PageSize,
                    TotalCount = result.TotalCount
                };

                _logger.LogInformation("Returning {Count} applications for client {ClientId} (total {Total})", dtoList.Items.Count(), request.ClientId, dtoList.TotalCount);
                return Result<PaginatedList<ApplicationFormResponseDto>>.SuccessResult(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applications for client {ClientId}", request.ClientId);
                return Result<PaginatedList<ApplicationFormResponseDto>>.Failure($"Error retrieving applications: {ex.Message}");
            }
        }
    }
}
