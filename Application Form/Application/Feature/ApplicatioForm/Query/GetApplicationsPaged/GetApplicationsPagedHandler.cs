using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Application.Models;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using Application_Form.Infrastructure.Repositories;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using static System.Net.Mime.MediaTypeNames;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged
{
    public class GetApplicationsPagedHandler : IRequestHandler<GetApplicationsPagedQuery, Result<PaginatedList<ApplicationFormListResponseDto>>>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApplicationsPagedHandler> _logger;

        public GetApplicationsPagedHandler(IApplicationFormRepository repository, IMapper mapper, ILogger<GetApplicationsPagedHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result<PaginatedList<ApplicationFormListResponseDto>>> Handle(GetApplicationsPagedQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving paged applications: page {Page}, pageSize {PageSize}, status {Status}", request.Page, request.PageSize, request.Status);
            try
            {
                var result = await _repository.GetPagedApplicationsAsync(
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

                if (!result.Items.Any())
                {
                    _logger.LogInformation("No applications found.");
                    return Result<PaginatedList<ApplicationFormListResponseDto>>.SuccessResult(dtoList, "No applications founds");
                }

                _logger.LogInformation("Returning {Count} applications (total {Total})", dtoList.Items.Count(), dtoList.TotalCount);
                return Result<PaginatedList<ApplicationFormListResponseDto>>.SuccessResult(dtoList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving applications");
                return Result<PaginatedList<ApplicationFormListResponseDto>>.Failure($"Error retrieving applications: {ex.Message}");
            }
        }
    }
}
