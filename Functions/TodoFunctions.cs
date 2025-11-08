using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using ToDoFunction.Models;
using ToDoFunction.Services;
using System.IO;

namespace ToDoFunction.Functions
{
    public class TodoFunctions
    {
        private readonly ITodoService _todoService;
        private readonly ILogger _logger;

        public TodoFunctions(ITodoService todoService, ILoggerFactory loggerFactory)
        {
            _todoService = todoService;
            _logger = loggerFactory.CreateLogger<TodoFunctions>();
        }


        // GET: Get all todos
        [Function("GetAllTodos")]
        public async Task<HttpResponseData> GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos")] HttpRequestData req)
        {
            try
            {
                var todos = await _todoService.GetAllTodosAsync();
                
                var response = req.CreateResponse(HttpStatusCode.OK);
                //response.Headers.Add("Content-Type", "application/json; charset=utf-8");
                await response.WriteAsJsonAsync(todos);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todos");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // GET: Get todo by ID
        [Function("GetTodoById")]
        public async Task<HttpResponseData> GetTodoById(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "todos/{id:int}")] HttpRequestData req)
        {
            // Extract ID from route
            var routeData = req.FunctionContext.BindingContext.BindingData;
            if (!routeData.TryGetValue("id", out var idObj) || !int.TryParse(idObj?.ToString(), out int id))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid ID");
                return badResponse;
            }

            try
            {
                var todo = await _todoService.GetTodoByIdAsync(id);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(todo);
                return response;
            }
            catch (ArgumentException)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting todo with ID: {Id}", id);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // POST: Create new todo
        [Function("CreateTodo")]
        public async Task<HttpResponseData> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "todos")] HttpRequestData req)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var todoData = JsonSerializer.Deserialize<TodoItem>(requestBody);

                if (todoData == null || string.IsNullOrWhiteSpace(todoData.Title))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Title is required");
                    return badResponse;
                }

                var todo = await _todoService.CreateTodoAsync(todoData.Title, todoData.Description);

                var response = req.CreateResponse(HttpStatusCode.Created);
                response.Headers.Add("Location", $"/api/todos/{todo.Id}");
                await response.WriteAsJsonAsync(todo);
                return response;
            }
            catch (ArgumentException ex)
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync(ex.Message);
                return badResponse;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating todo");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // PUT: Update existing todo
        [Function("UpdateTodo")]
        public async Task<HttpResponseData> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Function, "put", Route = "todos/{id:int}")] HttpRequestData req)
        {
            // Extract ID from route
            var routeData = req.FunctionContext.BindingContext.BindingData;
            if (!routeData.TryGetValue("id", out var idObj) || !int.TryParse(idObj?.ToString(), out int id))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid ID");
                return badResponse;
            }

            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
                var todoData = JsonSerializer.Deserialize<TodoItem>(requestBody);

                if (todoData == null || string.IsNullOrWhiteSpace(todoData.Title))
                {
                    var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                    await badResponse.WriteStringAsync("Title is required");
                    return badResponse;
                }

                var updatedTodo = await _todoService.UpdateTodoAsync(id, todoData.Title, todoData.Description, todoData.IsCompleted);

                var response = req.CreateResponse(HttpStatusCode.OK);
                await response.WriteAsJsonAsync(updatedTodo);
                return response;
            }
            catch (ArgumentException)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating todo with ID: {Id}", id);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }

        // DELETE: Delete todo
        [Function("DeleteTodo")]
        public async Task<HttpResponseData> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "todos/{id:int}")] HttpRequestData req)
        {
            // Extract ID from route
            var routeData = req.FunctionContext.BindingContext.BindingData;
            if (!routeData.TryGetValue("id", out var idObj) || !int.TryParse(idObj?.ToString(), out int id))
            {
                var badResponse = req.CreateResponse(HttpStatusCode.BadRequest);
                await badResponse.WriteStringAsync("Invalid ID");
                return badResponse;
            }

            try
            {
                var deleted = await _todoService.DeleteTodoAsync(id);
                
                if (!deleted)
                {
                    return req.CreateResponse(HttpStatusCode.NotFound);
                }

                return req.CreateResponse(HttpStatusCode.NoContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID: {Id}", id);
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}