using System;
using System.Collections.Generic;
using System.Text;

namespace LiTest.Server.Services
{
    public class Result<T>
    {
        public Result(bool isSuccess, T? value, string[] errorCodes, string[]? errorMessages = null)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorCodes = errorCodes;
            ErrorMessages = errorMessages ?? [];
        }

        public bool IsSuccess { get; }
        public T? Value { get; }
        public string[] ErrorCodes { get; }
        public string[] ErrorMessages { get; }


        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string[] errorCodes, string[]? errorMessages = null) 
            => new Result<T>(false, default, errorCodes, errorMessages);
        public static Result<T> Failure(string errorCode, string? errorMessage = null) 
            => new Result<T>(false, default, [errorCode], errorMessage != null ? [errorMessage] : []);
    }
}
