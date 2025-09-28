using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using UnityEngine;

namespace SlayTheSpire.System.SavingSystem
{
    public class FileDataHandler
    {
        private string _dataDirectoryPath = ""; 
        private string _dataFileName = "";

        public FileDataHandler(string dataDirectoryPath, string dataFileName)
        {
            _dataDirectoryPath = dataDirectoryPath;
            _dataFileName = dataFileName;
        }
        
        public SaveData Load()
        {
            string path = Path.Combine(_dataDirectoryPath, _dataFileName);
            SaveData loadedData = new SaveData();
            
            if (!File.Exists(path))
                return null;
            
            try
            {
                using (FileStream stream = new FileStream(path, FileMode.Open))
                using (BsonDataReader reader = new BsonDataReader(stream))
                {
                    JsonSerializer serializer = new JsonSerializer();
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
            string path = Path.Combine(_dataDirectoryPath, _dataFileName);

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(path));

                using (FileStream stream = new FileStream(path, FileMode.Create))
                using (BsonDataWriter writer = new BsonDataWriter(stream))
                {
                    JsonSerializer serializer = new JsonSerializer();
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
            string path = Path.Combine(_dataDirectoryPath, _dataFileName);
            return File.Exists(path);
        }
    }
}