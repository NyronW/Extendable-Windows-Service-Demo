using System;
using System.Collections.Generic;
using System.Linq;

namespace WillCorp
{
    public class Result
    {
        public bool Success { get; }
        public string Error { get; }
        public bool Failure => !Success;

        protected Result(bool success, string error)
        {
            Contracts.Require(success || !string.IsNullOrEmpty(error));
            Contracts.Require(!success || string.IsNullOrEmpty(error));

            Success = success;
            Error = error;
        }

        public static Result Fail(string message)
        {
            return new Result(false, message);
        }

        public static Result<T> Fail<T>(string message)
        {
            return new Result<T>(default(T), false, message);
        }

        public static Result Ok()
        {
            return new Result(true, string.Empty);
        }

        public static Result<T> Ok<T>(T value)
        {
            return new Result<T>(value, true, string.Empty);
        }

        public static Result Combine(params Result[] results)
        {
            foreach (Result result in results)
            {
                if (result.Failure)
                    return result;
            }

            return Ok();
        }
    }

    public class Result<T> : Result
    {
        private readonly T _value;

        public T Value
        {
            get
            {
                Contracts.Require(Success);

                return _value;
            }
        }

        protected internal Result(T value, bool isSuccess, string error)
            : base(isSuccess, error)
        {
            Contracts.Require(value != null || !Success);

            _value = value;
        }
    }
}
