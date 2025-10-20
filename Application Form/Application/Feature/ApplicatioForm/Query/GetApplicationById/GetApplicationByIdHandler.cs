using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById
{
    public class GetApplicationByIdHandler : IRequestHandler<GetApplicationByIdQuery, Result<ApplicationFormResponseDto>>
    {
        private readonly IApplicationFormRepository _applicationFormRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetApplicationByIdHandler> _logger;

        public GetApplicationByIdHandler(IApplicationFormRepository applicationFormRepository, IMapper mapper, ILogger<GetApplicationByIdHandler> logger)
        {
            _applicationFormRepository = applicationFormRepository;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<Result<ApplicationFormResponseDto>> Handle(GetApplicationByIdQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving application by id {AppId}", request.Id);
            try
            {
                var application = await _applicationFormRepository.GetByIdAsync(request.Id);

                if (application == null)
                {
                    _logger.LogWarning("Application {AppId} not found", request.Id);
                    return Result<ApplicationFormResponseDto>.Failure("Application not found.");
                }
                if (application.ExpirationDate <= DateOnly.FromDateTime(DateTime.UtcNow) && application.IsActive == true)
                {
                    application.IsActive = false;
                    application.ApprovalStatus = Status.Expired.ToString();
                    _applicationFormRepository.Update(application);
                    _logger.LogInformation("Application {AppId} has expired. Updated status to Expired and set IsActive to false.", request.Id);
                    await _applicationFormRepository.SaveChangesAsync();
                }

                var dto = _mapper.Map<ApplicationFormResponseDto>(application);
                _logger.LogInformation("Returning application {AppId}", request.Id);
                return Result<ApplicationFormResponseDto>.SuccessResult(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving application {AppId}", request.Id);
                return Result<ApplicationFormResponseDto>.Failure($"Error retrieving application: {ex.Message}");
            }
        }
    }
}
