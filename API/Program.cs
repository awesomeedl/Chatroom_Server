using API;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChatContext>(opt => opt.UseSqlite("Data Source=Chat.db"));
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapPost("login", async (User user, ChatContext chatContext) =>
{
    var result = await chatContext.Users.AsNoTracking().SingleOrDefaultAsync(a => string.Equals(a.Username, user.Username));
    
    if(result is null)
    {
        return Results.NotFound("user not found");
    }
    
    if (!string.Equals(result.Password, user.Password))
    {
        return Results.BadRequest("incorrect password");
    }

    return Results.Ok("login successful");
})
.WithOpenApi();

app.MapPost("register", async (User user, ChatContext chatContext) =>
{
    var result = await chatContext.Users.SingleOrDefaultAsync(a => string.Equals(a.Username, user.Username));

    if (result is not null)
    {
        return Results.BadRequest("username already exists");
    }

    chatContext.Users.Add(user);
    await chatContext.SaveChangesAsync();

    return Results.Ok("signup successful");
})
.WithOpenApi();

app.MapPost("send", async (ChatMessage msg, ChatContext chatContext) =>
{
    chatContext.Messages.Add(msg);
    await chatContext.SaveChangesAsync();

    return Results.Ok("message sent");
})
.WithOpenApi(); 

app.MapGet("history", async (ChatContext chatContext) => Results.Ok(await chatContext.Messages.AsNoTracking().ToListAsync())).WithOpenApi();

app.MapGet("history/{id:int}", async (int id, ChatContext chatContext) => Results.Ok(await chatContext.Messages.AsNoTracking().Where(m => m.Id > id).ToListAsync())).WithOpenApi();

app.Run();