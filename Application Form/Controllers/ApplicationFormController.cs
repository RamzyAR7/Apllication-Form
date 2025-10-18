using MediatR;
using Microsoft.AspNetCore.Mvc;
using Application_Form.Application.DTOs;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using Application_Form.Domain.Common;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;
using Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication;
using Application_Form.Application.Feature.ApplicatioForm.Command.BulkDeleteApplications;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId;

namespace Application_Form.Controllers
{
    [Route("api/")]
    [ApiController]
    public class ApplicationFormController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ApplicationFormController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// Get a single application form by its id.
        /// </summary>
        /// <param name="id">The application id (GUID).</param>
        /// <returns>Returns a Result object containing the application data.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [HttpGet("application/{id:Guid}")]
        public async Task<IActionResult> GetApplicationFormById(Guid id)
        {
            var result = await _mediator.Send(new GetApplicationByIdQuery(id));

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get paged applications for a specific client.
        /// </summary>
        /// <param name="clientId">Client identifier (GUID).</param>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Page size (default 10).</param>
        /// <param name="sortBy">Field to sort by (default CreatedAt) allowed => [CreatedAt, UpdatedAt, ApplicationName, Status, ExpirationDate].</param>
        /// <param name="sortOrder">Sort order: asc or desc.</param>
        /// <param name="status">Filter by approval status allowed => [Approved, Revoked, Rejected, Pending, All].</param>
        /// <returns>Returns a paginated list of application response DTOs.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("applications/{clientId:Guid}")]
        public async Task<IActionResult> GetApplicationsByClientId(Guid clientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] string sortOrder = "desc", [FromQuery] string status = "All")
        {
            var query = new GetApplicationsPagedByClientIdQuery
            {
                ClientId = clientId,
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Status = status
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result);

            if (result.Data == null || !result.Data.Items.Any())
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get paged applications across all clients.
        /// </summary>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Page size (default 10).</param>
        /// <param name="sortBy">Field to sort by (default CreatedAt) allowed => [CreatedAt, UpdatedAt, ApplicationName, Status, Country, ExpirationDate].</param>
        /// <param name="sortOrder">Sort order: asc or desc.</param>
        /// <param name="status">Filter by approval status allowed => [Approved, Revoked, Rejected, Pending, All].</param>
        /// <returns>Returns a paginated list of application response DTOs.</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpGet("applications")]
        public async Task<IActionResult> GetApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] string sortOrder = "desc", [FromQuery] string status = "All")
        {
            var query = new GetApplicationsPagedQuery
            {
                Page = page,
                PageSize = pageSize,
                SortBy = sortBy,
                SortOrder = sortOrder,
                Status = status
            };

            var result = await _mediator.Send(query);

            if (!result.Success)
                return BadRequest(result);

            if (result.Data == null || !result.Data.Items.Any())
                return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Update an existing application.
        /// </summary>
        /// <param name="id">The application id (GUID).</param>
        /// <param name="dto">Update DTO containing editable fields.</param>
        /// <returns>Returns 204 No Content on success.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPut("application/{id:Guid}")]
        public async Task<IActionResult> UpdateApplicationForm(Guid id, [FromBody] UpdateApplicationFormDto dto)
        {
            var command = new UpdateApplicationFormCommand(id, dto);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            // Return 204 No Content on successful update
            return NoContent();
        }

        /// <summary>
        /// Create a new application.
        /// </summary>
        /// <param name="dto">Create DTO containing application details.</param>
        /// <returns>Returns 201 Created with a Result object.</returns>
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPost("application")]
        public async Task<IActionResult> CreateApplicationForm([FromBody] CreateApplicationFormDto dto)
        {
            var command = new CreateApplicationFormCommand(dto);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return CreatedAtAction(nameof(GetApplicationFormById), null, result);
        }

        /// <summary>
        /// Change the approval status of an application (approve, reject, revoke).
        /// </summary>
        /// <param name="id">Application id (GUID).</param>
        /// <param name="dto">Change status DTO (NewStatus, AdminNotes, ExpirationDate).</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("application/{id:Guid}/status")]
        public async Task<IActionResult> ChangeApplicationStatus(Guid id, [FromBody] ChangeApplicationStatusDto dto)
        {
            var command = new ChangeApplicationStatusCommand(id, dto);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Renew API credentials for an application.
        /// </summary>
        /// <param name="id">Application id (GUID)</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("application/{id:Guid}/renew")]
        public async Task<IActionResult> RenewApplicationCredentials(Guid id)
        {
            var command = new RenewApplicationCredentialsCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Renew the expiration date for an application.
        /// </summary>
        /// <param name="id">Application id (GUID)</param>
        /// <param name="dto">DTO containing the new expiration date.</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("application/{id:Guid}/renew-expiration")]
        public async Task<IActionResult> RenewApplicationExpirationDate(Guid id, [FromBody] RenewApplicationExpirationDateDto dto)
        {
            var command = new RenewApplicationExpirationDateCommand(id, dto.NewExpirationDate);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Renew or update the expiration date for an application.
        /// </summary>
        /// <param name="id">Application id (GUID)</param>
        /// <param name="dto">DTO containing new expiration date</param>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpPatch("application/{id:Guid}/expiration")]
        public async Task<IActionResult> RenewApplicationExpiration(Guid id, [FromBody] Application_Form.Application.DTOs.RenewApplicationExpirationDateDto dto)
        {
            if (dto == null)
                return BadRequest(Result.Failure("Request body is required."));

            var command = new RenewApplicationExpirationDateCommand(id, dto.NewExpirationDate);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Soft delete an application (mark IsDeleted=true and IsActive=false).
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("application/{id:Guid}")]
        public async Task<IActionResult> DeleteApplication(Guid id)
        {
            var command = new DeleteApplicationCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Bulk soft delete applications (mark IsDeleted=true and IsActive=false) by ids.
        /// </summary>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [HttpDelete("applications/bulk-delete")]
        public async Task<IActionResult> BulkDeleteApplications([FromBody] IEnumerable<Guid> ids)
        {
            var command = new BulkDeleteApplicationsCommand(ids);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
