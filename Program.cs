using Microsoft.EntityFrameworkCore;
using ProjectmanagementAPI;
using ProjectmanagementAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services
builder.Services.AddControllers();

// Register CORS policy (Allows all origins, headers, and methods)
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Register DB Context & Dapper
builder.Services.AddDbContext<ProjectManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<ProjectmanagementAPI.Data.SqlConnectionFactory>();

// Register Swagger & GraphQL
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddErrorFilter<GraphQLErrorFilter>();

// 👇 ADDED THIS LINE TO ALLOW EXTERNAL NETWORK CONNECTIONS (PHONE) 👇
builder.WebHost.UseUrls("http://0.0.0.0:5085");

var app = builder.Build();

// 2. Configure HTTP Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// MUST COME FIRST to handle preflight CORS checks
app.UseCors();

// Disabled for local dev to prevent CORS 307 preflight redirect errors
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();
app.MapGraphQL();
app.Run();