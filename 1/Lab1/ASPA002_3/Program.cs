using Microsoft.AspNetCore.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.AddFilter("Microsoft.AspNetCore.Diagnostics", LogLevel.None); // ������ ���������

        var app = builder.Build();

        app.UseExceptionHandler("/error"); // ���� � ����������� Exception

        app.MapGet("/", () => "Start");

        app.MapGet("/test1", () =>
        {
            throw new Exception("-- Exception Test --"); // ���������������� ����������
            return "test1";
        });

        app.MapGet("/test2", () =>
        {
            int x = 0, y = 0, z = 0;
            z = x / y; // ������� �� 0
            return "test2";
        });

        app.MapGet("/test3", () =>
        {
            int[] x = new int[] { 1, 2, 3 };
            int y = x[3]; // ����� �� ������� �������
            return "test3";
        });

        app.Map("/error", async (ILogger<Program> logger, HttpContext context) =>
        {
            IExceptionHandlerFeature? exobj = context.Features.Get<IExceptionHandlerFeature>(); // ���������� � Exception
            await context.Response.WriteAsync("<h1>Oops!</h1>");
            logger.LogError(exobj?.Error, "ExceptionHandler");
        });

        app.Run();
    }
}
