using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Effect;

namespace VTuber.BattleSystem.Buff
{
    public class VBuff
    {
        private readonly VBuffConfiguration _configuration;

        public int latency;

        public VBuff(VBuffConfiguration configuration, List<VEffect> effects)
        {
            _configuration = configuration;
            Effects = effects;
            latency = _configuration.latency;
        }

        public List<VEffect> Effects { get; private set; }

        public uint ConfigId => _configuration.id;

        public bool IsPermanent => _configuration.IsBuffPermanent();
        public BuffType BuffType => _configuration.buffType;
        public Sprite Icon => _configuration.icon;

        public string GetDescription(int layer)
        {
            var des = _configuration.description;
            if (des.Contains("X1"))
                des = des.Replace("X1", Effects[0].GetValue());
            if (des.Contains("X2"))
                des = des.Replace("X2", Effects[1].GetValue());
            if (des.Contains("X3"))
                des = des.Replace("X3", Effects[2].GetValue());
            if (des.Contains("X4"))
                des = des.Replace("X4", Effects[3].GetValue());
            if (des.Contains("X5"))
                des = des.Replace("X5", Effects[3].GetValue());

            des = des.Replace("L", layer.ToString());
            des = des.Replace("D", latency.ToString());

            return des;
        }

        public virtual bool IsStackable()
        {
            return _configuration.stackable;
        }

        public string GetBuffName()
        {
            return _configuration.buffName;
        }

        public void RemoveModifierEffects()
        {
            Effects = Effects.Where(effect => effect is not VModifierEffect).ToList();
        }

        public void AddEffect(VEffect effect)
        {
            Effects.Add(effect);
        }
    }
}