using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuffItem
    {
        public VBuff buff;
        public int value;
        public int Id { get; private set; }
        public int ConfigId => buff.ConfigId;

        private VBattle _battle;

        public VBuffItem(VBuff buff, int value)
        {
            this.buff = buff;
            this.value = value;
        }

        public bool DecrementDuration()
        {
            if (buff.IsPermanent)
                return false;

            value -= 1;
            if (value <= 0)
                return true;

            VDebug.Log($"{buff.GetBuffName()} duration decremented to {value}");

            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", value },
                { "Delta", -1 },
                { "IsFromCard", false },
                { "ShouldPlayTwice", false }
            });
            return false;
        }

        public virtual void Stack(int addValue, bool isFromCard, bool shouldPlayTwice)
        {
            value += addValue;

            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", value },
                { "Delta", addValue},
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
        }

        public void OnBuffAdded(VBattle battle, int id)
        {
            Id = id;
            _battle = battle;
            VBattleRootEventCenter.Instance.RegisterListener(buff.WhenToApply, ApplyBuff);
        }

        public void OnBuffRemoved()
        {
            VBattleRootEventCenter.Instance.RemoveListener(buff.WhenToApply, ApplyBuff);
        }

        public void ApplyBuff(Dictionary<string, object> message)
        {
            if (buff.Effects == null || buff.Effects.Count == 0)
                return;

            foreach (var effect in buff.Effects)
            {
                if (effect.CanApply(_battle, message))
                    effect.ApplyEffect(_battle, value);
            }
        }

        public bool ApplyCost(int cost)
        {
            if (cost <= 0 || value < cost)
                return false;
            
            value -= cost;
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", value },
                { "Delta", cost},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false }
            });
  
            return value <= 0;
        }

        public bool TestCost(int cost)
        {
            return value >= cost;
        }
    }

    
    public class VBuffManager
    {
        private readonly List<VBuffItem> _buffs = new List<VBuffItem>();
        private VBattle _battle;
        private int _idDistributor = 0;

        public VBuffManager(VBattle battle)
        {
            _battle = battle;
        }

        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }
        
        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }

        private void OnTurnEnd(Dictionary<string, object> messagedict)
        {
            var buffsToRemove = new List<VBuffItem>();
            foreach (var buff in _buffs)
            {
                if (buff.DecrementDuration())
                {
                    buffsToRemove.Add(buff);
                }
            }
            foreach (var buffItem in buffsToRemove)
            {
                RemoveBuff(buffItem);
            }
        }

        private void RemoveBuff(VBuffItem buffItem)
        {
            buffItem.OnBuffRemoved();
            _buffs.Remove(buffItem);
                            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffRemoved, new Dictionary<string, object>
            {
                { "Id", buffItem.Id }
            });
        }
        
        public void AddBuff(VBuff buff, int value, bool isFromCard, bool shouldPlayTwice)
        {
            if (buff == null || string.IsNullOrEmpty(buff.GetBuffName()))
                return;

            var existingBuff = _buffs.Find(b => b.ConfigId == buff.ConfigId);
            if (existingBuff != null && buff.IsStackable())
            {
                existingBuff.Stack(value, isFromCard, shouldPlayTwice);
            }
            else
            {
                var buffItem = new VBuffItem(buff, value);
                _buffs.Add(buffItem);
                buffItem.OnBuffAdded(_battle, _idDistributor++);
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffAdded, new Dictionary<string, object>
                {
                    { "Id", buffItem.Id },
                    { "BuffId", buff.ConfigId },
                    { "BuffName", buff.GetBuffName() }, 
                    { "IsPermanent", buff.IsPermanent },
                    { "Value", value},
                    { "IsFromCard",  isFromCard},
                    { "ShouldPlayTwice", shouldPlayTwice },
                });
            }
        }
        
        public void Clear()
        {
            _buffs.Clear();
        }

        public List<VBuff> GetAllBuffs()
        {
            return new List<VBuff>(_buffs.Select(buffItem => buffItem.buff));
        }

        public bool TryGetBuff(int buffId, out VBuffItem buff)
        {
            buff = _buffs.Find(b => b.ConfigId == buffId);
            return buff != null;
        }

        public void ApplyCost(int id, int cost)
        {
            if (TryGetBuff(id, out var buffItem))
            {
                if(buffItem.ApplyCost(cost))
                    RemoveBuff(buffItem);
            }
        }

        public bool TestCost(int id, int cost)
        {
            if (TryGetBuff(id, out var buffItem))
            {
                return buffItem.TestCost(cost);
            }
            return false;
        }
        
    }
}
