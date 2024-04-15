using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NlpBridge.Models
{
    public class ExecutionResult<TClientResponse>
    {
        public bool IsSuccess { 
            get { return Error is not null; } 
        }

        public Error Error { get; set; }

        public TClientResponse Response { get; set; }

        public static ExecutionResult<TClientResponse> Success(TClientResponse response)
        {
            return new ExecutionResult<TClientResponse>
            {
                Response = response,
            };
        }

        public static ExecutionResult<TClientResponse> Failure(string message, Exception exception = null)
        {
            return new ExecutionResult<TClientResponse>
            {
                Error = new Error
                {
                    Message = message,
                    Exception = exception,
                }
            };
        }
    }

    public class Error
    {
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
