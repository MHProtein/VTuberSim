using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.Core.Managers;

namespace VTuber.Relic
{
    public class VBattleRelicManagerSaveData
    {
        public List<VBattleRelicSaveData> relics;
    }

    public class VBattleRelicSaveData
    {
        public uint configID;
        public int layer;
    }

    public class VBattleRelicManager
    {
        public VBattle Battle => _battle;
        private VBattle _battle;

        public uint idDistributor = 0;
        private List<VBattleRelic> relics;

        public VBattleRelicManagerSaveData Save()
        {
            return new VBattleRelicManagerSaveData()
            {
                relics = relics.Select(relic => new VBattleRelicSaveData()
                {
                    configID = relic.ConfigId,
                    layer = relic.Layer
                }).ToList()
            };
        }

        public VBattleRelicManager(VBattle battle, VBattleRelicManagerSaveData saveData)
        {
            _battle = battle;
            relics = new List<VBattleRelic>();
            foreach (var relicSaveData in saveData.relics)
            {
                var relic = VDataManager.Instance.CreateRelicByID(relicSaveData.configID) as VBattleRelic;
                relic.LoadLayer(relicSaveData.layer);
                AddRelic(relic);
            }
        }

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
            relic.OnRelicAddedInBattle();
        }

        public void Remove(VBattleRelic relic)
        {
            relic.OnRelicRemovedInBattle();
            relics.Remove(relic);
        }
    }
}