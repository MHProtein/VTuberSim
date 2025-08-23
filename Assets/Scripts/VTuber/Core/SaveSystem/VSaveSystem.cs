using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace VTuber.BattleSystem.Core.SaveSystem
{
    public static class VSaveSystem
    {
        static string _savePath = $"{Application.persistentDataPath}/plaer.vtuber";
        
        public static void Save(VSave save)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(_savePath, FileMode.Create);
            
            formatter.Serialize(stream, save);
            stream.Close();
        }
        
        public static VSave Load()
        {
            if (File.Exists(_savePath))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream stream = new FileStream(_savePath, FileMode.Open);
                
                VSave save = formatter.Deserialize(stream) as VSave;
                stream.Close();
                
                return save;
            }
            else
            {
                return null;
            }
        }
    }
}