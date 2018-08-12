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

        public Result MarkAsCompleted(string user)
        {
            if (Completed != null) return Result.Fail("Task already completed");
            if(string.IsNullOrEmpty(user)) return Result.Fail("Invalid user");

            Completed = new Completed(user, DateTime.Now);
            return Result.Ok();
        }

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
        public Completed(string user, DateTime date)
        {
            Contracts.Require(!string.IsNullOrWhiteSpace(user), "Description is required");
            Contracts.Require(date != null, "Date is required");

            User = user;
            Date = date;
        }

        public string User { get; }
        public DateTime Date { get; }
    }
}
