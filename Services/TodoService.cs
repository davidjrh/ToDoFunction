using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ToDoFunction.Data;
using ToDoFunction.Models;

namespace ToDoFunction.Services
{
    public class TodoService : ITodoService
    {
        private readonly TodoContext _context;
        private readonly ILogger<TodoService> _logger;

        public TodoService(TodoContext context, ILogger<TodoService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<TodoItem>> GetAllTodosAsync()
        {
            _logger.LogInformation("Getting all todos");
            try
            {
                return await _context.TodoItems.ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todos");
                throw;
            }
        }

        public async Task<TodoItem> GetTodoByIdAsync(int id)
        {
            _logger.LogInformation("Getting todo with ID: {Id}", id);
            try
            {
                var todo = await _context.TodoItems.FindAsync(id);
                if (todo == null)
                {
                    throw new ArgumentException($"Todo with ID {id} not found");
                }
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo with ID: {Id}", id);
                throw;
            }
        }

        public async Task<TodoItem> CreateTodoAsync(string title, string description = "")
        {
            _logger.LogInformation("Creating new todo: {Title}", title);

            if (string.IsNullOrWhiteSpace(title))
            {
                throw new ArgumentException("Title is required");
            }

            try
            {
                var todo = new TodoItem
                {
                    Title = title,
                    Description = description,
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow
                };

                _context.TodoItems.Add(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully created todo with ID: {Id}", todo.Id);
                return todo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                throw;
            }
        }

        public async Task<TodoItem> UpdateTodoAsync(int id, string title, string description = "", bool isCompleted = false)
        {
            _logger.LogInformation("Updating todo with ID: {Id}", id);

            

            try
            {
                var existingTodo = await _context.TodoItems.FindAsync(id);
                if (existingTodo == null)
                {
                    throw new ArgumentException($"Todo with ID {id} not found");
                }

                if (!string.IsNullOrWhiteSpace(title))
                {
                    existingTodo.Title = title;
                }

                if (!string.IsNullOrWhiteSpace(description))
                {
                    existingTodo.Description = description;
                }
                
                // Handle completion status change
                if (isCompleted && !existingTodo.IsCompleted)
                {
                    existingTodo.CompletedAt = DateTime.UtcNow;
                }
                else if (!isCompleted)
                {
                    existingTodo.CompletedAt = null;
                }
                existingTodo.IsCompleted = isCompleted;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully updated todo with ID: {Id}", id);
                return existingTodo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteTodoAsync(int id)
        {
            _logger.LogInformation("Deleting todo with ID: {Id}", id);
            try
            {
                var todo = await _context.TodoItems.FindAsync(id);
                if (todo == null)
                {
                    return false;
                }

                _context.TodoItems.Remove(todo);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Successfully deleted todo with ID: {Id}", id);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID: {Id}", id);
                throw;
            }
        }
    }
}