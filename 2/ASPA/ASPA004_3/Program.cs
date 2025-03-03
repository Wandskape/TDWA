using DAL004;
using Microsoft.AspNetCore.Diagnostics;

internal class Program
{
    private static void Main(string[] args)
    {
        Repository.JSONFileName = "Celebrities.json";
        using (IRepository repository = new Repository("D:\\6sem\\TDWA\\2\\ASPA\\Test_DAL004\\Celebrities"))
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.UseExceptionHandler("/Celebrities/Error");

            app.MapGet("/Celebrities", () => repository.GetAllCelebrities());
            app.MapGet("/Celebrities/{id:int}", (int id) =>
            {
                Celebrity celebrity = repository.GetCelebrityById(id);
                if (celebrity == null) throw new FoundByIdException($"Celebrity Id = {id}");
                return celebrity;
            });

            app.MapPost("/Celebrities", (Celebrity celebrity) =>
            {
                int? id = repository.addCelebrity(celebrity);
                if (id == null) throw new AddCelebrityException("Celebrities error, id == null");
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                return new Celebrity((int)id, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            });

            app.MapDelete("/Celebrities/{id:int}", (int id, HttpContext context) =>
            {
                bool result = repository.delCelebrityById(id);
                if (!result) throw new DeleteCelebrityException("Celebrities error, result == false");
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                context.Response.Headers.Add("Message", $"Celebrity with Id = {id} deleted");
                context.Response.WriteAsync($"Celebrity with Id = {id} deleted");
            });

            app.MapPut("/Celebrities/{id:int}", (int id, Celebrity celebrity, HttpContext context) =>
            {
                int newId = repository.updCelebrityById(id, celebrity).Value;
                if(newId == 0) throw new UpdateCelebrityExeption("Celebrities error, id == null");
                if (repository.SaveChanges() <= 0) throw new SaveException("/Celebrities error, SaveChanges() <= 0");
                context.Response.WriteAsync(celebrity.ToString());
            });

            app.MapFallback((HttpContext ctx) => Results.NotFound(new { error = $"Path {ctx.Request.Path} not supported" }));

            app.Map("/Celebrities/Error", (HttpContext ctx) =>
            {
                Exception? ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
                IResult rc = Results.Problem(detail: "Panic", instance: app.Environment.EnvironmentName, title: "ASPA004", statusCode: 500);

                if (ex != null)
                {
                    if (ex is FoundByIdException) rc = Results.NotFound(ex.Message); // 404
                    if (ex is DirectoryNotFoundException) rc = Results.Problem(title: "ASPA004", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500); // 404
                    if (ex is FileNotFoundException) rc = Results.Problem(title: "ASPA004", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500); // 404
                    if (ex is BadHttpRequestException) rc = Results.BadRequest(ex.Message); // Ошибка в формате запроса
                    if (ex is SaveException) rc = Results.Problem(title: "ASPA004/SaveChanges", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is AddCelebrityException) rc = Results.Problem(title: "ASPA004/AddCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is DeleteCelebrityException) rc = Results.Problem(title: "ASPA004/DelCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                    if (ex is UpdateCelebrityExeption) rc = Results.Problem(title: "ASPA004/UpdCelebrity", detail: ex.Message, instance: app.Environment.EnvironmentName, statusCode: 500);
                }

                return rc;
            });

            app.Run();
        }
    }
}

public class DeleteCelebrityException : Exception { public DeleteCelebrityException(string message) : base($"Delete by Id: {message}") { } };

public class FoundByIdException : Exception { public FoundByIdException(string message) : base($"Found by Id: {message}") { } };

public class SaveException : Exception { public SaveException(string message) : base($"SaveChanges error: {message}") { } };

public class AddCelebrityException : Exception { public AddCelebrityException(string message) : base($"AddCelebrityException error: {message}") { } };

public class UpdateCelebrityExeption : Exception { public UpdateCelebrityExeption(string message) : base($"UpdateCelebrityExeption error: {message}") { } };
