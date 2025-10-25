using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine;
using VTuber.Core.Foundation;

namespace SlayTheSpire.System.SavingSystem
{
    public class FileDataHandler
    {
        private readonly string _dataDirectoryPath = "";
        private readonly string _dataFileName = "";

        public FileDataHandler(string dataDirectoryPath, string dataFileName)
        {
            _dataDirectoryPath = dataDirectoryPath;
            _dataFileName = dataFileName;
        }

        public SaveData Load()
        {
            var path = Path.Combine(_dataDirectoryPath, _dataFileName);
            var loadedData = new SaveData();

            if (!File.Exists(path))
                return null;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open))
                using (var reader = new BsonDataReader(stream))
                {
                    var serializer = new JsonSerializer();
                    loadedData = serializer.Deserialize<SaveData>(reader);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to Load : " + e.Message);
            }

            return loadedData;
        }

        public void Save(SaveData data)
        {
            var path = Path.Combine(_dataDirectoryPath, _dataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (var stream = new FileStream(path, FileMode.Create))
                using (var writer = new BsonDataWriter(stream))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(writer, data);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Failed to Save : " + e.Message);
            }
        }

        public bool SaveExists()
        {
            var path = Path.Combine(_dataDirectoryPath, _dataFileName);
            return File.Exists(path);
        }

        public void Delete()
        {
            var path = Path.Combine(_dataDirectoryPath, _dataFileName);
            try
            {
                File.Delete(path);
            }
            catch (Exception e)
            {
                VDebug.LogError(e.Message);
            }
        }
    }
}