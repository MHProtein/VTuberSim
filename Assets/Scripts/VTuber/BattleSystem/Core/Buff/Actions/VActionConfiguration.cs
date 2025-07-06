using System;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.TypeSerialization;

namespace VTuber.BattleSystem.Core.Buff.Actions
{
    public class VActionConfiguration : VScriptableObject
    {
        [TypeFilter(typeof(VAction))] SerializableType actionType;
        
        public string description;
        
        [Tooltip("duration==-1 means infinite, duration==0 means apply buff duration")]
        [Range(-1, 100)]public int duration;

        public VAction CreateAction()
        {
            return (VAction)Activator.CreateInstance(actionType.GetType(), this);
        }
        
    }
}