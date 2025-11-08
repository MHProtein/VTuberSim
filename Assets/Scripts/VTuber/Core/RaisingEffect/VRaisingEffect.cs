using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;

namespace VTuber.Core.RaisingEffect
{
    public enum VInstigatorType
    {
        Buff,
        Coop,
        Event,
        Relic,
        Account,
        Pressure,
        Consumable
    }
    public abstract class VRaisingEffect
    {
        protected VRaisingEffectConfiguration _configuration;

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

        public virtual void ApplyEffect(VCharacter character, Dictionary<string, object> messagedict, VInstigatorType instigatorType, Sprite icon, Dictionary<string, object> animationParams)
        {
            
        }

        public abstract void Upgrade();

        public abstract void DownGrade();

        public abstract string GetParameter();
    }
}