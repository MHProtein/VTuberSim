using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;

namespace VTuber.BattleSystem.Buff
{
    public class VBuffSaveData
    {
        public uint configID;
        public int layer;
        public bool isFirstTurn;
        public List<VModifierEffectSaveData> modifierEffectSaveDatas;
    }

    public class VBuffItem
    {
        public VBuff buff;

        public int Value => _value;
        private int _value;
        public uint Id { get; private set; }
        public uint ConfigId => buff.ConfigId;

        private VBattle _battle;

        private bool _isFirstTurn = true;

        public VBuffItem(VBuff buff, int value)
        {
            this.buff = buff;
            this._value = value;
        }
        
        public VBuffItem(VBuffSaveData saveData)
        {
            _value = saveData.layer;
            _isFirstTurn = saveData.isFirstTurn;
            buff = VDataManager.Instance.CreateBuffByID(saveData.configID);
            buff.RemoveModifierEffects();
            foreach (var modifierEffectSaveData in saveData.modifierEffectSaveDatas)
            {
                var effect = VDataManager.Instance.CreateEffectByID(modifierEffectSaveData.modifierID, "", "");
                (effect as VModifierEffect).Load(modifierEffectSaveData);
                buff.AddEffect(effect);
            }
        }

        public VBuffSaveData Save()
        {
            List<VModifierEffectSaveData> modifierEffectSaveDatas = new List<VModifierEffectSaveData>();
            foreach (var effect in buff.Effects)
            {
                if (effect is VModifierEffect modifierEffect)
                {
                    modifierEffectSaveDatas.Add(modifierEffect.Save());
                }
            }

            return new VBuffSaveData()
            {
                configID = ConfigId,
                layer = _value,
                isFirstTurn = _isFirstTurn,
                modifierEffectSaveDatas = modifierEffectSaveDatas
            };
        }

    // 每回合减少延迟计数
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
                { "ShouldPlayTwice", false },
                { "Buff", this }
            });
            
            VDebug.Log($"{buff.GetBuffName()} 延迟减少到 {buff.latency}");
        }
        
        // 减少持续时间，返回 true 表示应该移除该 Buff
        public bool DecrementDuration()
        {
            if (buff.latency > 0)
            {
                DecrementLatency();
                return false;
            }
            
            if (buff.IsPermanent)
                return false;
            
            if (_isFirstTurn)
            {
                _isFirstTurn = false;
                // 如果该Buff没有在第一回合生效的效果，则跳过首次递减
                bool shouldSkipDecrement = true;
                foreach (var effect in buff.Effects)
                {
                    if (effect.Triggered)
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
            
            _value -= 1;
            if (Value <= 0)
                return true;

            VDebug.Log($"{buff.GetBuffName()} 持续时间减少到 {Value}");

            // 广播数值变化事件
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", -1 },
                { "Latency", buff.latency},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false },
                { "Buff", this }
            });
            
            // 通知效果层数变化
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(_value);
            }
            
            return false;
        }

        // 叠加Buff层数，返回 true 表示需要移除
        public virtual bool Stack(int addValue, bool isFromCard, bool shouldPlayTwice)
        {
            _value += addValue;
            VDebug.Log(buff.GetBuffName() + " 叠加到 " + Value);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", addValue},
                { "Latency", buff.latency},
                { "IsFromCard", isFromCard },
                { "ShouldPlayTwice", shouldPlayTwice },
                { "Buff", this }
            });
            
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(_value);
            }
            
            if(_value <= 0)
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
                effect.OnBuffAdded(_battle, _value);
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
            
            _value -= cost;
            VDebug.Log(buff.GetBuffName() + " 消耗已应用，剩余数值: " + Value);
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnBuffValueUpdated, new Dictionary<string, object>
            {
                { "Id", Id },
                { "BuffId", buff.ConfigId},
                { "Value", Value },
                { "Delta", cost},
                { "Latency", buff.latency},
                { "IsFromCard", false },
                { "ShouldPlayTwice", false },
                { "Buff", this }
            });
            
            foreach (var effect in buff.Effects)
            {
                effect.OnBuffLayerChange(_value);
            }
            return Value <= 0;
        }

        public bool TestCost(int cost)
        {
            return Value >= cost;
        }
    }

    public class VBuffLayerModifier
    {
        public uint buffId;
        Dictionary<uint, float> modifiers = new Dictionary<uint, float>();
        private uint _idDistributor = 0;
        
        public uint AddModifier(float modifier)
        {
            _idDistributor++;
            modifiers.Add(_idDistributor, modifier);
            return _idDistributor;
        }
        
        public void RemoveModifier(uint id)
        {
            modifiers.Remove(id);
        }
        
        public void ChangeModifier(uint id, float newValue)
        {
            modifiers[id] = newValue;
        }
        
        public float GetModifier()
        {
            return modifiers.Values.Sum();
        }
    }
    
    public class VBuffLayerModifierManager
    {
        public Dictionary<uint, VBuffLayerModifier> modifiers = new Dictionary<uint, VBuffLayerModifier>();
        
        public uint AddModifier(uint buffId, float modifier)
        {
            if (!modifiers.ContainsKey(buffId))
            {
                modifiers.Add(buffId, new VBuffLayerModifier());
            }
            return modifiers[buffId].AddModifier(modifier);
        }
        
        public void RemoveModifier(uint buffId, uint id)
        {
            if (modifiers.ContainsKey(buffId))
            {
                modifiers[buffId].RemoveModifier(id);
            }
        }
        
        public void ChangeModifier(uint buffId, uint id, float newValue)
        {
            if (modifiers.ContainsKey(buffId))
            {
                modifiers[buffId].ChangeModifier(id, newValue);
            }
        }
        
        public float GetModifier(uint buffId)
        {
            if (modifiers.ContainsKey(buffId))
            {
                return modifiers[buffId].GetModifier();
            }
            return 0;
        }
    }
}