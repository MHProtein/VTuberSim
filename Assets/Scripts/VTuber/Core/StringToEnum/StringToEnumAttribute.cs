using System;
using UnityEngine;

namespace VTuber.Core.StringToEnum
{
    [AttributeUsage(AttributeTargets.Field)]
    public class StringToEnumAttribute : PropertyAttribute
    {
        public StringToEnumAttribute(string key = "")
        {
            Key = key;
        }

        public string Key { get; private set; }
    }
}