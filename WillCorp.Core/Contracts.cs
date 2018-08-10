using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillCorp
{
    /// <summary>
    /// This class was designed to perform simple domain
    /// validation. its basocally an abstraction over
    /// the "if not valid then throw an exception" coding pattern
    /// it will throw a custonm exception, which error handling code 
    /// can catch and handle as needed.
    /// Checkout https://enterprisecraftsmanship.com/2015/02/14/code-contracts-vs-input-validation/ for more details
    /// </summary>
    public static class Contracts
    {
        [DebuggerStepThrough]
        public static void Require(bool precondition, string message = "")
        {
            if (!precondition)
                throw new ContractException(message);
        }
    }

    [Serializable]
    public class ContractException : Exception
    {
        private readonly List<string> _messages = new List<string>();

        public ContractException(string message)
            : base(message)
        {
            _messages.Add(message);
        }

        public ContractException(IEnumerable<string> messages)
                : base("One or More Errors occured")
        {
            _messages.AddRange(messages);
        }

        public IReadOnlyList<string> Errors => _messages;
    }
}