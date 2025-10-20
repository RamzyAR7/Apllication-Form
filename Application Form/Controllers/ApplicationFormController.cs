using Application_Form.Application.DTOs;
using Application_Form.Application.Feature.ApplicatioForm.Command.ChangeApplicationStatus;
using Application_Form.Application.Feature.ApplicatioForm.Command.CreateApplicationForm;
using Application_Form.Application.Feature.ApplicatioForm.Command.DeleteApplication;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationCredentials;
using Application_Form.Application.Feature.ApplicatioForm.Command.RenewApplicationExpirationDate;
using Application_Form.Application.Feature.ApplicatioForm.Command.UpdateApplicationForm;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationById;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPaged;
using Application_Form.Application.Feature.ApplicatioForm.Query.GetApplicationsPagedByClientId;
using Application_Form.Application.Models;
using Application_Form.Domain.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using Application_Form.SwaggerDocs;

namespace Application_Form.Controllers
{
    [Route("api/")]
    [ApiController]
    [Produces("application/json")]
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
        /// <response code="200">Application found and returned successfully. Response contains the application data.</response>
        /// <response code="404">Application not found for the provided id.</response>
        [ProducesResponseType(typeof(Result<ApplicationFormResponseDto>), StatusCodes.Status200OK)]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status404NotFound)]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(BadRequestExample))]
        [HttpGet("application/{id:Guid}")]
        public async Task<ActionResult<Result<ApplicationFormResponseDto>>> GetApplicationFormById(Guid id)
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
        /// <param name="sortBy">Field to sort by (default CreatedAt) allowed =&gt; [CreatedAt, LastModified, ApplicationName, ApprovalStatus, ExpirationDate].</param>
        /// <param name="sortOrder">Sort order: asc or desc.</param>
        /// <param name="status">Filter by approval status allowed =&gt; [Approved, Revoked, Rejected, Pending, Expired, All].</param>
        /// <returns>Returns a paginated list of application response DTOs.</returns>
        /// <response code="200">Paged applications returned successfully.</response>
        /// <response code="400">Invalid query parameters were provided.</response>
        [ProducesResponseType(typeof(Result<PaginatedList<ApplicationFormListResponseDto>>), StatusCodes.Status200OK)]
        //===========================================
        //[ProducesResponseType(typeof(Result<PaginatedList<CustomEmptyResult>>), StatusCodes.Status404NotFound)]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpGet("applications/{clientId:Guid}")]
        public async Task<ActionResult<Result<PaginatedList<ApplicationFormListResponseDto>>>> GetApplicationsByClientId(Guid clientId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] string sortOrder = "desc", [FromQuery] string status = "All")
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

            //if (result.Data.TotalCount == 0)
            //    return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Get paged applications across all clients.
        /// </summary>
        /// <param name="page">Page number (default 1).</param>
        /// <param name="pageSize">Page size (default 10).</param>
        /// <param name="sortBy">Field to sort by (default CreatedAt) allowed =&gt; [CreatedAt, LastModified, ApplicationName, ApprovalStatus, ExpirationDate].</param>
        /// <param name="sortOrder">Sort order: asc or desc.</param>
        /// <param name="status">Filter by approval status allowed =&gt; [Approved, Revoked, Rejected, Pending, Expired, All].</param>
        /// <returns>Returns a paginated list of application response DTOs.</returns>
        /// <response code="200">Paged applications returned successfully.</response>
        /// <response code="400">Invalid query parameters were provided.</response>
        [ProducesResponseType(typeof(Result<PaginatedList<ApplicationFormListResponseDto>>), StatusCodes.Status200OK)]
        //===========================================
        //[ProducesResponseType(typeof(Result<PaginatedList<CustomEmptyResult>>), StatusCodes.Status404NotFound)]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpGet("applications")]
        public async Task<ActionResult<Result<PaginatedList<ApplicationFormListResponseDto>>>> GetApplications([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string sortBy = "CreatedAt", [FromQuery] string sortOrder = "desc", [FromQuery] string status = "All")
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

            //if (result.Data.TotalCount == 0)
            //    return NotFound(result);

            return Ok(result);
        }

        /// <summary>
        /// Update an existing application.
        /// </summary>
        /// <param name="id">The application id (GUID).</param>
        /// <param name="dto">Update DTO containing editable fields.</param>
        /// <returns>Returns 200 Ok on success.</returns>
        /// <response code="200">Application updated successfully. No content is returned.</response>
        /// <response code="400">Invalid update request or validation failed.</response>
        [ProducesResponseType(typeof(Result<ApplicationFormListResponseDto>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpPut("application/{id:Guid}")]
        public async Task<ActionResult<Result<ApplicationFormListResponseDto>>> UpdateApplicationForm(Guid id, [FromBody] UpdateApplicationFormDto dto)
        {
            var command = new UpdateApplicationFormCommand(id, dto);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Create a new application.
        /// </summary>
        /// <param name="dto">Create DTO containing application details.</param>
        /// <returns>Returns 201 Created with a Result object.</returns>
        /// <response code="201">Application created successfully. Returns the created resource information.</response>
        /// <response code="400">Invalid create request or validation failed.</response>
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status201Created)]
        [SwaggerResponseExample(StatusCodes.Status201Created, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]

        [HttpPost("application")]
        public async Task<ActionResult< Result<ApplicationFormListResponseDto>>> CreateApplicationForm([FromBody] CreateApplicationFormDto dto)
        {
            var command = new CreateApplicationFormCommand(dto);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return StatusCode(201, result);
        }

        /// <summary>
        /// Change the approval status of an application ( Approved, Rejected, Revoked).
        /// </summary>
        /// <param name="id">Application id (GUID).</param>
        /// <param name="dto">Change status DTO (NewStatus, AdminNotes, ExpirationDate).</param>
        /// <response code="200">Status changed successfully. Returns updated application information.</response>
        /// <response code="400">Invalid status change request or validation failed.</response>
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpPatch("application/{id:Guid}/status")]
        public async Task<ActionResult<Result<ApplicationFormListResponseDto>>> ChangeApplicationStatus(Guid id, [FromBody] ChangeApplicationStatusDto dto)
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
        /// <response code="200">Credentials renewed successfully. Returns new credentials/info.</response>
        /// <response code="400">Failed to renew credentials due to validation or business rules.</response>
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpPatch("application/{id:Guid}/renew-credentials")]
        public async Task<ActionResult<Result<ApplicationFormListResponseDto>>> RenewApplicationCredentials(Guid id)
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
        /// <response code="200">Expiration date updated successfully.</response>
        /// <response code="400">Invalid expiration date or request failed validation.</response>
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpPatch("application/{id:Guid}/renew-expiration")]
        public async Task<ActionResult<Result<ApplicationFormListResponseDto>>> RenewApplicationExpirationDate(Guid id, [FromBody] RenewApplicationExpirationDateDto dto)
        {
            var command = new RenewApplicationExpirationDateCommand(id, dto.NewExpirationDate);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        /// <summary>
        /// Soft delete an application (mark IsDeleted=true and IsActive=false).
        /// </summary>
        /// <response code="200">Application soft-deleted successfully.</response>
        /// <response code="400">Failed to delete the application due to validation or business rules.</response>
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status200OK)]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OkRequestExample))]
        //===========================================
        [ProducesResponseType(typeof(Result<CustomEmptyResult>), StatusCodes.Status400BadRequest)]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(BadRequestExample))]
        [HttpDelete("application/{id:Guid}")]
        public async Task<ActionResult<Result<object>>> DeleteApplication(Guid id)
        {
            var command = new DeleteApplicationCommand(id);
            var result = await _mediator.Send(command);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }
    }
}
