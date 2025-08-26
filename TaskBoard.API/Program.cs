using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using TaskBoard.API.Mapping;
using TaskBoard.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// 1) Controllers
builder.Services.AddControllers();

// 2) Health checks
builder.Services.AddHealthChecks();

// 3) SQL Server via appsettings (Development/Production). Fail fast if missing.
var cs = builder.Configuration.GetConnectionString("Sql");
if (string.IsNullOrWhiteSpace(cs))
    throw new InvalidOperationException("ConnectionStrings:Sql is missing. Add it to appsettings or user-secrets.");

// Use DbContext POOL for better perf
builder.Services.AddDbContextPool<TaskBoardDbContext>(options =>
    options.UseSqlServer(cs, sql =>
    {
        sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
        // Optional: point migrations assembly explicitly (DbContext is in Core)
        // sql.MigrationsAssembly(typeof(TaskBoardDbContext).Assembly.FullName);
    })
    .EnableDetailedErrors(builder.Environment.IsDevelopment())
    .EnableSensitiveDataLogging(builder.Environment.IsDevelopment()));

// 4) AutoMapper
builder.Services.AddAutoMapper(typeof(ApiMappingProfile).Assembly);
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    //var xml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    //var xmlPath = Path.Combine(AppContext.BaseDirectory, xml);
    //if (File.Exists(xmlPath))
    //    opt.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);

    opt.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "TaskBoard API",
        Version = "v1",
        Description = "API Controllers for TaskBoard (SQL Server)"
    });
});

var app = builder.Build();

// Global exception handler ProblemDetails
app.UseExceptionHandler(a =>
{
    a.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerPathFeature>();
        var problem = Results.Problem(
            title: "An unexpected error occurred.",
            detail: feature?.Error.Message,
            statusCode: StatusCodes.Status500InternalServerError);
        await problem.ExecuteAsync(context);
    });
});

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskBoard API v1");
    c.DisplayRequestDuration();
});

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/healthz");

// ---- Apply migrations automatically in Dev/Prod on startup ----
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    db.Database.Migrate();
}

app.Run();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//var summaries = new[]
//{
//    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
//};

//app.MapGet("/weatherforecast", () =>
//{
//    var forecast = Enumerable.Range(1, 5).Select(index =>
//        new WeatherForecast
//        (
//            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
//            Random.Shared.Next(-20, 55),
//            summaries[Random.Shared.Next(summaries.Length)]
//        ))
//        .ToArray();
//    return forecast;
//})
//.WithName("GetWeatherForecast")
//.WithOpenApi();

//app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
