using System.Collections.Generic;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;

namespace VTuber.BattleSystem.Buff
{
    public class VBuffManager
    {
        private readonly List<VBuff> _buffs = new List<VBuff>();
        private VBattle _battle;
        private int idDistributor = 0;

        public VBuffManager(VBattle battle)
        {
            _battle = battle;
        }

        public void OnEnable()
        {
            VRootEventCenter.Instance.RegisterListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
        }
        
        public void OnDisable()
        {
            VRootEventCenter.Instance.RemoveListener(VRootEventKey.OnTurnEnd, OnTurnEnd);
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
                            
                VRootEventCenter.Instance.Raise(VRootEventKey.OnBuffRemoved, new Dictionary<string, object>
                {
                    { "Id", buff.Id }
                });
            }
        }

        public void AddBuffs(List<VBuffConfiguration> buffs)
        {
            if (buffs == null || buffs.Count == 0)
                return;

            foreach (var buff in buffs)
            {
                AddBuff(buff.CreateBuff());
            }
        }
        
        public void AddBuff(VBuff buff)
        {
            if (buff == null || string.IsNullOrEmpty(buff.GetBuffName()))
                return;

            var existingBuff = _buffs.Find(b => b.GetBuffName() == buff.GetBuffName());
            if (existingBuff != null)
            {
                existingBuff.Stack(buff);
            }
            else
            {
                _buffs.Add(buff);
                buff.OnBuffAdded(_battle, idDistributor++);
                VRootEventCenter.Instance.Raise(VRootEventKey.OnBuffAdded, new Dictionary<string, object>
                {
                    { "Buff", buff }
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
        
    }
}
