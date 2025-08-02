using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Buff
{
    public class VRelicManager
    {
        private readonly List<VRelic> _relics = new List<VRelic>();
        private VBattle _battle;

        public VRelicManager(VBattle battle)
        {
            _battle = battle;
        }

        public void AddRelic(VRelic relic)
        {
            if (relic == null)
                return;

            if (relicAlreadyOwned(relic) && relic._configuration.isUnique)
            {
                VDebug.Log("重复添加唯一遗物: " + relic.GetRelicName());
                return;
            }

            _relics.Add(relic);
            relic.Activate(_battle);

            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRelicAdded, new Dictionary<string, object>
            {
                { "Id", relic.ConfigId },
                { "Name", relic.GetRelicName() }
            });
        }

        public void RemoveRelic(uint relicId)
        {
            var relic = _relics.Find(r => r.ConfigId == relicId);
            if (relic != null)
            {
                relic.OnRelicRemoved();
                _relics.Remove(relic);
            }
        }

        public bool relicAlreadyOwned(VRelic relic)
        {
            return _relics.Any(r => r.ConfigId == relic.ConfigId);
        }

        public List<VRelic> GetAllRelics() => new List<VRelic>(_relics);
    }

}