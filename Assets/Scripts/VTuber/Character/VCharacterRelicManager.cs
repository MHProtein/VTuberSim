using System.Collections.Generic;
using System.Linq;
using VTuber.Core.Foundation;
using VTuber.Relic;

namespace VTuber.Character
{
    public class VCharacterRelicManager
    {
        private readonly List<VBattleRelic> _battleRelics;
        private uint _battleRelicIdDistributor;

        public VCharacterRelicManager(VCharacter character)
        {
            _battleRelics = new List<VBattleRelic>();
            RaisingRelicManager = new VRaisingRelicManager(character);
        }

        public VRaisingRelicManager RaisingRelicManager { get; }

        public List<VBattleRelic> GetBattleRelics()
        {
            return _battleRelics.Select(relic => relic.Copy()).ToList();
        }

        public List<VRelic> GetRelics()
        {
            var relics = new List<VRelic>();
            relics.AddRange(RaisingRelicManager.GetRelics());
            relics.AddRange(_battleRelics);
            return relics;
        }

        public void AddRelic(VRelic relic)
        {
            if (relic is VBattleRelic battleRelic)
            {
                if (_battleRelics.Find(relic => relic.ConfigId == battleRelic.ConfigId) != null)
                    return;
                _battleRelics.Add(battleRelic);
                battleRelic.Initialize(_battleRelicIdDistributor++);
                battleRelic.OnRelicAddedInRaising();
            }
            else
            {
                RaisingRelicManager.AddRelic(relic as VRaisingRelic);
            }

            VDebug.Log("Added Relic " + relic.GetRelicName());
        }

        public void RemoveRelic(VRelic relic)
        {
            if (relic is VBattleRelic battleRelic)
            {
                if (_battleRelics.Contains(battleRelic))
                    return;
                _battleRelics.Remove(battleRelic);
                battleRelic.OnRelicRemovedInRaising();
            }
            else
            {
                RaisingRelicManager.Remove(relic as VRaisingRelic);
            }
        }

        public void Clear()
        {
            foreach (var battleRelic in _battleRelics) battleRelic.OnRelicRemovedInRaising();

            _battleRelics.Clear();
            RaisingRelicManager.Clear();
        }

        public void AddRelics(List<VRelic> relics)
        {
            foreach (var relic in relics)
            {
                AddRelic(relic);
            }
        }
    }
}