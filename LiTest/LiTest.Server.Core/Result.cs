using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Core
{
    public class Result<T>
    {
        public Result(bool isSuccess, T? value, string[] errorCodes, string[]? errorMessages = null, string[]? successMessages = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorCodes = errorCodes;
            ErrorMessages = errorMessages ?? [];
            SuccessMessages = successMessages ?? [];
        }

        public bool IsSuccess { get; }
        public T? Value { get; }
        public string[] ErrorCodes { get; }
        public string[] ErrorMessages { get; }
        public string[] SuccessMessages { get; }


        public static Result<T> Success(T value, string[]? messages = null) => new Result<T>(true, value, [], [], messages);
        public static Result<T> Failure(string[] errorCodes, string[]? errorMessages = null) 
            => new Result<T>(false, default, errorCodes, errorMessages);
        public static Result<T> Failure(string errorCode, string? errorMessage = null) 
            => new Result<T>(false, default, [errorCode], errorMessage != null ? [errorMessage] : []);
    }
}
