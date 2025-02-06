using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using StackOverflowTagsApi.Exceptions;
using StackOverflowTagsApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.EnableAnnotations();
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StackOverflowTags API",
        Version = "v1",
        Description = "An example ASP.NET Core Web API to fetch StackOverflow tags",
    });
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddSingleton(sp =>
    ConnectionMultiplexer.Connect(
        sp.GetRequiredService<IConfiguration>().GetConnectionString("Redis")!, 
        x => x.AllowAdmin = true));

builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<ITagsService, TagsService>();

builder.Services.AddControllers();

builder.Services.AddExceptionHandler<CustomExceptionHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My Web API v1");
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.UseExceptionHandler(options => { });

using (var scope = app.Services.CreateScope())
{
    var tagsService = scope.ServiceProvider.GetRequiredService<ITagsService>();
    await tagsService.FetchTagsAsync();
}

app.Run();