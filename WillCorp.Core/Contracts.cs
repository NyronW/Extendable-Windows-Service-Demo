using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace WillCorp
{
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