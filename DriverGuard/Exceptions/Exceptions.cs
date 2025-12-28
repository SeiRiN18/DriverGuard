namespace DriverGuard.Backend.Exceptions
{
    public abstract class BusinessException : Exception
    {
        public int StatusCode { get; }

        protected BusinessException(string message, int statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class NotFoundException : BusinessException
    {
        public NotFoundException(string message)
            : base(message, 404) { }
    }

    public class ForbiddenException : BusinessException
    {
        public ForbiddenException(string message)
            : base(message, 403) { }
    }

    public class ValidationException : BusinessException
    {
        public ValidationException(string message)
            : base(message, 400) { }
    }


}
