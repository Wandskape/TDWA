using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL004
{
    public interface IRepository : IDisposable
    {
        string BasePath { get; } // полный дирректорий для JSON и фотографий 
        Celebrity[] GetAllCelebrities(); // получить весь список знаменитостей
        Celebrity? GetCelebrityById(int id); // получить знаменитость по Id
        Celebrity[] GetCelebritiesBySurname(string surname); // получить знаменитость по фамилии
        string? GetPhotoPathById(int id); // получить путь для GET-запроса к фотографии 
        int? addCelebrity(Celebrity celebrity); // добавить знаменитость, =Id новой знаменитости
        bool delCelebrityById(int id); // удалить знаменитость по Id, =true успех 
        int? updCelebrityById(int id, Celebrity celebrity); // изменить знаменитость по Id, =Id — новый Id - успех 
        int SaveChanges(); // сохранить изменения в JSON, =количество изменений
    }

    public record Celebrity(int Id, string Firstname, string Surname, string PhotoPath);
}
