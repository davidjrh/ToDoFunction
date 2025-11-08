# TODO API - Azure Functions

A simple CRUD API for managing TODO items built with Azure Functions and .NET 8, featuring **Model Context Protocol (MCP) Server** capabilities for AI-powered integrations.

Authors: 
- [@PabloSR06](https://github.com/PabloSR06)
- [@davidjrh](https://github.com/davidjrh)


## Features

- Create, Read, Update, Delete TODO items
- Toggle completion status
- In-memory database (EntityFramework Core)
- RESTful API design
- Comprehensive error handling
- **MCP Server integration** - Expose TODO operations as AI-accessible tools using the [Microsoft.Azure.Functions.Worker.Extensions.Mcp](https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Extensions.Mcp) NuGet package

## API Endpoints

### Health Check
- **GET** `/api/health` - Check if the API is running

### TODO Operations

#### Get All TODOs
- **GET** `/api/todos`
- Returns all TODO items

#### Get TODO by ID
- **GET** `/api/todos/{id}`
- Returns a specific TODO item by ID

#### Create New TODO
- **POST** `/api/todos`
- Creates a new TODO item
- **Request Body:**
```json
{
  "title": "Task title (required)",
  "description": "Task description (optional)"
}
```

#### Update TODO
- **PUT** `/api/todos/{id}`
- Updates an existing TODO item
- **Request Body:**
```json
{
  "title": "Updated title (required)",
  "description": "Updated description (optional)",
  "isCompleted": true
}
```

#### Delete TODO
- **DELETE** `/api/todos/{id}`
- Deletes a TODO item

#### Toggle Completion
- **PATCH** `/api/todos/{id}/toggle`
- Toggles the completion status of a TODO item

## TODO Item Structure

```json
{
  "id": 1,
  "title": "Sample Task",
  "description": "This is a sample task",
  "isCompleted": false,
  "createdAt": "2025-10-08T10:00:00Z",
  "completedAt": null
}
```

## Running the Application

1. Make sure you have the .NET 8 SDK installed
2. Install Azure Functions Core Tools
3. Navigate to the project directory
4. Run `func start` to start the local development server
5. The API will be available at `http://localhost:7071`

## Testing the API

You can test the API using tools like:
- Postman
- curl
- VS Code REST Client extension

### Example curl commands:

```bash
# Health check
curl http://localhost:7071/api/health

# Get all todos
curl http://localhost:7071/api/todos

# Create a new todo
curl -X POST http://localhost:7071/api/todos \
  -H "Content-Type: application/json" \
  -d '{"title": "My first todo", "description": "This is a test todo"}'

# Get todo by ID
curl http://localhost:7071/api/todos/1

# Update todo
curl -X PUT http://localhost:7071/api/todos/1 \
  -H "Content-Type: application/json" \
  -d '{"title": "Updated todo", "description": "Updated description", "isCompleted": true}'

# Toggle completion
curl -X PATCH http://localhost:7071/api/todos/1/toggle

# Delete todo
curl -X DELETE http://localhost:7071/api/todos/1
```

## Project Structure

```
ToDoFunction/
├── Functions/
│   ├── TodoFunctions.cs     # Main CRUD operations
│   └── HealthCheck.cs       # Health check endpoint
├── Models/
│   └── TodoItem.cs          # TODO item data model
├── Data/
│   └── TodoContext.cs       # Entity Framework DbContext
├── Services/
│   ├── ITodoService.cs      # Todo service interface
│   └── TodoService.cs       # Todo service implementation
├── MCP/
│   ├── TodoMcpTools.cs      # MCP Server tool implementations
│   └── TodoToolsInformation.cs  # MCP tool metadata and descriptions
├── Program.cs               # Application startup and DI configuration
├── ToDoFunction.csproj      # Project file with MCP extension
├── host.json               # Azure Functions configuration
├── local.settings.json     # Local development settings
└── mcp.json                # MCP Server configuration
```

## MCP Server Integration

This Azure Functions app doubles as a **Model Context Protocol (MCP) Server**, allowing AI assistants and agents to interact with the TODO list through standardized tool interfaces.

### What is MCP?

The Model Context Protocol (MCP) is an open protocol that standardizes how applications provide context to Large Language Models (LLMs). By implementing MCP, this TODO API can be seamlessly integrated into AI workflows, enabling AI assistants to manage tasks on your behalf.

### MCP Tools Available

The following MCP tools are exposed by this server:

| Tool Name | Description | Parameters |
|-----------|-------------|------------|
| `create_task` | Create a new task in your personal todo list | `task_title` (required), `task_description` (optional) |
| `list_all_tasks` | View all tasks with their details | None |
| `find_task_by_id` | Look up a specific task by its unique ID | `task_id` (required) |
| `modify_existing_task` | Update task details or completion status | `task_id` (required), `task_title` (required), `task_description` (optional), `is_completed` (optional) |
| `remove_task` | Permanently delete a task | `task_id` (required) |

### Configuration

The MCP Server is configured via the `mcp.json` file, which defines both local and remote server endpoints:

```json
{
    "inputs": [
        {
            "type": "promptString",
            "id": "functions-mcp-extension-system-key",
            "description": "Azure Functions MCP Extension System Key",
            "password": true
        },
        {
            "type": "promptString",
            "id": "functionapp-name",
            "description": "Azure Functions App Name"
        }
    ],
    "servers": {
        "local-mcp-todo-function": {
            "type": "http",
            "url": "http://0.0.0.0:7071/runtime/webhooks/mcp"
        },
        "remote-mcp-todo-function": {
            "type": "http",
            "url": "https://${input:functionapp-name}.azurewebsites.net/runtime/webhooks/mcp",
            "headers": {
                "x-functions-key": "${input:functions-mcp-extension-system-key}"
            }
        }
    }
}
```

### Using the MCP Server

#### Local Development

When running locally with `func start`, the MCP Server is available at:
```
http://localhost:7071/runtime/webhooks/mcp
```

#### Production (Azure)

When deployed to Azure, the MCP Server endpoint is:
```
https://<your-function-app>.azurewebsites.net/runtime/webhooks/mcp
```

**Note:** You'll need to provide the system key via the `x-functions-key` header for authentication.

#### Connecting with MCP Clients

To connect an MCP-compatible client (like Claude Desktop, VS Code with MCP extension, or custom MCP clients):

1. Configure the client to use the MCP server URL
2. For local testing, use: `http://localhost:7071/runtime/webhooks/mcp`
3. For production, use the Azure Functions URL with the system key

#### Example MCP Tool Usage

Once connected, AI assistants can interact with your TODO list using natural language:

- "Create a task to buy groceries"
- "Show me all my tasks"
- "Mark task 3 as completed"
- "Delete the task about the meeting"

### Implementation Details

The MCP integration uses the **Microsoft.Azure.Functions.Worker.Extensions.Mcp** NuGet package, which provides:

- `[McpToolTrigger]` - Attribute to expose Azure Functions as MCP tools
- `[McpToolProperty]` - Attribute to define tool parameters with descriptions
- `ToolInvocationContext` - Context object for MCP tool invocations

Example implementation:

```csharp
[Function("McpSaveTodo")]
public async Task<object> SaveTodo(
    [McpToolTrigger(SaveTodoToolName, SaveTodoToolDescription)] ToolInvocationContext context,
    [McpToolProperty(TitlePropertyName, TitlePropertyDescription, true)] string title,
    [McpToolProperty(DescriptionPropertyName, DescriptionPropertyDescription)] string description = ""
)
{
    // Implementation
}
```

### Benefits of MCP Integration

- **AI-Powered Task Management**: Let AI assistants manage your tasks through natural conversation
- **Standardized Interface**: Works with any MCP-compatible client
- **Secure**: Uses Azure Functions authentication for remote access
- **Extensible**: Easy to add new tools and capabilities
- **Developer-Friendly**: Simple attribute-based programming model

## Notes

- This implementation uses an in-memory database, so data will be lost when the application restarts
- For production use, consider using a persistent database like SQL Server, CosmosDB, or PostgreSQL
- The API uses function-level authorization by default
- All timestamps are in UTC format
- The MCP Server endpoint is automatically secured with Azure Functions system keys in production
- MCP tools provide a standardized way for AI assistants to interact with your TODO list

## Resources

- [Model Context Protocol (MCP) Documentation](https://modelcontextprotocol.io/)
- [Microsoft.Azure.Functions.Worker.Extensions.Mcp NuGet Package](https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Extensions.Mcp)
- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/)