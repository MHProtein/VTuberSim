using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Core.Foundation;
using Random = UnityEngine.Random;

namespace VTuber.BattleSystem.Core
{
    [Serializable]
    public class VMultiplierManagerSaveData
    {
        public int currentTurnIndex;
        public List<int> multiplierSequence;
    }

    public class VMultiplierManager
    {
        private int _currentTurnIndex;
        private List<int> multiplierSequence;

        public VMultiplierManager(int mainAttributeIndex,
            int maxConsecutiveMultiplierCount, List<int> abilityTurnCounts,
            VBattleMultiplierAttribute singingMultiplierAttribute,
            VBattleMultiplierAttribute gamingMultiplierAttribute,
            VBattleMultiplierAttribute chattingMultiplierAttribute,
            VBattleTurnAttribute turnAttribute)
        {
            Multipliers = new List<VBattleMultiplierAttribute>
            {
                singingMultiplierAttribute, //red
                gamingMultiplierAttribute, //yellow
                chattingMultiplierAttribute //blue
            };

            multiplierSequence = new List<int>(new int[turnAttribute.MaxTurn]);
            multiplierSequence[0] = mainAttributeIndex;
            multiplierSequence[^1] = mainAttributeIndex;

            GenerateMultiplierSequence(turnAttribute.MaxTurn, maxConsecutiveMultiplierCount, mainAttributeIndex,
                abilityTurnCounts);
        }

        public VMultiplierManager(VBattleMultiplierAttribute singingMultiplierAttribute,
            VBattleMultiplierAttribute gamingMultiplierAttribute,
            VBattleMultiplierAttribute chattingMultiplierAttribute,
            VMultiplierManagerSaveData saveData)
        {
            multiplierSequence = saveData.multiplierSequence;
            _currentTurnIndex = saveData.currentTurnIndex - 1;

            Multipliers = new List<VBattleMultiplierAttribute>
            {
                singingMultiplierAttribute, //red
                gamingMultiplierAttribute, //yellow
                chattingMultiplierAttribute //blue
            };

            multiplierSequence = saveData.multiplierSequence;
            Multiplier = Multipliers[multiplierSequence[_currentTurnIndex]];

            VBattleRootEventCenter.Instance.Raise(
                VBattleEventKey.OnMultiplierSequenceCalculated,
                new Dictionary<string, object>
                {
                    { "Colors", multiplierSequence.Select(index => Multipliers[index].color).ToList() },
                    { "Index", _currentTurnIndex }
                });
        }

        public VBattleMultiplierAttribute Multiplier { get; private set; }
        public List<VBattleMultiplierAttribute> Multipliers { get; private set; }

        public VMultiplierManagerSaveData Save()
        {
            return new VMultiplierManagerSaveData
            {
                multiplierSequence = new List<int>(multiplierSequence),
                currentTurnIndex = _currentTurnIndex
            };
        }

        public void OnEnable()
        {
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnTurnChange, OnTurnChange);
        }

        public void OnDisable()
        {
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnBegin, OnTurnBegin);
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnTurnChange, OnTurnChange);
        }

        private void OnTurnChange(Dictionary<string, object> messagedict)
        {
            var delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            if (multiplierSequence is null)
                return;
            for (var i = 0; i < delta; i++) multiplierSequence.Add(multiplierSequence.Last());
        }

        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRotateMultiplier, new Dictionary<string, object>
            {
                { "Name", Multipliers[multiplierSequence[_currentTurnIndex]].AttributeName },
                { "NewValue", Multipliers[multiplierSequence[_currentTurnIndex]].Value },
                { "Color", Multipliers[multiplierSequence[_currentTurnIndex]].color }
            });
            Multiplier = Multipliers[multiplierSequence[_currentTurnIndex]];
            if (_currentTurnIndex <= multiplierSequence.Count - 1)
            {
                _currentTurnIndex++;
                VDebug.Log("_currentTurnIndex: " + _currentTurnIndex);
            }

            VDebug.Log(Multiplier.AttributeName + " Value : " + Multiplier.Value);
        }

        private void GenerateMultiplierSequence(
            int maxTurn,
            int maxConsecutiveMultiplierCount,
            int mainAttributeIndex,
            List<int> abilityTurnCounts)
        {
            if (abilityTurnCounts.Count != Multipliers.Count)
                throw new ArgumentException("abilityTurnCounts must match the number of abilities.");

            if (abilityTurnCounts.Sum() != maxTurn)
                throw new ArgumentException("Total turns from abilityTurnCounts must equal maxTurn.");

            const int maxRetries = 100;
            var attempt = 0;

            while (attempt++ < maxRetries)
            {
                multiplierSequence = Enumerable.Repeat(-1, maxTurn).ToList();
                multiplierSequence[0] = mainAttributeIndex;
                multiplierSequence[^1] = mainAttributeIndex;

                var remainingCounts = new List<int>(abilityTurnCounts);
                remainingCounts[mainAttributeIndex] -= 2;

                var success = true;

                for (var i = 1; i < maxTurn - 1; i++)
                {
                    var prev = multiplierSequence[i - 1];
                    var consecutiveCount = 1;

                    for (var j = i - 2; j >= 0 && multiplierSequence[j] == prev; j--)
                        consecutiveCount++;

                    var candidates = new List<int>();
                    for (var ability = 0; ability < remainingCounts.Count; ability++)
                        if (remainingCounts[ability] > 0)
                            candidates.Add(ability);

                    if (consecutiveCount >= maxConsecutiveMultiplierCount)
                        candidates.Remove(prev);

                    if (i == maxTurn - 2)
                        candidates.Remove(mainAttributeIndex);

                    if (candidates.Count == 0)
                    {
                        success = false;
                        break;
                    }

                    var choice = candidates[Random.Range(0, candidates.Count)];
                    multiplierSequence[i] = choice;
                    remainingCounts[choice]--;
                }

                if (success)
                {
                    // Success → raise event
                    VBattleRootEventCenter.Instance.Raise(
                        VBattleEventKey.OnMultiplierSequenceCalculated,
                        new Dictionary<string, object>
                        {
                            { "Colors", multiplierSequence.Select(index => Multipliers[index].color).ToList() }
                        });
                    return;
                }
            }

            throw new InvalidOperationException("Unable to generate valid multiplier sequence after retries.");
        }


        public void Reset()
        {
            Multiplier = null;
            multiplierSequence.Clear();
            multiplierSequence = null;
            Multipliers.Clear();
            Multipliers = null;
        }
    }
}