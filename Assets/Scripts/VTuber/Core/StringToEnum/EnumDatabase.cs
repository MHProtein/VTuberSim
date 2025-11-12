using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VTuber.Core.StringToEnum
{
    public class EnumDatabase : SerializedScriptableObject
    {
        public Dictionary<string, List<string>> enumData;
        public static EnumDatabase Instance => Resources.Load<EnumDatabase>("enum database");

        private void OnEnable()
        {
            if (enumData == null)
                enumData = new Dictionary<string, List<string>>();
        }

        public List<string> GetEnumData(string key)
        {
            if (key == "")
            {
                var allEnums = new List<string>();
                foreach (var enumList in enumData.Values) allEnums.AddRange(enumList);
                return allEnums;
            }

            return enumData[key];
        }
    }
}