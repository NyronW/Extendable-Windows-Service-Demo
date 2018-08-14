using System;

namespace WillCorp.Core.Entity
{
    public class Todo
    {
        public Todo(string id, string description, Completed completed)
        {
            Contracts.Require(!string.IsNullOrWhiteSpace(description), "Description is required");
            Id = string.IsNullOrWhiteSpace(id) ? GenerateId() : id;
            Description = description;
            Completed = completed;
        }

        public Todo(string description) : this(string.Empty, description, null)
        {

        }

        public string Id { get; }
        public string Description { get; }
        public Completed Completed { get; private set; }

        public Result MarkAsCompleted()
        {
            if (Completed != null) return Result.Fail("Task already completed");

            Completed = new Completed(DateTime.Now);
            return Result.Ok();
        }

        public bool IsCompleted => Completed != null;

        private static string GenerateId()
        {
            var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray()).Replace("==", "");

            if (id.Contains("/"))
            {
                id = id.Replace("/", "");
            }

            return id;
        }
    }

    public class Completed
    {
        public Completed(DateTime date)
        {
            Contracts.Require(date != null, "Date is required");
            Contracts.Require(date <= DateTime.Now, "Completed date cannot be in the future");

            Date = date;
        }

        public DateTime Date { get; }
    }
}
