using Application_Form.Domain.Common;
using Swashbuckle.AspNetCore.Filters;

namespace Application_Form.SwaggerDocs
{
    public class BadRequestExample : IExamplesProvider<Result<CustomEmptyResult>>
    {
        public Result<CustomEmptyResult> GetExamples()
        {
            return new Result<CustomEmptyResult>
            {
                Success = false,
                Message = "Error Message",
                Data = null
            };
        }
    }
}
