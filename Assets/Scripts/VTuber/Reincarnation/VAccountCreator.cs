using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Relic;

namespace VTuber.Reincarnation
{
    public class VAccountCreator
    {
        public static VAccount CreateAccount(VReincarnationConfiguration config, string ratingLevel,
            VCharacter character)
        {
            var account = new VAccount(ratingLevel, GetCards(config, ratingLevel, character),
                GetRelics(config, ratingLevel, character),
                GetEffects(config, ratingLevel, character));
            return account;
        }

        public static List<VCard> GetCards(VReincarnationConfiguration config, string ratingLevel, VCharacter character)
        {
            // Work on a *mutable* pool so we can remove cards as we pick them
            var pool = character.CardLibrary.GetCards()
                .OrderBy(_ => Random.Range(0f, 1f))
                .ToList();

            var cards = new List<VCard>();

            var currentCapacity = 0;
            var maxCapacity = config.cardLevels[ratingLevel].cardTotalCapacity;
            var maxCount = config.cardLevels[ratingLevel].cardCount;

            // ----- STEP 1: rarity requirements -----
            foreach (var requirement in config.cardLevels[ratingLevel].cardRarityRequirements)
            {
                // find all candidates that meet rarity, from the shrinking pool
                var candidates = pool
                    .Where(card => (int)card.Rarity >= (int)requirement.rarity)
                    .OrderBy(_ => Random.Range(0f, 1f))
                    .ToList();

                var takeCount = Mathf.Min(requirement.count, candidates.Count);

                for (int i = 0; i < takeCount; i++)
                {
                    var card = candidates[i];
                    cards.Add(card);
                    pool.Remove(card); // ← prevents duplicates
                }
            }

            // ----- Recalculate capacity -----
            currentCapacity = 0;
            foreach (var card in cards)
            {
                foreach (var cap in config.cardCapacities)
                {
                    if (cap.rarity == card.Rarity)
                    {
                        currentCapacity += card.IsUpgraded ? cap.upgradeCapacity : cap.capacity;
                        break;
                    }
                }
            }

            // How much space is left?
            var remainingCapacity = maxCapacity - currentCapacity;

            // ----- STEP 2: fill remaining slots optimally -----
            // pool is already shuffled from above
            foreach (var card in pool.ToList()) // iterate safely
            {
                if (cards.Count >= maxCount) break;

                int capacity = 0;
                foreach (var cap in config.cardCapacities)
                {
                    if (cap.rarity == card.Rarity)
                    {
                        capacity = card.IsUpgraded ? cap.upgradeCapacity : cap.capacity;
                        break;
                    }
                }

                if (capacity <= remainingCapacity)
                {
                    cards.Add(card);
                    pool.Remove(card); // ← also prevents duplicates here
                    remainingCapacity -= capacity;

                    if (remainingCapacity == 0)
                        break;
                }
            }

            return cards;
        }

        public static List<VRelic> GetRelics(VReincarnationConfiguration config, string ratingLevel,
            VCharacter character)
        {
            if (character is null)
                return new List<VRelic>();
            var relicCount = config.relicCount[ratingLevel];
            var relicIDs = new List<uint>();
            var streamEvents = character.succeededStreams;
            foreach (var relicInfo in config.relicRewards)
                if (streamEvents.Contains(relicInfo.eventID))
                    relicIDs.Add(relicInfo.relicIDs[Random.Range(0, relicInfo.relicIDs.Count)]);

            if (relicCount < relicIDs.Count)
                relicIDs = relicIDs.OrderBy(r => Random.Range(0f, 1f)).Take(relicCount).ToList();

            var relics = new List<VRelic>();
            foreach (var relicID in relicIDs)
            {
                var relic = VDataManager.Instance.CreateRelicByID(relicID);
                relics.Add(relic);
            }

            return relics;
        }

        public static List<VEffectItem> GetEffects(VReincarnationConfiguration config, string ratingLevel,
            VCharacter character)
        {
            var count = config.attributeLevels[ratingLevel].count;
            var capacity = 0;
            var maxCapacity = config.attributeLevels[ratingLevel].capacity;
            var ratingLevelInfo = config.attributeLevels[ratingLevel];

            // StreamAttributes
            var streamEffectInfos = new List<VAbilityEffectInfo>();
            var streamEffects = new List<VEffectItem>();

            foreach (var requirement in ratingLevelInfo.streamEffectsRequirements)
                for (var i = 0; i < requirement.count; i++)
                {
                    var effectInfo =
                        config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];
                    while (streamEffectInfos.Find(info => info.ability == effectInfo.ability) != null)
                        effectInfo =
                            config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];

