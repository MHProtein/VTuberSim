using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using VTuber.Core.Foundation;

namespace VTuber.Core.StringToEnum
{
    public class EnumDatabase :  SerializedScriptableObject
    {
        public static EnumDatabase Instance => Resources.Load<EnumDatabase>("enum database");

        public Dictionary<string, List<string>> enumData;
        
        private void OnEnable() {
            if (enumData == null)
                enumData = new Dictionary<string, List<string>>();
        }
        
        public List<string> GetEnumData(string key)
        {
            if (key == "")
            {
                var allEnums = new List<string>();
                foreach (var enumList in enumData.Values)
                {
                    allEnums.AddRange(enumList);
                }
                return allEnums;
            }

            return enumData[key];
        }
        
    }
}

