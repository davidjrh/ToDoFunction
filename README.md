# TODO API - Azure Functions

A simple CRUD API for managing TODO items built with Azure Functions and .NET 8.

## Features

- Create, Read, Update, Delete TODO items
- Toggle completion status
- In-memory database (EntityFramework Core)
- RESTful API design
- Comprehensive error handling

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
├── Startup.cs               # Dependency injection configuration
├── ToDoFunction.csproj      # Project file
├── host.json               # Azure Functions configuration
└── local.settings.json     # Local development settings
```

## Notes

- This implementation uses an in-memory database, so data will be lost when the application restarts
- For production use, consider using a persistent database like SQL Server, CosmosDB, or PostgreSQL
- The API uses function-level authorization by default
- All timestamps are in UTC format