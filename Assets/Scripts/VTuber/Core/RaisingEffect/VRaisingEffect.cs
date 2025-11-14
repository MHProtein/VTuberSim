using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Core.RaisingEffect
{
    public enum VInstigatorType
    {
        Buff,
        Coop,
        Event,
        Relic,
        Pressure,
        Consumable,
        Dialog,
        Ignore
    }
    
    public abstract class VRaisingEffect
    {
        protected VRaisingEffectConfiguration _configuration;
        protected bool shouldPlayAnimation = true;

        public VRaisingEffect(VRaisingEffectConfiguration configuration)
        {
            _configuration = configuration;
        }

        public uint Id => _configuration.id;
        public string Name => _configuration.effectName;

        public string Description
        {
            get
            {
                var description = _configuration.description;

                return description.Replace("X", GetParameter());
            }
        }
        
        protected virtual void ApplyEffectImplement(VCharacter character, Dictionary<string, object> messagedict)
        {
        }
        
        public virtual void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VAnimationRequest animationRequest)
        {
            if (shouldPlayAnimation && animationRequest is not null)
            {
                animationRequest.effect = this;
                animationRequest.value = GetPreviewValue(character);
                animationRequest.effectApply = () => ApplyEffectImplement(character, messagedict);
            
                VRaisingAnimationSystem.Instance.EnqueueAnimationRequest(animationRequest);
                return;
            }
            ApplyEffectImplement(character, messagedict);
        }

        protected virtual int GetPreviewValue(VCharacter character)
        {
            return 0;
        }

        public abstract void Upgrade();

        public abstract void DownGrade();

        public abstract string GetParameter();
    }
}