using System;
using System.Collections.Generic;
using System.Linq;

namespace WillCorp.App.Web.Api.Models
{
    public class Envelope<T>
    {
        private List<string> _errorMessages = new List<string>();
        public T Result { get; }
        public IReadOnlyList<string> ErrorMessages => _errorMessages;
        public DateTime TimeGenerated { get; }

        protected internal Envelope(T result, string errorMessage) : this(result, new[] { errorMessage })
        {

        }

        protected internal Envelope(T result, IEnumerable<string> errorMessages)
        {
            Result = result;
            TimeGenerated = DateTime.UtcNow;
            if (errorMessages.Any(e => !string.IsNullOrEmpty(e))) _errorMessages.AddRange(errorMessages);
        }
    }

    public class Envelope : Envelope<string>
    {
        protected Envelope(string errorMessage)
            : base(null, errorMessage)
        {
        }

        protected Envelope(IEnumerable<string> errorMessages)
            : base(null, errorMessages)
        {
        }

        public static Envelope<T> Ok<T>(T result)
        {
            return new Envelope<T>(result, string.Empty);
        }

        public static Envelope Ok()
        {
            return new Envelope(string.Empty);
        }

        public static Envelope Error(string errorMessage)
        {
            return new Envelope(errorMessage);
        }

        public static Envelope Error(IEnumerable<string> errorMessages)
        {
            return new Envelope(errorMessages);
        }
    }
}
