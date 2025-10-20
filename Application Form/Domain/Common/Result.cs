using Application_Form.Application.DTOs;

namespace Application_Form.Domain.Common
{
    public class Result<T> where T : class
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static Result<T> SuccessResult(T data, string message = "")
            => new() { Success = true, Data = data, Message = message };

        public static Result<T> Failure(string message)
            => new() { Success = false, Message = message };
    }
}
