namespace HZC.Core
{
    public class Result<T> : Result
    {
        public T Body { get; set; }

        public Result()
        { }

        public Result(int code, T body, string message = "")
        {
            Code = code;
            Body = body == null ? default(T) : body;
            Message = message;
        }
    }
}
