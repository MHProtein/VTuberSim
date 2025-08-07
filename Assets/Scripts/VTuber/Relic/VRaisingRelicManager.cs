using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.Relic
{
    public class VRaisingRelicManager
    {
        public List<VRaisingRelic> Relics => _relics;
        private readonly List<VRaisingRelic> _relics = new List<VRaisingRelic>();

        public VCharacter Character { get; }

        public VRaisingRelicManager(VCharacter character)
        {
            Character = character;
        }

        public void AddRelic(VRaisingRelic relic)
        {
            if (relic == null)
                return;

            _relics.Add(relic);
        }

        public void Remove(VRaisingRelic relic)
        {
            if (_relics.Contains(relic))
            {
                relic.OnRelicRemoved();
                _relics.Remove(relic);
            }
        }
    }

}