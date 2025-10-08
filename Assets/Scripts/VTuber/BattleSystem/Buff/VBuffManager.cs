using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{


    
    
    public class VBuffManager
    {
        private readonly List<VBuffItem> _buffs = new List<VBuffItem>();
        private VBattle _battle;
        private uint _idDistributor = 0;
        private VBuffLayerModifierManager _buffLayerModifierManager = new VBuffLayerModifierManager();

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
                { "Id", buffItem.Id },
                { "Buff", this }
            });
        }
        
        public void AddBuff(VBuff buff, int value, bool isFromCard, bool shouldPlayTwice)
        {
            if (buff == null || string.IsNullOrEmpty(buff.GetBuffName()))
                return;
            
            var existingBuff = _buffs.Find(b => b.ConfigId == buff.ConfigId);
            if (existingBuff != null && buff.IsStackable())
            {
                if (existingBuff.Stack((int)(value * (1.0f + _buffLayerModifierManager.GetModifier(buff.ConfigId))),
                        isFromCard, shouldPlayTwice))
                {
                    RemoveBuff(existingBuff);
                }
            }
            else
            {
                if (value <= 0)
                    return;
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
                    { "Buff", buffItem }
                });
            }
        }
        
        public void Clear()
        {
            foreach (var buff in _buffs)
            {
                buff.OnBuffRemoved();
            }
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
