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
        
        public VBattleAttribute(int value, bool isPercentage)
        {
            Value = value;
            _isPercentage = isPercentage;
        }
        
        public virtual void AddTo(int delta)
        {
            Value += delta;
        }
        
        public virtual void MultiplyWith(int delta)
        {
            Value *= delta;
        }

        public virtual void OnEnable()
        {
            
        }
        
        public virtual void OnDisable()
        {
            
        }
        
    }
}