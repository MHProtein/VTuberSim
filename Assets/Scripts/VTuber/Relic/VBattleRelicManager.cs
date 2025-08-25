using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.Relic
{
    public class VBattleRelicManager
    {
        public VBattle Battle => _battle;
        private VBattle _battle;

        public uint idDistributor = 0;
        private List<VBattleRelic> relics;

        public VBattleRelicManager(VBattle battle, List<VBattleRelic> initRelics)
        {
            _battle = battle;
            relics = new List<VBattleRelic>();
            foreach (var relic in initRelics)
            {
                AddRelic(relic);
            }
        }

        public void AddRelic(VBattleRelic relic)
        {
            if (relic == null)
                return;

            relics.Add(relic);
            relic.Initialize(idDistributor++, this);
            relic.OnRelicAddedInRaising();
        }

        public void Remove(VBattleRelic relic)
        {
            relic.OnRelicRemovedInRaising();
            relics.Remove(relic);
        }
    }
}