using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/ping", () => "pong");
app.MapPitchEndpoints();

app.Run("http://0.0.0.0:5000");
