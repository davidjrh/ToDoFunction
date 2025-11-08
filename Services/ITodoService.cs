using System.Collections.Generic;
using System.Threading.Tasks;
using ToDoFunction.Models;

namespace ToDoFunction.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItem>> GetAllTodosAsync();
        Task<TodoItem> GetTodoByIdAsync(int id);
        Task<TodoItem> CreateTodoAsync(string title, string description = "");
        Task<TodoItem> UpdateTodoAsync(int id, string title, string description = "", bool isCompleted = false);
        Task<bool> DeleteTodoAsync(int id);
    }
}