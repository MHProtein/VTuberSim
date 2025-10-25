using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.Core;
using VTuber.Core.Managers;

namespace VTuber.Relic
{
    public class VBattleRelicManagerSaveData
    {
        public uint idDistributor;
        public List<VBattleRelicSaveData> relics;
    }

    public class VBattleRelicSaveData
    {
        public uint configID;
        public int layer;
    }

    public class VBattleRelicManager
    {
        private readonly List<VBattleRelic> relics;
        public uint idDistributor;

        public VBattleRelicManager(VBattle battle, VBattleRelicManagerSaveData saveData)
        {
            Battle = battle;
            relics = new List<VBattleRelic>();
            idDistributor = saveData.idDistributor;
            foreach (var relicSaveData in saveData.relics)
            {
                var relic = VDataManager.Instance.CreateRelicByID(relicSaveData.configID) as VBattleRelic;
                relic.LoadLayer(relicSaveData.layer);
                AddRelic(relic);
            }
        }

        public VBattleRelicManager(VBattle battle, List<VBattleRelic> initRelics)
        {
            Battle = battle;
            relics = new List<VBattleRelic>();
            foreach (var relic in initRelics) AddRelic(relic);
        }

        public VBattle Battle { get; }

        public VBattleRelicManagerSaveData Save()
        {
            return new VBattleRelicManagerSaveData
            {
                idDistributor = idDistributor,
                relics = relics.Select(relic => new VBattleRelicSaveData
                {
                    configID = relic.ConfigId,
                    layer = relic.Layer
                }).ToList()
            };
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