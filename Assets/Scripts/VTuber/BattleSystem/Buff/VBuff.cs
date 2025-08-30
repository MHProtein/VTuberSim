using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuff
    {
        private VBuffConfiguration _configuration;
        
        public List<VEffect> Effects => _effects;
        private List<VEffect> _effects;
        
        public uint ConfigId => _configuration.id;
        
        public bool IsPermanent => _configuration.IsBuffPermanent();
        
        public int latency;
        public BuffType BuffType => _configuration.buffType;
        public VBuff(VBuffConfiguration configuration, List<VEffect> effects)
        {
            _configuration = configuration;
            _effects = effects;
            latency = _configuration.latency;
        }

        public string GetDescription(int layer)
        {
            string des = _configuration.description; 
            if(des.Contains("X1"))
                des = des.Replace("X1", _effects[0].GetValue());
            if (des.Contains("X2"))
                des = des.Replace("X2", _effects[1].GetValue());
            if (des.Contains("X3"))
                des = des.Replace("X3", _effects[2].GetValue());
            if (des.Contains("X4"))
                des = des.Replace("X4", _effects[3].GetValue());
            if (des.Contains("X5"))
                des = des.Replace("X5", _effects[3].GetValue());

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
    }
}