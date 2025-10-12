using System.Collections.Generic;
using System.Linq;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Relic;


namespace VTuber.Character
{
    public class VCharacterRelicManager
    {
        public VRaisingRelicManager RaisingRelicManager => _raisingRelicManager;
        private readonly VRaisingRelicManager _raisingRelicManager;

        private uint _battleRelicIdDistributor = 0;
        private List<VBattleRelic> _battleRelics;

        public VCharacterRelicManager(VCharacter character)
        {
            _battleRelics = new List<VBattleRelic>();
            _raisingRelicManager = new VRaisingRelicManager(character);
        }

        public List<VBattleRelic> GetBattleRelics()
        {
            return _battleRelics;
        }

        public List<VRelic> GetRelics()
        {
            var relics = new List<VRelic>();
            relics.AddRange(_raisingRelicManager.GetRelics());
            relics.AddRange(_battleRelics);
            return relics;
        }

        public void AddRelic(VRelic relic)
        {
            if (relic is VBattleRelic battleRelic)
            {
                if (_battleRelics.Contains(battleRelic))
                    return;
                _battleRelics.Add(battleRelic);
                battleRelic.Initialize(_battleRelicIdDistributor++);
                battleRelic.OnRelicAddedInRaising();
            }
            else
            {
                _raisingRelicManager.AddRelic(relic as VRaisingRelic);
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
                _raisingRelicManager.Remove(relic as VRaisingRelic);
            }
        }

        public void Clear()
        {
            foreach (var battleRelic in _battleRelics)
            {
                battleRelic.OnRelicRemovedInRaising();
            }

            _battleRelics.Clear();
            _raisingRelicManager.Clear();
        }
    }
}