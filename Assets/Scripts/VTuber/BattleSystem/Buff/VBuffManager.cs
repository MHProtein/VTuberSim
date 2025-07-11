using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VBuffManager
    {
        private readonly List<VBuff> _buffs = new List<VBuff>();
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
            var buffsToRemove = new List<VBuff>();
            foreach (var buff in _buffs)
            {
                if (buff.DecrementDuration())
                {
                    buffsToRemove.Add(buff);
                }
            }
            foreach (var buff in buffsToRemove)
            {
                buff.OnBuffRemoved();
                _buffs.Remove(buff);
                            
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffRemoved, new Dictionary<string, object>
                {
                    { "Id", buff.Id }
                });
            }
        }

        public void AddBuffs(List<VBuffConfiguration> buffs, bool isFromCard, bool shouldPlayTwice)
        {
            if (buffs == null || buffs.Count == 0)
                return;

            foreach (var buff in buffs)
            {
                AddBuff(buff.CreateBuff(), isFromCard, shouldPlayTwice);
            }
        }
        
        public void AddBuff(VBuff buff, bool isFromCard, bool shouldPlayTwice)
        {
            if (buff == null || string.IsNullOrEmpty(buff.GetBuffName()))
                return;

            var existingBuff = _buffs.Find(b => b.GetBuffName() == buff.GetBuffName());
            if (existingBuff != null)
            {
                existingBuff.Stack(buff, isFromCard, shouldPlayTwice);
            }
            else
            {
                _buffs.Add(buff);
                buff.OnBuffAdded(_battle, _idDistributor++);
                VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnBuffAdded, new Dictionary<string, object>
                {
                    { "Buff", buff },
                    {"IsFromCard",  isFromCard},
                    {"ShouldPlayTwice", shouldPlayTwice },
                });
            }
        }
        
        public void Clear()
        {
            _buffs.Clear();
        }

        public List<VBuff> GetAllBuffs()
        {
            return new List<VBuff>(_buffs);
        }
        
        public bool TryGetBuff(string buffId, out VBuff buff)
        {
            buff = _buffs.Find(b => b.GetBuffName() == buffId);
            return buff != null;
        }

        public void ModifyBuff(string name, int value, bool isFromCard, bool shouldApplyTwice = false)
        {
            if(TryGetBuff(name, out var buff))
            {
                buff.AddLayerOrDuration(value, isFromCard, shouldApplyTwice);
                VDebug.Log($"Effect {name} applied {value} to buff {buff.GetBuffName()} with ID {buff.Id}");
                return;
            }
            
            VBattleRootEventCenter.Instance.Raise(VRootEventKey.OnEffectAnimationFinished, new Dictionary<string ,object>()
            {
                
            });
        }
        
    }
}
