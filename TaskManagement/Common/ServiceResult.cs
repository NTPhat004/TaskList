namespace TaskManagement.Common
{
    public enum ServiceResultStatus
    {
        Success,
        NotFound,
        ValidationError,
        ServerError
    }

    public class ServiceResult<T>
    {
        public ServiceResultStatus Status { get; private set; }
        public string Message { get; private set; }
        public List<string> Errors { get; private set; } = new List<string>();
        public T Data { get; private set; }

        private ServiceResult(ServiceResultStatus status, T data, string message = null, List<string> errors = null)
        {
            Status = status;
            Data = data;
            Message = message;
            Errors = errors ?? new List<string>();
        }

        public static ServiceResult<T> Success(T data) => new(ServiceResultStatus.Success, data);
        public static ServiceResult<T> NotFound(string message) => new(ServiceResultStatus.NotFound, default, message);
        public static ServiceResult<T> ValidationError(List<string> errors) => new(ServiceResultStatus.ValidationError, default, null, errors);
        public static ServiceResult<T> ServerError(string message) => new(ServiceResultStatus.ServerError, default, message);
    }
}
