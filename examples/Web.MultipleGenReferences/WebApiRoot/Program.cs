using BarDomain;
using Core;
using FooDomain;
using Skybeam;
using Skybeam.Abstractions;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();
services.AddOpenApi();

//services.AddTransient<IRequestHandler<FooQuery, FooResponse>, FooQueryHandler>();
//services.AddTransient<IRequestHandler<BarQuery, BarResponse>, BarQueryHandler>();

// has a registry per Skybeam package reference
services.AddSkybeam();

var app = builder.Build();

var foo = app.Services.GetRequiredService<IRequestHandler<FooQuery, FooResponse>>();
var bar = app.Services.GetRequiredService<IRequestHandler<BarQuery, BarResponse>>();
var gamma = app.Services.GetRequiredService<IRequestHandler<CoreQuery, CoreResponse>>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
