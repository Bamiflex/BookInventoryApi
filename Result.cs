namespace SqlBasedBookAPI.data
{
    public class Result
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

        // Optional field specifically for JWT token
        public string Token { get; set; }

        public Result() { }

        public Result(bool success, string message, object data = null, string token = null)
        {
            Success = success;
            Message = message;
            Data = data;
            Token = token;  // Allow JWT token if applicable
        }

        public Result(string message)
        {
            Success = false;
            Message = message;
            Data = null;
            Token = null;
        }
    }
}
