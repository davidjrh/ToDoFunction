using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;
using ToDoFunction.Services;
using System;
using System.Threading.Tasks;
using static ToDoFunction.MCP.TodoToolsInformation;

namespace ToDoFunction.MCP;

public class TodoMcpTools
{
    private readonly ITodoService _todoService;
    private readonly ILogger<TodoMcpTools> _logger;

    public TodoMcpTools(ITodoService todoService, ILogger<TodoMcpTools> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }


    [Function("McpGetTodos")]
    public async Task<object> GetTodos(
        [McpToolTrigger(GetTodosToolName, GetTodosToolDescription)] ToolInvocationContext context
    )
    {
        _logger.LogInformation("Getting all todos via MCP");
        try
        {
            var todos = await _todoService.GetAllTodosAsync();
            return todos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todos via MCP");
            return new { error = "Failed to retrieve todos", details = ex.Message };
        }
    }

    [Function("McpGetTodoById")]
    public async Task<object> GetTodoById(
        [McpToolTrigger(GetTodoByIdToolName, GetTodoByIdToolDescription)] ToolInvocationContext context,
        [McpToolProperty(IdPropertyName, IdPropertyDescription, true)] int id
    )
    {
        _logger.LogInformation("Getting todo by ID via MCP: {Id}", id);
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            return todo;
        }
        catch (ArgumentException)
        {
            return new { error = "Todo not found", id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo by ID via MCP");
            return new { error = "Failed to retrieve todo", details = ex.Message };
        }
    }

    [Function("McpSaveTodo")]
    public async Task<object> SaveTodo(
        [McpToolTrigger(SaveTodoToolName, SaveTodoToolDescription)] ToolInvocationContext context,
        [McpToolProperty(TitlePropertyName, TitlePropertyDescription, true)] string title,
        [McpToolProperty(DescriptionPropertyName, DescriptionPropertyDescription)] string description = ""
    )
    {
        _logger.LogInformation("Creating new todo via MCP: {Title}", title);
        
        try
        {
            var todo = await _todoService.CreateTodoAsync(title, description);
            
            _logger.LogInformation("Successfully created todo with ID: {Id}", todo.Id);
            return todo;
        }
        catch (ArgumentException ex)
        {
            return new { error = ex.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo via MCP");
            return new { error = "Failed to create todo", details = ex.Message };
        }
    }

    [Function("McpUpdateTodo")]
    public async Task<object> UpdateTodo(
        [McpToolTrigger(UpdateTodoToolName, UpdateTodoToolDescription)] ToolInvocationContext context,
        [McpToolProperty(IdPropertyName, IdPropertyDescription, true)] int id,
        [McpToolProperty(TitlePropertyName, TitlePropertyDescription, true)] string title,
        [McpToolProperty(DescriptionPropertyName, DescriptionPropertyDescription)] string description = "",
        [McpToolProperty(CompletedPropertyName, CompletedPropertyDescription)] bool completed = false
    )
    {
        _logger.LogInformation("Updating todo via MCP: {Id}", id);
        
        try
        {
            var existingTodo = await _todoService.UpdateTodoAsync(id, title, description, completed);
            
            _logger.LogInformation("Successfully updated todo with ID: {Id}", id);
            return existingTodo;
        }
        catch (ArgumentException ex)
        {
            return new { error = ex.Message };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo via MCP");
            return new { error = "Failed to update todo", details = ex.Message };
        }
    }

    [Function("McpDeleteTodo")]
    public async Task<object> DeleteTodo(
        [McpToolTrigger(DeleteTodoToolName, DeleteTodoToolDescription)] ToolInvocationContext context,
        [McpToolProperty(IdPropertyName, IdPropertyDescription, true)] int id
    )
    {
        _logger.LogInformation("Deleting todo via MCP: {Id}", id);
        try
        {
            var deleted = await _todoService.DeleteTodoAsync(id);
            
            if (!deleted)
            {
                return new { error = "Todo not found", id };
            }
            
            _logger.LogInformation("Successfully deleted todo with ID: {Id}", id);
            return new { success = true, message = "Todo deleted successfully", id };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo via MCP");
            return new { error = "Failed to delete todo", details = ex.Message };
        }
    }
}