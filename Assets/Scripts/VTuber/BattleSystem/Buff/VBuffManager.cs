using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuffItem
    {
        public VBuff buff;

        public int Value => value;
        private int value;
        public uint Id { get; private set; }
        public uint ConfigId => buff.ConfigId;

        private VBattle _battle;
        
        private bool isFirstTurn = true;

        public VBuffItem(VBuff buff, int value)
        {
            this.buff = buff;
            this.value = value;
        }

        public void DecrementLatency()
        {
            buff.latency -= 1;
            if (buff.latency <= 0)
                Activate();
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", -1 },
                { "Latency", buff.latency},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false }
            });
            
            VDebug.Log($"{buff.GetBuffName()} 延迟减少到 {buff.latency}");
        }

        public bool DecrementDuration()
        {
            if (buff.latency > 0)
            {
                DecrementLatency();
                return false;
            }
            
            if (buff.IsPermanent)
                return false;
            
            
            if (isFirstTurn)
            {
                isFirstTurn = false;
                bool shouldSkipDecrement = true;
                foreach (var effect in buff.Effects)
                {
                    if (effect.TriggeredInFirstTurn)
                    {
                        shouldSkipDecrement = false;
                        break;
                    }
                }

                if (shouldSkipDecrement)
                {
                    VDebug.Log("第一次执行Buff " + buff.GetBuffName() + " 的持续时间减少逻辑，跳过。");
                    return false;
                }
            }
            
            value -= 1;
            if (Value <= 0)
                return true;

            VDebug.Log($"{buff.GetBuffName()} 持续时间减少到 {Value}");

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", -1 },
                { "Latency", buff.latency},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false }
            });

            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(value);
            }
            
            return false;
        }

        public virtual bool Stack(int addValue, bool isFromCard, bool shouldPlayTwice)
        {
            value += addValue;
            VDebug.Log(buff.GetBuffName() + " 叠加到 " + Value);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", addValue},
                { "Latency", buff.latency},
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice }
            });
            
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(value);
            }
            
            if(value <= 0)
            {
                VDebug.Log(buff.GetBuffName() + " 数值为零或更低，移除Buff。");
                return true; // Indicates that the buff should be removed
            }

            return false;
        }
        
        public void OnBuffAdded(VBattle battle, uint id)
        {
            Id = id;
            _battle = battle;

            if(buff.latency > 0)
                return;

            Activate();
        }

        public void Activate()
        {
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffAdded(_battle, value);
            }
        }

        public void OnBuffRemoved()
        {
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffRemove();
            }
        }

        public bool ApplyCost(int cost)
        {
            if (cost <= 0 || Value < cost)
                return false;
            
            value -= cost;
            VDebug.Log(buff.GetBuffName() + " 消耗已应用，剩余数值: " + Value);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", cost},
                { "Latency", buff.latency},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false }
            });
            
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(value);
            }
            return Value <= 0;
        }

        public bool TestCost(int cost)
        {
            return Value >= cost;
        }
    }

    
    public class VBuffManager
    {
        private readonly List<VBuffItem> _buffs = new List<VBuffItem>();
        private VBattle _battle;
        private uint _idDistributor = 0;

        public VBuffManager(VBattle battle)
        {
            _battle = battle;
        }

        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
        }
        
        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnEnd, OnTurnEnd);
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
            
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffRemoved, new Dictionary<string, object>
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
                if (existingBuff.Stack(value, isFromCard, shouldPlayTwice))
                {
                    RemoveBuff(existingBuff);
                }
            }
            else
            {
    
                var buffItem = new VBuffItem(buff, value);
                _buffs.Add(buffItem);
                buffItem.OnBuffAdded(_battle, _idDistributor++);
                
                VDebug.Log("Buff已添加: " + buff.GetBuffName() + ", 数值: " + value);
                
                VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffAdded, new Dictionary<string, object>
                {
                    { "Id", buffItem.Id },
                    { "BuffId", buff.ConfigId },
                    { "BuffName", buff.GetBuffName() }, 
                    { "IsPermanent", buff.IsPermanent },
                    { "Latency", buff.latency},
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

        public bool TryGetBuff(uint buffId, out VBuffItem buff)
        {
            buff = _buffs.Find(b => b.ConfigId == buffId);
            return buff != null;
        }

        public void ApplyCost(uint id, int cost)
        {
            if (TryGetBuff(id, out var buffItem))
            {
                if(buffItem.ApplyCost(cost))
                    RemoveBuff(buffItem);
            }
        }

        public bool TestCost(uint id, int cost)
        {
            if (TryGetBuff(id, out var buffItem))
            {
                return buffItem.TestCost(cost);
            }
            return false;
        }
        
    }
}
