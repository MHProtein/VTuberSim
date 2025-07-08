using System;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;

namespace VTuber.BattleSystem.Buff
{
    public enum BuffTemporalType
    {
        Permanent,
        Persistent,
    }
    public class VBuffConfiguration : VScriptableObject
    {
        [StringToEnum] public string buffName;
        [TextArea] public string description;
        public Sprite icon;
        public BuffTemporalType buffTemporalType;
        [ShowIf("ShouldShowDuration")]public int duration;
        public bool stackable = true;
        [StringToEnum] public string battleAttributeToApplyName;
        
        protected Type buffType;

        public VBuff CreateBuff()
        {
            return (VBuff)Activator.CreateInstance(buffType, this);
        }
     
        
        bool ShouldShowDuration()
        {
            return buffTemporalType == BuffTemporalType.Persistent;
        }
        
    }
    
}