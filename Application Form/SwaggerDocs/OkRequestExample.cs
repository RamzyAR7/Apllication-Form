using Application_Form.Domain.Common;
using Swashbuckle.AspNetCore.Filters;

namespace Application_Form.SwaggerDocs
{
    public class OkRequestExample : IExamplesProvider<Result<CustomEmptyResult>>
    {
        public Result<CustomEmptyResult> GetExamples()
        {
            return new Result<CustomEmptyResult>
            {
                Success = true,
                Message = string.Empty,
                Data = new CustomEmptyResult()
            };
        }
    }
}
