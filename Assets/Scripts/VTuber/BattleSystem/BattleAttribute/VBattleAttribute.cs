using System.Collections.Generic;
using Sirenix.Utilities;
using VTuber.BattleSystem.Buff;

namespace VTuber.BattleSystem.BattleAttribute
{
    
    //All the attributes treated as int type, if is percentage, it is multiplied by 100 and vice versa when used. 
    public class VBattleAttribute
    {
        public int Value { get; protected set; }
        private bool _isPercentage;
        
        private List<VBuff> buffs;
        
        public VBattleAttribute(int value, bool isPercentage)
        {
            buffs = new List<VBuff>();
            Value = value;
            _isPercentage = isPercentage;
        }

        public void AddBuff(VBuff buff)
        {
            foreach (var vBuff in buffs)
            {
                if (buff.IsStackable(vBuff))
                {
                    vBuff.Stack(buff);
                    return;
                }
            }

            buffs.Add(buff);
            
            Value = buff.ApplyBuff(Value);
        }
        
        public virtual void ApplyBuffs()
        {
            foreach (var buff in buffs)
            {
                Value = buff.ApplyBuff(Value);
            }
        }

        
    }
}