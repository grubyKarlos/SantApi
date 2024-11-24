using SantApi.Middleware;
using SantApi.Services;
using SantApi.Settings;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.Configure<HackerNewsSettings>(builder.Configuration.GetSection("HackerNewsSettings"));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<IStoryService, StoryService>();
builder.Services.AddScoped<IHackerNewsService, HackerNewsService>();
builder.Services.AddScoped<ICachedHackerNewsService, CachedHackerNewsService>();

builder.Services.AddHttpClient(AppConstants.HttpClientName, client =>
{
    var hackerNewsSettings = builder.Configuration.GetSection("HackerNewsSettings").Get<HackerNewsSettings>();
    client.BaseAddress = new Uri(hackerNewsSettings.BaseUrl);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }