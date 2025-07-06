using System;
using UnityEngine;

namespace VTuber.Core.TypeSerialization
{
    [Serializable]
    public class SerializableType : ISerializationCallbackReceiver
    {
        [SerializeField] private string assemblyQualifiedName = string.Empty;
        
        public Type TypeToSerialize { get; private set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            assemblyQualifiedName = TypeToSerialize?.AssemblyQualifiedName ?? assemblyQualifiedName;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!TryGetType(assemblyQualifiedName, out var type))
            {
                Debug.LogError($"Type {assemblyQualifiedName} not found");
                return;
            }

            TypeToSerialize = type;
        }

        static bool TryGetType(string typeString, out Type type)
        {
            type = Type.GetType(typeString);
            return type != null || !string.IsNullOrEmpty(typeString);
        }

        public static implicit operator Type(SerializableType sType) => sType.TypeToSerialize;

        public static implicit operator SerializableType(Type type) => new() { TypeToSerialize = type };

    }
}