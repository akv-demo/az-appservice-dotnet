var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");
app.MapGroup("/1")
    .MapGet("/ping", () => "pong");

app.Run();

