using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Systems.Persistence
{
    public class FileDataService : IDataService
    {
        ISerializer serializer;
        string dataPath;
        string fileExtension;
        string filename;

        public FileDataService(ISerializer serializer)
        {
            dataPath = Application.persistentDataPath;
            fileExtension = "json";
            filename = "GameData";
            this.serializer = serializer;
        }

        string GetPathToFile(string fileName)
        {
            return Path.Combine(dataPath, string.Concat(fileName, ".", fileExtension));
        }

        public void Save(GameData data, bool overwrite = true)
        {
            string fileLocation = GetPathToFile(filename);

            if (!overwrite && File.Exists(fileLocation))
            {
                throw new IOException($"The file '{filename}.{fileExtension}' already exists and cannot be overwritten.");
            }

            File.WriteAllText(fileLocation, serializer.Serialize(data));
        }

        public GameData Load()
        {
            string fileLocation = GetPathToFile(filename);

            if (!File.Exists(fileLocation))
            {
                // throw new ArgumentException($"No persisted GameData with name '{filename}'");
                return null;
            }

            return serializer.Deserialize<GameData>(File.ReadAllText(fileLocation));
        }

        public void Delete()
        {
            string fileLocation = GetPathToFile(filename);

            if (File.Exists(fileLocation))
            {
                File.Delete(fileLocation);
            }
        }

        // public void DeleteAll()
        // {
        //     foreach (string filePath in Directory.GetFiles(dataPath))
        //     {
        //         File.Delete(filePath);
        //     }
        // }

        // public IEnumerable<string> ListSaves()
        // {
        //     foreach (string path in Directory.EnumerateFiles(dataPath))
        //     {
        //         if (Path.GetExtension(path) == fileExtension)
        //         {
        //             yield return Path.GetFileNameWithoutExtension(path);
        //         }
        //     }
        // }
    }
}