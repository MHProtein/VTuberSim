using System.Collections.Generic;
using VTuber.BattleSystem.Core;

namespace VTuber.Relic
{
    public class VBattleRelicManager
    {
        public VBattle Battle => _battle;
        private VBattle _battle;

        private List<VBattleRelic> relics;

        public VBattleRelicManager(VBattle battle, List<VBattleRelic> initRelics)
        {
            _battle = battle;
            relics = new List<VBattleRelic>();
            relics.AddRange(initRelics);
        }

        public void AddRelic(VBattleRelic relic)
        {
            relics.Add(relic);
            relic.OnRelicAdded();
        }

        public void Remove(VBattleRelic relic)
        {
            relic.OnRelicRemoved();
        }
    }
}