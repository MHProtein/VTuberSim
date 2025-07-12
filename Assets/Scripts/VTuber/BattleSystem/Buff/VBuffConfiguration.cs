using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.StringToEnum;
using VTuber.Core.TypeSerialization;

namespace VTuber.BattleSystem.Buff
{
    public enum BuffType
    {
        Permanent,
        Persistent,
    }
    public class VBuffConfiguration : VScriptableObject
    {
        public int buffId;
        public string buffName;
        public Sprite icon;
        [FormerlySerializedAs("buffTemporalType")] public BuffType buffType;
        public bool stackable = true;

        public VRootEventKey whenToApply;
        
        public List<VEffectConfiguration> effects;

        public VBuff CreateBuff()
        {
            return new VBuff(this);
        }
        
        public bool IsBuffPersistent()
        {
            return buffType == BuffType.Persistent;
        }
        
        public bool IsBuffPermanent()
        {
            return buffType == BuffType.Permanent;
        }
    }
}