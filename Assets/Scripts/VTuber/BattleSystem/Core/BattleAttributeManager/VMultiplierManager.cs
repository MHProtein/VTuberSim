using System;
using System.Collections.Generic;
using System.Linq;
using VTuber.BattleSystem.BattleAttribute;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VMultiplierManagerSaveData
    {
        public List<int> multiplierSequence;
        public int currentTurnIndex;
    }
    public class VMultiplierManager
    {
        public VBattleMultiplierAttribute Multiplier { get; private set; }
        public List<VBattleMultiplierAttribute> Multipliers => _multiplierAttributes;
        private List<VBattleMultiplierAttribute> _multiplierAttributes;
        private List<int> multiplierSequence;
        private int _currentTurnIndex = 0;
        public VMultiplierManager(int mainAttributeIndex, 
            int maxConsecutiveMultiplierCount, List<int> abilityTurnCounts, 
            VBattleMultiplierAttribute singingMultiplierAttribute, 
            VBattleMultiplierAttribute gamingMultiplierAttribute,
            VBattleMultiplierAttribute chattingMultiplierAttribute,
            VBattleTurnAttribute turnAttribute)
        {
            _multiplierAttributes = new List<VBattleMultiplierAttribute>
            {
                singingMultiplierAttribute, //red
                gamingMultiplierAttribute, //yellow
                chattingMultiplierAttribute //blue
            };
            
            multiplierSequence = new List<int>(new int[turnAttribute.MaxTurn]);
            multiplierSequence[0] = mainAttributeIndex;
            multiplierSequence[^1] = mainAttributeIndex;

            GenerateMultiplierSequence(turnAttribute.MaxTurn, maxConsecutiveMultiplierCount, mainAttributeIndex, abilityTurnCounts);
        }
        
        public VMultiplierManager(int mainAttributeIndex, 
            int maxConsecutiveMultiplierCount, List<int> abilityTurnCounts, 
            VBattleMultiplierAttribute singingMultiplierAttribute, 
            VBattleMultiplierAttribute gamingMultiplierAttribute,
            VBattleMultiplierAttribute chattingMultiplierAttribute,
            VBattleTurnAttribute turnAttribute, VMultiplierManagerSaveData saveData)
        {
            multiplierSequence = saveData.multiplierSequence;
            _currentTurnIndex = saveData.currentTurnIndex;
            
            
            _multiplierAttributes = new List<VBattleMultiplierAttribute>
            {
                singingMultiplierAttribute, //red
                gamingMultiplierAttribute, //yellow
                chattingMultiplierAttribute //blue
            };
            
            multiplierSequence = saveData.multiplierSequence;
            _currentTurnIndex = saveData.currentTurnIndex;
            
            VBattleRootEventCenter.Instance.Raise(
                VBattleEventKey.OnMultiplierSequenceCalculated,
                new Dictionary<string, object>
                {
                    { "Colors", multiplierSequence.Select(index => _multiplierAttributes[index].color).ToList() }
                });
        }
        
        public VMultiplierManagerSaveData SaveData()
        {
            return new VMultiplierManagerSaveData
            {
                multiplierSequence = multiplierSequence,
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
            int delta = (int)messagedict["Delta"];
            if (delta <= 0)
                return;

            if (multiplierSequence is null)
                return;
            for (int i = 0; i < delta; i++)
            {
                multiplierSequence.Add(multiplierSequence.Last());
            }
        }
        
        private void OnTurnBegin(Dictionary<string, object> messagedict)
        {
            VBattleRootEventCenter.Instance.Raise(VBattleEventKey.OnRotateMultiplier, new Dictionary<string, object>()
            {
                { "Name", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].AttributeName },
                { "NewValue", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].Value },
                { "Color", _multiplierAttributes[multiplierSequence[_currentTurnIndex]].color },
            });
            Multiplier = _multiplierAttributes[multiplierSequence[_currentTurnIndex]];
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
            if (abilityTurnCounts.Count != _multiplierAttributes.Count)
                throw new ArgumentException("abilityTurnCounts must match the number of abilities.");

            if (abilityTurnCounts.Sum() != maxTurn)
                throw new ArgumentException("Total turns from abilityTurnCounts must equal maxTurn.");

            const int maxRetries = 100;
            int attempt = 0;

            while (attempt++ < maxRetries)
            {
                multiplierSequence = Enumerable.Repeat(-1, maxTurn).ToList();
                multiplierSequence[0] = mainAttributeIndex;
                multiplierSequence[^1] = mainAttributeIndex;

                List<int> remainingCounts = new List<int>(abilityTurnCounts);
                remainingCounts[mainAttributeIndex] -= 2;

                bool success = true;

                for (int i = 1; i < maxTurn - 1; i++)
                {
                    int prev = multiplierSequence[i - 1];
                    int consecutiveCount = 1;

                    for (int j = i - 2; j >= 0 && multiplierSequence[j] == prev; j--)
                        consecutiveCount++;

                    List<int> candidates = new List<int>();
                    for (int ability = 0; ability < remainingCounts.Count; ability++)
                    {
                        if (remainingCounts[ability] > 0)
                            candidates.Add(ability);
                    }

                    if (consecutiveCount >= maxConsecutiveMultiplierCount)
                        candidates.Remove(prev);

                    if (i == maxTurn - 2)
                        candidates.Remove(mainAttributeIndex);

                    if (candidates.Count == 0)
                    {
                        success = false;
                        break;
                    }

                    int choice = candidates[UnityEngine.Random.Range(0, candidates.Count)];
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
                            { "Colors", multiplierSequence.Select(index => _multiplierAttributes[index].color).ToList() }
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
            _multiplierAttributes.Clear();
            _multiplierAttributes = null;
        }
    }
}