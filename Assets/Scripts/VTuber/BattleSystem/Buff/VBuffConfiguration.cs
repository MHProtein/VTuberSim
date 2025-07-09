using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
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
        public Sprite icon;
        public BuffTemporalType buffTemporalType;
        [ShowIf("IsBuffPermanent")] public int layer = -1;
        [ShowIf("IsBuffPersistent")] public int duration = -1;
        [ShowIf("IsBuffPersistent")] public bool stackable = true;
        [StringToEnum] public string battleAttributeToApplyName;

        public VRootEventKey whenToApply;
        
        public List<VEffect> effects;
        
        protected Type buffType;

        public VBuff CreateBuff()
        {
            return (VBuff)Activator.CreateInstance(buffType, this);
        }
     
        
        public bool IsBuffPersistent()
        {
            return buffTemporalType == BuffTemporalType.Persistent;
        }
        
        public bool IsBuffPermanent()
        {
            return buffTemporalType == BuffTemporalType.Permanent;
        }
    }
}