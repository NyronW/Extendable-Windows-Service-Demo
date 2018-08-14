using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WillCorp.Core.Entity;

namespace WillCorp.App.Web.Model
{
    public class TodoDataStore
    {
        static ConcurrentDictionary<string, Todo> todoItems = new ConcurrentDictionary<string, Todo>();

        public IReadOnlyCollection<Todo> Get() => todoItems.Values.ToList();

        public Todo GetById(string id)
        {
            if (todoItems.TryGetValue(id, out Todo todo)) return todo;

            return null;
        }

        public Result<Todo> Add(string description)
        {
            var todo = new Todo(description);

            if (todoItems.TryAdd(todo.Id, todo)) return Result.Ok(todo);

            return Result.Fail<Todo>("Item not created");
        }

        public Result Update(TodoModel model)
        {
            if (model == null) return Result.Fail<Todo>("No data recieved");
            if (string.IsNullOrEmpty(model.Id)) return Result.Fail<Todo>("Id is required");
            if (!todoItems.TryGetValue(model.Id, out Todo todo)) return Result.Fail<Todo>("Item Not Found");

            Todo newTodo = new Todo(todo.Id, todo.Description, todo.Completed);

            if (model.Completed)
            {
                var result = newTodo.MarkAsCompleted();
                if (result.Failure) return Result.Fail<Todo>(result.Error);
            }
            else
            {
                newTodo = new Todo(todo.Id, todo.Description, null);
            }

            if (todoItems.TryUpdate(todo.Id, newTodo, todo))
            {
                return Result.Ok(newTodo);
            }
            else
                return Result.Fail<Todo>("Item not created");
        }
    }
}
