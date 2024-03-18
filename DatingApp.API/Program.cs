using DatingApp.API.Data;
using DatingApp.API.Extenstions;
using DatingApp.API.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
{
    // Add services to the container.
    builder.Services.AddControllers();
    builder.Services.AddApplicationServices(builder.Configuration);
    builder.Services.AddIdentityService(builder.Configuration);
}


// MIDDLEWARES
var app = builder.Build();
{
    app.UseMiddleware<ExceptionMiddleware>();
    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();

    app.UseCors(builder => builder.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200"));

    app.UseAuthentication();

    app.UseAuthorization();

    app.MapControllers();


    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<DataContext>();
        await context.Database.MigrateAsync();
        await Seed.SeedUsers(context);
    }
    catch(Exception ex) 
    {
        var logger = services.GetService<ILogger<Program>>();
        logger.LogError(ex, "An error occured during migration");
    }

    app.Run();
}


