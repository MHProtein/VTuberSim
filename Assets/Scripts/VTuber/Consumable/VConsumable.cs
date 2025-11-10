using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Effect;
using VTuber.Core.RaisingEffect;

namespace VTuber.Consumable
{
    public class VRaisingConsumable : VConsumable
    {
        public List<VRaisingEffect> raisingEffects;

        public VRaisingConsumable(uint id, VRaisingConsumableConfiguration configuration) : base(id, configuration)
        {
            type = VConsumableType.Raising;
            raisingEffects = configuration.effects;
        }

        public override bool CanApply()
        {
            return _consumableManager.CanUseConsumable;
        }

        public override void ApplyEffect()
        {
            _consumableManager.ApplyRaisingEffects(raisingEffects);
            _consumableManager.Remove(this);
        }
    }

    public class VBattleConsumable : VConsumable
    {
        public List<VEffect> effects;

        public VBattleConsumable(uint id, VBattleConsumableConfiguration configuration) : base(id, configuration)
        {
            type = VConsumableType.Stream;
            effects = configuration.effects;
        }

        public override void ApplyEffect()
        {
            _consumableManager.ApplyBattleEffects(effects);
            _consumableManager.Remove(this);
        }

        public override bool CanApply()
        {
            return _consumableManager.CanUseBattleConsumable();
        }
    }

    public class VConsumable
    {
        public VConsumableConfiguration _configuration;

        protected VConsumableManager _consumableManager;

        public VConsumableType type;

        public VConsumable(uint id, VConsumableConfiguration configuration)
        {
            Id = id;
            _configuration = configuration;
        }

        public uint Id { get; private set; }
        public uint ConfigId => _configuration.id;
        public string Name => _configuration.name;
        public string Description => _configuration.description;
        public Sprite Icon => _configuration.icon;
        public VConsumableRarity Rarity => _configuration.rarity;

        public virtual bool CanApply()
        {
            return false;
        }

        public void Initialize(VConsumableManager consumableManager)
        {
            _consumableManager = consumableManager;
        }

        public virtual void ApplyEffect()
        {
        }

        public void Discard()
        {
            _consumableManager.Remove(this);
        }
    }
}