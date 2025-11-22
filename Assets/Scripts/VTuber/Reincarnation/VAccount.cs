using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VTuber.BattleSystem.Card;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Relic;

namespace VTuber.Reincarnation
{
    public class VAccountSaveData
    {
        public string accountName;
        public List<uint> cards;
        public List<int> effectLevels;
        public List<VEffectItem> effects;
        public List<uint> relics;
        public string scoreLevel;
        public string icon;
    }

    public class VAccount
    {
        private readonly List<VEffectItem> _effectItems;
        public string accountName;
        public Sprite icon;

        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VEffectItem> effects)
        {
            Cards = cards;
            Relics = relics;

            ScoreLevel = scoreLevel;
            _effectItems = effects;

            Effects = new List<VRaisingEffect>();
            foreach (var effect in effects) Effects.Add(effect.CreateRaisingEffect());

            EffectLevels = new List<int>();
            foreach (var effect in effects) EffectLevels.Add(effect.level);
        }

        public VAccount(VAccountSaveData data)
        {
            accountName = data.accountName;
            ScoreLevel = data.scoreLevel;
            Cards = data.cards.Select(card => VDataManager.Instance.CreateCardByID(card)).ToList();
            Relics = data.relics.Select(relic => VDataManager.Instance.CreateRelicByID(relic)).ToList();
            EffectLevels = data.effectLevels;
            _effectItems = data.effects;
            Effects = new List<VRaisingEffect>();
            if (_effectItems is null)
                return;
            foreach (var effect in _effectItems) Effects.Add(effect.CreateRaisingEffect());
            icon = VResourcesManager.Instance.TryGetSprite(data.icon);
        }

        public VAccount(string scoreLevel, List<VCard> cards, List<VRelic> relics, List<VRaisingEffect> effects,
            List<int> effectLevels)
        {
            ScoreLevel = scoreLevel;
            Cards = cards;
            Relics = relics;

            Effects = effects;
            EffectLevels = effectLevels;
        }

        public List<VCard> Cards { get; }

        public List<VRelic> Relics { get; }

        public List<VRaisingEffect> Effects { get; }

        public List<int> EffectLevels { get; }

        public string ScoreLevel { get; }

        public VAccountSaveData Save()
        {
            return new VAccountSaveData
            {
                accountName = accountName,
                cards = Cards.Select(card => card.configID).ToList(),
                relics = Relics.Select(relic => relic.ConfigId).ToList(),
                effects = _effectItems,
                effectLevels = EffectLevels,
                scoreLevel = ScoreLevel,
                icon = icon.name
            };
        }
    }
}