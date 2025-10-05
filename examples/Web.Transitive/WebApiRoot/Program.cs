using Core;
using FooDomain;
using Skybeam;
using Skybeam.Abstractions;
using WebApiRoot;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllers();
services.AddOpenApi();

//services.AddTransient<IRequestHandler<FooQuery, FooResponse>, FooQueryHandler>();
//services.AddTransient<IRequestHandler<BarQuery, BarResponse>, BarQueryHandler>();

services.AddSkybeam()
    .AddBehavior(typeof(WebBehavior<,>));

var app = builder.Build();

var gamma = app.Services.GetRequiredService<IRequestHandler<CoreQuery, CoreResponse>>();
var foo = app.Services.GetRequiredService<IRequestHandler<FooQuery, FooResponse>>();
var quux = app.Services.GetRequiredService<IRequestHandler<WebQuery, WebResponse>>();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
