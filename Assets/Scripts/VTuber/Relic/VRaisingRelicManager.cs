using System.Collections.Generic;
using VTuber.Character;

namespace VTuber.Relic
{
    public class VRaisingRelicManager
    {
        public uint idDistributor;

        public VRaisingRelicManager(VCharacter character)
        {
            Character = character;
        }

        public List<VRaisingRelic> Relics { get; } = new();

        public VCharacter Character { get; }

        public void AddRelic(VRaisingRelic relic)
        {
            if (relic == null)
                return;

            if (Relics.Find(r => r.ConfigId == relic.ConfigId) != null)
                return;
            Relics.Add(relic);
            relic.Initialize(idDistributor++, this);
            relic.OnRelicAddedInRaising();
        }

        public void Remove(VRaisingRelic relic)
        {
            if (Relics.Contains(relic))
            {
                relic.OnRelicRemovedInRaising();
                Relics.Remove(relic);
            }
        }

        public void Clear()
        {
            foreach (var relic in Relics) relic.OnRelicRemovedInRaising();
            Relics.Clear();
        }

        public List<VRaisingRelic> GetRelics()
        {
            return Relics;
        }
    }
}