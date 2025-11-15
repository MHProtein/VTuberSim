using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Effect;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.RaisingEffect;
using VTuber.RaisingAnimationSystem;
using VTuber.ScheduleSystem.UI.RaisingAnimationSystem;

namespace VTuber.Consumable
{
    public class VConsumableManager
    {
        private readonly VCharacter _character;
        private readonly List<VConsumable> consumables = new();
        private VBattle _battle;
        private bool _canUseConsumable;

        public uint maxConsumableCount = 3;

        public VConsumableManager(VCharacter character)
        {
            _character = character;
        }

        public bool CanUseConsumable => _canUseConsumable || CanUseBattleConsumable();

        public bool CanAddConsumable()
        {
            return consumables.Count < maxConsumableCount;
        }

        public void AddConsumable(VConsumable consumable)
        {
            consumables.Add(consumable);
            consumable.Initialize(this);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnAddConsumable, new Dictionary<string, object>
            {
                { "Consumable", consumable },
                { "AreSlotsFull", !CanAddConsumable() }
            });
        }

        public void RemoveConsumable(VConsumable consumable)
        {
            consumables.Remove(consumable);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRemoveConsumable, new Dictionary<string, object>
            {
                { "Consumable", consumable },
                { "AreSlotsFull", !CanAddConsumable() }
            });
        }

        public void SetBattle(VBattle battle)
        {
            _battle = battle;
        }

        public void ClearBattle()
        {
            _battle = null;
        }

        public bool CanUseBattleConsumable()
        {
            return _battle is not null;
        }

        public void SetCanUseConsumable(bool canUseConsumable)
        {
            _canUseConsumable = canUseConsumable;
        }

        public void ApplyBattleEffects(List<VEffect> effects)
        {
            effects.ForEach(effect => effect.ApplyEffect(_battle));
        }

        public void ApplyRaisingEffects(List<VRaisingEffect> effects, Sprite icon, string description)
        {
            effects.ForEach(effect => effect.ApplyEffect(_character, null, VAnimationRequestFactory.Create(VInstigatorType.Consumable, icon, description)));
        }

        public void Remove(VConsumable consumable)
        {
            RemoveConsumable(consumable);
        }

        public void Clear()
        {
            foreach (var consumable in consumables)
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnRemoveConsumable,
                    new Dictionary<string, object>
                    {
                        { "Consumable", consumable },
                        { "AreSlotsFull", !CanAddConsumable() }
                    });
            consumables.Clear();
        }

        public List<VConsumable> GetConsumables()
        {
            return consumables;
        }
    }
}