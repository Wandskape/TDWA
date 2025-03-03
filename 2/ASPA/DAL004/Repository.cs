using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DAL004
{
    public class Repository : IRepository
    {
        public string BasePath { get; }
        public static string JSONFileName { get; set; } = "/Celebrities/Celebrities.json";
        private Celebrity[] _celebrities;
        private Celebrity[] _originalCelebrities;
        private int _currentCelebritiesID;

        public Repository(string basePath)
        {
            BasePath = basePath;
            var jsonFilePath = Path.Combine(BasePath, JSONFileName);
            if (File.Exists(jsonFilePath))
            {
                var jsonData = File.ReadAllText(jsonFilePath);
                _celebrities = JsonConvert.DeserializeObject<Celebrity[]>(jsonData);
                _originalCelebrities = (Celebrity[])_celebrities.Clone();
                _currentCelebritiesID = _celebrities.Length > 0 ? _celebrities.Max(c => c.Id) + 1 : 1;
            }
            else
            {
                _celebrities = Array.Empty<Celebrity>();
                _originalCelebrities = Array.Empty<Celebrity>();
            }
        }

        public Celebrity[] GetAllCelebrities() => _celebrities;

        public Celebrity? GetCelebrityById(int id) => _celebrities.FirstOrDefault(c => c.Id == id);

        public Celebrity[] GetCelebritiesBySurname(string surname) => _celebrities.Where(c => c.Surname.Equals(surname, StringComparison.OrdinalIgnoreCase)).ToArray();

        public string? GetPhotoPathById(int id) => _celebrities.FirstOrDefault(c => c.Id == id)?.PhotoPath;

        public int? addCelebrity(Celebrity celebrity)
        {
            var newId = _celebrities.Length > 0 ? _celebrities.Max(c => c.Id) + 1 : 1;
            var newCelebrity = new Celebrity(newId, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);

            Celebrity[] newArray = new Celebrity[_celebrities.Length + 1];

            for (int i = 0; i < _celebrities.Length; i++)
            {
                newArray[i] = _celebrities[i];
            }

            newArray[_celebrities.Length] = newCelebrity;

            _celebrities = newArray;
            _currentCelebritiesID++;

            return newCelebrity.Id;
        }


        public bool delCelebrityById(int id)
        {
            int index = Array.FindIndex(_celebrities, c => c.Id == id);
            if (index == -1)
            {
                return false;
            }

            Celebrity[] newArray = new Celebrity[_celebrities.Length - 1];
            for (int i = 0, j = 0; i < _celebrities.Length; i++)
            {
                if (i != index)
                {
                    newArray[j++] = _celebrities[i];
                }
            }

            _celebrities = newArray;
            return true;
        }

        public int? updCelebrityById(int id, Celebrity celebrity)
        {
            var newId = _currentCelebritiesID;
            _currentCelebritiesID++;
            var newCelebrity = new Celebrity(newId, celebrity.Firstname, celebrity.Surname, celebrity.PhotoPath);
            Celebrity[] newArray = new Celebrity[_celebrities.Length];

            for (int i = 0; i < _celebrities.Length; i++)
            {
                if (_celebrities[i].Id == id)
                {
                    newArray[i] = newCelebrity; 
                }
                else
                {
                    newArray[i] = _celebrities[i];
                }
            }
            _celebrities = newArray;
            return newCelebrity.Id;
        }

        public int SaveChanges()
        {
            int changesCount = 0;

            var jsonFilePath = Path.Combine(BasePath, JSONFileName);
            var newCelebritiesJson = JsonConvert.SerializeObject(_celebrities, Formatting.Indented);
            File.WriteAllText(jsonFilePath, newCelebritiesJson);

            for (int i = 0; i < _celebrities.Length; i++)
            {
                if (i >= _originalCelebrities.Length || !_celebrities[i].Equals(_originalCelebrities[i]))
                {
                    changesCount++;
                }
            }

            _originalCelebrities = (Celebrity[])_celebrities.Clone();

            return changesCount;
        }


        public static IRepository Create(string basePath)
        {
            return new Repository(basePath);
        }

        public void Dispose() { }
    }
}
