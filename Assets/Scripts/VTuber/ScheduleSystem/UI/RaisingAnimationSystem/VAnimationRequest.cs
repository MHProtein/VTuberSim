using System;
using UnityEngine;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.UI.RaisingAnimationSystem
{
    public class VAnimationRequest
    {
        public VInstigatorType instigatorType;
        public Sprite instigatorIcon;
        public Sprite attributeIcon;
        public string description;
        public int value;
        public VRaisingEffect effect;
        public VAnimationType animationType;
        public Action effectApply;
    }

    public class VAnimationRequestFactory
    {
        public static VAnimationRequest Create(VInstigatorType instigatorType, Sprite icon, string description)
        {
            return new VAnimationRequest
            {
                instigatorType = instigatorType,
                instigatorIcon = icon,
                description = description,
            };
        }
    }
}