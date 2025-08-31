using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.Relic
{
    public class VRaisingRelicManager
    {
        public List<VRaisingRelic> Relics => _relics;
        private readonly List<VRaisingRelic> _relics = new List<VRaisingRelic>();
        public uint idDistributor = 0;

        public VCharacter Character { get; }

        public VRaisingRelicManager(VCharacter character)
        {
            Character = character;
        }

        public void AddRelic(VRaisingRelic relic)
        {
            if (relic == null)
                return;

            if (_relics.Contains(relic))
                return;
            _relics.Add(relic);
            relic.Initialize(idDistributor++, this);
            relic.OnRelicAddedInRaising();
        }

        public void Remove(VRaisingRelic relic)
        {
            if (_relics.Contains(relic))
            {
                relic.OnRelicRemovedInRaising();
                _relics.Remove(relic);
            }
        }

        public void Clear()
        {
            foreach (var relic in _relics)
            {
                relic.OnRelicRemovedInRaising();
            }
            _relics.Clear();
        }
    }
}