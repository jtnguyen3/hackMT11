using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using HackMT2026.Endpoints;

namespace HackMT2026
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            // Basic test endpoint
            app.MapGet("/ping", () => "pong");

            // Map your endpoints
            app.MapPitchEndpoints();
            app.MapGameEndpoints();

            // Run the app
            app.Run("http://0.0.0.0:5000");
        }
    }
}
