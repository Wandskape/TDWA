using Microsoft.Extensions.FileProviders;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var app = builder.Build();

        var defSett = new DefaultFilesOptions();
        defSett.DefaultFileNames.Add("Neumann.html");

        app.UseDefaultFiles(defSett);
        app.UseStaticFiles();

        var staticFileOpt = new StaticFileOptions();
        staticFileOpt.FileProvider = new PhysicalFileProvider(Path.Combine(builder.Environment.ContentRootPath, "Picture"));
        staticFileOpt.RequestPath = "/jora";

        app.Run();
    }

}