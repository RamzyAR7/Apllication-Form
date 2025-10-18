using Application_Form.Application.DTOs;
using Application_Form.Application.Interfaces.Repositories;
using Application_Form.Domain.Common;
using Application_Form.Domain.Constant;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm
{
    public class UpdateApplicationFormHandler : IRequestHandler<UpdateApplicationFormCommand, Result>
    {
        private readonly IApplicationFormRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateApplicationFormHandler> _logger;

        public UpdateApplicationFormHandler(IApplicationFormRepository repository, IMapper mapper, ILogger<UpdateApplicationFormHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateApplicationFormCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating application {AppId}", request.Id);
            try
            {
                var entity = await _repository.GetByIdAsync(request.Id);
                if (entity == null || entity.IsDeleted)
                {
                    _logger.LogWarning("Application {AppId} not found or deleted", request.Id);
                    return Result.Failure("Application not found.");
                }

                if (entity.ApprovalStatus != Status.Pending.ToString())
                {
                    _logger.LogWarning("Attempt to update application {AppId} with status {Status}", request.Id, entity.ApprovalStatus);
                    return Result.Failure("Only Pending applications can be updated.");
                }

                // Map update DTO onto existing entity; mapping is configured to ignore ClientId and system fields
                _mapper.Map(request.Dto, entity);

                entity.LastModified = DateTime.UtcNow;

                _repository.Update(entity);
                await _repository.SaveChangesAsync();

                _logger.LogInformation("Application {AppId} updated successfully", request.Id);
                // Do not return the updated entity in Data; return empty message as requested.
                return Result.SuccessResult(string.Empty);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating application {AppId}", request.Id);
                return Result.Failure($"Error updating application: {ex.Message}");
            }
        }
    }
}
