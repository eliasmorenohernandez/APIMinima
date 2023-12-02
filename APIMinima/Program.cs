using APIMinima;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var app = builder.Build();

//var todoItems = app.MapGroup("/todoitems");

//todoItems.MapGet("/", GetAllTodos);
//todoItems.MapGet("/complete", GetComplete);
//todoItems.MapGet("/{id}", GetTodo);
//todoItems.MapPost("/", CreateTodo);
//todoItems.MapPut("/{id}", UpdateTodo);
//todoItems.MapDelete("/{id}", DeleteTodo);

RouteGroupBuilder todoItems = app.MapGroup("/todoitems");
todoItems.MapGet("/", GetAllTodos);
todoItems.MapGet("/complete", GetComplete);
todoItems.MapGet("/{id}", GetTodo);
todoItems.MapPost("/", CreateTodo);
todoItems.MapPut("/{id}", UpdateTodo);
todoItems.MapDelete("/{id}", DeleteTodo);


app.Run();

//* Version 3*/

static async Task<IResult> GetAllTodos(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Select(x =>new TodoItemDTO(x)).ToArrayAsync());
}

static async Task<IResult> GetComplete(TodoDb db)
{
    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).Select(x => new TodoItemDTO(x)).ToListAsync());
}

static async Task<IResult> GetTodo(int id, TodoDb db)
{
    return await db.Todos.FindAsync(id)
        is Todo todo
        ? TypedResults.Ok(new TodoItemDTO(todo))
        : TypedResults.NotFound();
}

static async Task<IResult> CreateTodo(TodoItemDTO todoItemDto, TodoDb db)
{
    var todoItem = new Todo
    {
        Name = todoItemDto.Name,
        IsComplete = todoItemDto.IsComplete
    };

    db.Todos.Add(todoItem);
    await db.SaveChangesAsync();
    todoItemDto = new TodoItemDTO(todoItem);
    return TypedResults.Created($"/todoitems/{todoItem.Id}", todoItemDto);
}

static async Task<IResult> UpdateTodo(int id, TodoItemDTO todoItemDto, TodoDb db)
{
    var todo = await db.Todos.FindAsync(id);
    if (todo is null) return TypedResults.NotFound();
    todo.Name = todoItemDto.Name;
    todo.IsComplete = todoItemDto.IsComplete;
    await db.SaveChangesAsync();
    return TypedResults.NoContent();
}

static async Task<IResult> DeleteTodo(int id, TodoDb db)
{
    if (await db.Todos.FindAsync(id) is Todo todo)
    {
        db.Todos.Remove(todo);
        await db.SaveChangesAsync();

        TodoItemDTO todoItemDto  = new TodoItemDTO(todo);

        return TypedResults.Ok(todoItemDto);
    }
    return TypedResults.NotFound();
}


//* Version 2 */
//static async Task<IResult> GetAllTodos(TodoDb db)
//{
//    return TypedResults.Ok(await db.Todos.ToListAsync());
//}

//static async Task<IResult> GetComplete(TodoDb db)
//{
//    return TypedResults.Ok(await db.Todos.Where(t => t.IsComplete).ToListAsync());
//}

//static async Task<IResult> GetTodo(int id, TodoDb db)
//{
//    return await db.Todos.FindAsync(id)
//        is Todo todo
//        ? TypedResults.Ok(todo)
//        : TypedResults.NotFound();
//}

//static async Task<IResult> CreateTodo(Todo todo, TodoDb db)
//{
//    db.Todos.Add(todo);
//    await db.SaveChangesAsync();
//    return TypedResults.Created($"/todoitems/{todo.Id}", todo);
//}

//static async Task<IResult> UpdateTodo(int id, Todo inputTodo, TodoDb db)
//{
//    var todo = await db.Todos.FindAsync(id);
//    if(todo is null) return TypedResults.NotFound();
//    todo.Name= inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;
//    await db.SaveChangesAsync();
//    return TypedResults.NoContent();
//}

//static async Task<IResult> DeleteTodo(int id, TodoDb db)
//{
//    if(await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return TypedResults.Ok(todo);
//    }
//    return TypedResults.NotFound();
//}

/* Version 1*/

//todoItems.MapGet("/", async (TodoDb db) =>
//    await db.Todos.ToListAsync()
//);

//todoItems.MapGet("/complete", async (TodoDb db) =>
//    await db.Todos.Where(t => t.IsComplete).ToListAsync()
//);

//todoItems.MapGet("/{id}", async (int id, TodoDb db) => 
//    await db.Todos.FindAsync(id)
//    is Todo todo
//    ? Results.Ok(todo)
//    : Results.NotFound()
//);


//todoItems.MapPost("/", async (Todo todo, TodoDb db) =>
//{
//    db.Todos.Add(todo);
//    await db.SaveChangesAsync();
//    return Results.Created($"/todoitems/{todo.Id}", todo);
//}
//);

//todoItems.MapPut("/{id}", async(int id, Todo inputTodo, TodoDb db) =>
//{
//    var todo = await db.Todos.FindAsync(id);

//    if(todo is null)
//    {
//        return Results.NotFound();
//    }

//    todo.Name = inputTodo.Name;
//    todo.IsComplete = inputTodo.IsComplete;

//    await db.SaveChangesAsync();

//    return Results.NoContent();
//});

//todoItems.MapDelete("/{id}", async (int id, TodoDb db) =>
//{
//    if(await db.Todos.FindAsync(id) is Todo todo)
//    {
//        db.Todos.Remove(todo);
//        await db.SaveChangesAsync();
//        return Results.Ok(todo);
//    }
//    return Results.NotFound();

//});

//app.Run();
