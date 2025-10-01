using Skybeam;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseAuthorization();

app.MapControllers();

app.Run();

//HandlerDescription x = new HandlerDescription();

//SkybeamFluentBuilder xx = new SkybeamFluentBuilder(null);
