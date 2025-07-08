using System;
using UnityEngine;

namespace VTuber.Core.StringToEnum
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringToEnumAttribute : PropertyAttribute
    {
        public string Key { get; private set; }
        public StringToEnumAttribute(string key = "")
        {
            Key = key;
        }
    }

}