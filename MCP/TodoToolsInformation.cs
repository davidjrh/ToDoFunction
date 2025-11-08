namespace ToDoFunction.MCP;

internal sealed class TodoToolsInformation
{
    public const string SaveTodoToolName = "create_task";
    public const string SaveTodoToolDescription = "Create a new task in your personal todo list. Perfect for adding things you need to remember or accomplish.";

    public const string GetTodosToolName = "list_all_tasks";
    public const string GetTodosToolDescription = "View all your tasks at once. Shows both completed and pending items with their details.";

    public const string GetTodoByIdToolName = "find_task_by_id";
    public const string GetTodoByIdToolDescription = "Look up a specific task using its unique ID number to see all its details.";

    public const string UpdateTodoToolName = "modify_existing_task";
    public const string UpdateTodoToolDescription = "Update any task details like title, description, or mark it as completed or pending.";

    public const string DeleteTodoToolName = "remove_task";
    public const string DeleteTodoToolDescription = "Permanently delete a task from your todo list. This action cannot be undone.";

    public const string TitlePropertyName = "task_title";
    public const string DescriptionPropertyName = "task_description";
    public const string IdPropertyName = "task_id";
    public const string CompletedPropertyName = "is_completed";

    public const string TitlePropertyDescription = "A clear, descriptive title for your task (e.g., 'Buy groceries', 'Finish project report').";
    public const string DescriptionPropertyDescription = "Optional detailed description with additional notes, context, or steps for completing this task.";
    public const string IdPropertyDescription = "The unique identifier number assigned to this specific task.";
    public const string CompletedPropertyDescription = "Set to true when the task is finished, false when it's still pending or in progress.";

}
    
   