                    var level = Random.Range(requirement.level, requirement.highestLevel + 1);
                    var param = effectInfo.levelInfos[level].levelParam;

                    streamEffects.Add(new VEffectItem
                    {
                        id = effectInfo.effect,
                        parameter = param,
                        upgradedParameter = param,
                        level = level
                    });

                    streamEffectInfos.Add(effectInfo);
                    capacity += config.effectCapacities[level];

                    if (streamEffects.Count + 0 >= count) // already at max effects
                        return streamEffects;
                }

            // OtherAttributes
            var otherEffects = new List<VEffectItem>();
            var otherEffectInfos = new List<VAttributeEffectInfo>();
            foreach (var requirement in ratingLevelInfo.attributeEffectsRequirements)
                for (var i = 0; i < requirement.count; i++)
                {
                    var effectInfo = config.attributeEffects[Random.Range(0, config.attributeEffects.Count)];
                    while (otherEffectInfos.Find(info => info.effect == effectInfo.effect) != null)
                        effectInfo = config.attributeEffects[Random.Range(0, config.attributeEffects.Count)];

                    if (streamEffects.Count + otherEffects.Count >= count)
                        return otherEffects.Union(streamEffects).ToList();
                    var level = Random.Range(requirement.level, requirement.highestLevel + 1);
                    var param = effectInfo.levelInfos[level].levelParam;

                    otherEffects.Add(new VEffectItem
                    {
                        id = effectInfo.effect,
                        parameter = param,
                        upgradedParameter = param,
                        level = level
                    });
                    otherEffectInfos.Add(effectInfo);

                    capacity += config.effectCapacities[level];
                }

            while (streamEffects.Count < 2)
            {
                var effectInfo = config.streamAttributeEffects[Random.Range(0, config.streamAttributeEffects.Count)];
                if (streamEffectInfos.Find(info => info.ability == effectInfo.ability) != null)
                    continue;

                var level = Random.Range(0, effectInfo.levelInfos.Count);
                var levelInfo = effectInfo.levelInfos[level];

                if (capacity + config.effectCapacities[level] <= maxCapacity)
                {
                    streamEffects.Add(new VEffectItem
                    {
                        id = effectInfo.effect,
                        parameter = levelInfo.levelParam,
                        upgradedParameter = levelInfo.levelParam,
                        level = level
                    });
                    ;
                    streamEffectInfos.Add(effectInfo);
                    capacity += config.effectCapacities[level];

                    if (streamEffects.Count >= count)
                        return streamEffects;
                }
            }

            // Remaining pool (allEffects)
            var allEffects = config.attributeEffects.Where(e =>
                    !otherEffects.Exists(eft => eft.id == e.effect))
                .OrderBy(r => Random.Range(0f, 1f)).ToList(); // shuffle.ToList();

            foreach (var streamAttributeEffect in config.streamAttributeEffects)
                if (!streamEffectInfos.Exists(info => info.ability == streamAttributeEffect.ability) &&
                    !allEffects.Exists(info =>
                    {
                        if (info is VAbilityEffectInfo abilityEffectInfo)
                            return abilityEffectInfo.ability == streamAttributeEffect.ability;
                        return false;
                    }))
                    allEffects.Add(streamAttributeEffect);

            // Fill with random remaining effects without exceeding capacity and count
            foreach (var effectInfo in allEffects)
            {
                if (streamEffects.Count + otherEffects.Count >= count)
                    break;

                var level = Random.Range(0, effectInfo.levelInfos.Count);
                var levelInfo = effectInfo.levelInfos[level];

                var cost = config.effectCapacities[level];
                if (capacity + cost > maxCapacity)
                    continue;

                otherEffects.Add(new VEffectItem
                {
                    id = effectInfo.effect,
                    parameter = levelInfo.levelParam,
                    upgradedParameter = levelInfo.levelParam,
                    level = level
                });
                ;
                capacity += cost;

                if (capacity == maxCapacity || streamEffects.Count + otherEffects.Count >= count)
                    break;
            }

            if (capacity > maxCapacity)
                VDebug.LogError(ratingLevel + " Capacity exceeded " + (maxCapacity - capacity) + " " +
                                streamEffects.Union(otherEffects).Count());
            else
                VDebug.Log(maxCapacity - capacity);

            return streamEffects.Union(otherEffects).ToList();
        }
    }
}