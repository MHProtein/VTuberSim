using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEditor;
using UnityEngine;
using VTuber.BattleSystem.Core;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events;
using Random = UnityEngine.Random;

namespace VTuber.CoopSystem
{
    public class VCoopEventHeaderIndex
    {
        public const int Id = 0;
        public const int Name = 1;
        public const int Description = 2;
        public const int Probability = 3;
        public const int UnlockLevel = 4;
        public const int EventTypes = 5;
        public const int Effect1 = 6;
        public const int E1Param = 7;
        public const int Effect2 = 8;
        public const int E2Param = 9;
        public const int Effect3 = 10;
        public const int E3Param = 11;
    }

    public class VCoopEvent
    {
        public List<VRaisingEffect> effects;
        public string eventName;
        public List<VCoopEventType> eventTypes;
        public uint id;
        public float probability;
        public int unlockLevel;
        public string description;

        public VCoopEvent(CellRange row)
        {
            id = uint.Parse(row.Columns[VCoopEventHeaderIndex.Id].Value);
            eventName = row.Columns[VCoopEventHeaderIndex.Name].Value;
            unlockLevel = int.Parse(row.Columns[VCoopEventHeaderIndex.UnlockLevel].Value);
            probability = float.Parse(row.Columns[VCoopEventHeaderIndex.Probability].Value);
            description = row.Columns[VCoopEventHeaderIndex.Description].Value;

            effects = new List<VRaisingEffect>();
            for (var i = VCoopEventHeaderIndex.Effect1; i <= VCoopEventHeaderIndex.E3Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if (effectIDStr.IsNullOrWhitespace())
                    continue;
                effects.Add(VDataManager.Instance.CreateRaisingEffectByID(Convert.ToUInt32(effectIDStr),
                    row.Columns[i + 1].Value.Trim(), row.Columns[i + 1].Value.Trim()));
            }

            eventTypes = new List<VCoopEventType>();
            foreach (var type in row.Columns[VCoopEventHeaderIndex.EventTypes].Value.Split(','))
            {
                var typeStr = type.Trim();
                var t = new VCoopEventType();
                if (type.Contains("Stream") && type.Length != 6)
                {
                    t.abilityIndex = int.Parse(type.Substring(6));
                    typeStr = "Stream";
                }
                else
                {
                    t.abilityIndex = -1;
                }

                t.eventType = Enum.Parse<VEventType>(typeStr);
                eventTypes.Add(t);
            }
        }

        public struct VCoopEventType
        {
            public VEventType eventType;
            public int abilityIndex;
        }
    }

    public struct VCoopEventItem
    {
        public VCoopEvent e;
        public Sprite pfp;
        public Vector2Int position;
        public string description;
    }

    public class VCoopSaveData
    {
        public uint configId;
        public int coopValue;
        public int currentLevelIndex;
        public bool hasExecutedUpgradeEventThisWeek;
        public int upgradeEvent;
    }

    public class VCooperator
    {
        public readonly VCooperatorConfiguration configuration;

        private bool _hasExecutedUpgradeEventThisWeek;

        public VCooperator(VCooperatorConfiguration configuration)
        {
            CurrentLevel = 0;
            this.configuration = configuration;
            CoopEvents = this.configuration.CoopEvents.Select(@event => VDataManager.Instance.GetCoopEventByID(@event))
                .ToList();
        }

        public uint Id => configuration.Id;

        public VCoopLevel CurrentCoopLevel => configuration.CoopLevels[CurrentLevel];

        public int CoopValue { get; private set; }

        public int CurrentLevel { get; private set; }

        public List<VCoopEvent> CoopEvents { get; }

        public VScheduleEvent UpgradeEvent { get; private set; }
        public Sprite Pfp => configuration.Icon;
        public string CoopName => configuration.Name;

        public static VCooperator Load(VCoopSaveData saveData)
        {
            var configuration = VGameManager.Instance.GetCooperatorConfigurationByID(saveData.configId);
            var cooperator = new VCooperator(configuration);
            cooperator.CurrentLevel = saveData.currentLevelIndex;
            cooperator.CoopValue = saveData.coopValue;
            cooperator._hasExecutedUpgradeEventThisWeek = saveData.hasExecutedUpgradeEventThisWeek;
            if (saveData.upgradeEvent != -1)
            {
                if (cooperator.CurrentCoopLevel.eventType == VEventType.Stream)
                    cooperator.UpgradeEvent = VDataManager.Instance.CreateStreamEventByID((uint)saveData.upgradeEvent);
                else
                    cooperator.UpgradeEvent =
                        VDataManager.Instance.CreateDialogueEventByID((uint)saveData.upgradeEvent);
            }

            return cooperator;
        }

        public VCoopSaveData Save()
        {
            return new VCoopSaveData
            {
                configId = configuration.Id,
                currentLevelIndex = CurrentLevel,
                coopValue = CoopValue,
                hasExecutedUpgradeEventThisWeek = _hasExecutedUpgradeEventThisWeek,
                upgradeEvent = UpgradeEvent is not null ? (int)UpgradeEvent.EventID : -1
            };
        }

        public void OnEnable()
        {
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnWeekEnd, OnWeekEnd);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToModifySchedule,
                OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSwitchToScheduleCreation,
                OnSwitchToScheduleCreation);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }

        public void OnDisable()
        {
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnWeekEnd, OnWeekEnd);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToModifySchedule,
                OnSwitchToModifySchedule);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSwitchToScheduleCreation,
                OnSwitchToScheduleCreation);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnEventEnd, OnEventEnd);
        }

        private void OnSwitchToScheduleCreation(Dictionary<string, object> messagedict)
        {
            if (!_hasExecutedUpgradeEventThisWeek && UpgradeEvent != null)
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSetCoopUpgradeEvent,
                    new Dictionary<string, object>
                    {
                        { "Cooperator", this }
                    });
        }

        private void OnSwitchToModifySchedule(Dictionary<string, object> messagedict)
        {
            if (!_hasExecutedUpgradeEventThisWeek && UpgradeEvent != null)
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnSetCoopUpgradeEvent,
                    new Dictionary<string, object>
                    {
                        { "Cooperator", this }
                    });
        }

        private void OnWeekEnd(Dictionary<string, object> messagedict)
        {
            _hasExecutedUpgradeEventThisWeek = false;
        }

        private void OnEventEnd(Dictionary<string, object> messagedict)
        {
            if (messagedict["Event"] == UpgradeEvent)
            {
                _hasExecutedUpgradeEventThisWeek = true;
                UpgradeEvent = null;
            }
        }

        public void AddCoopValue(int value)
        {
            CoopValue += value;
            VDebug.Log("CoopValue: " + CoopValue);
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorValueUpdated,
                new Dictionary<string, object>
                {
                    { "Cooperator", this },
                    { "Level", CurrentLevel }
                });
            if (CoopValue - CurrentCoopLevel.to >= 0)
            {
                if (CurrentCoopLevel.eventType == VEventType.Stream)
                    UpgradeEvent = VDataManager.Instance.CreateStreamEventByID(CurrentCoopLevel.upgradeEventID);
                else
                    UpgradeEvent = VDataManager.Instance.CreateDialogueEventByID(CurrentCoopLevel.upgradeEventID);
            }
        }

        public void UpgradeLevel()
        {
            CurrentLevel = Mathf.Clamp(CurrentLevel + 1, 0, configuration.CoopLevels.Count - 1);

            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnCooperatorValueUpdated,
                new Dictionary<string, object>
                {
                    { "Cooperator", this },
                    { "Level", CurrentLevel }
                });
            if (CurrentLevel == configuration.CoopLevels.Count - 1)
            {
                UpgradeEvent = null;
                return;
            }

            if (CoopValue - CurrentCoopLevel.to >= 0)
            {
                if (CurrentCoopLevel.eventType == VEventType.Stream)
                    UpgradeEvent = VDataManager.Instance.CreateStreamEventByID(CurrentCoopLevel.upgradeEventID);
                else
                    UpgradeEvent = VDataManager.Instance.CreateDialogueEventByID(CurrentCoopLevel.upgradeEventID);
            }

            _hasExecutedUpgradeEventThisWeek = true;
        }

        #region CoopEventGeneration

        public List<VCoopEventItem> GenerateCoopEventPositions(List<Vector2Int> occupiedPositions)
        {
            CoopEvents.Sort((x, y) => x.probability.CompareTo(y.probability));
            var events = new List<VCoopEventItem>();
            var positionCount = Random.Range(configuration.MinEvents, configuration.MaxEvents + 1);
            for (var i = 0; i < positionCount; i++)
            {
                var position = new Vector2Int(GetDay(), GetTime());
                while (occupiedPositions.Contains(position) || events.Exists(x => x.position == position))
                    position = new Vector2Int(GetDay(), GetTime());

                VDebug.Log(position);

                VCoopEvent e = null;
                while (e is null) e = GetEvent();
                events.Add(new VCoopEventItem
                {
                    e = e,
                    position = position,
                    pfp = configuration.Icon,
                    description = e.description
                });
            }

            return events;
        }

        public VCoopEvent GetEvent()
        {
            var events = CoopEvents.Where(x => x.unlockLevel <= CurrentLevel).ToList();
            if (events.Count == 1)
                return events.FirstOrDefault();
            var probabilitySum = events.Sum(x => x.probability);
            if (probabilitySum <= 0) return null;

            var probability = Random.Range(0, 1f);
            float totalProbability = 0;
            foreach (var e in events)
            {
                totalProbability += e.probability / probabilitySum;
                if (probability <= totalProbability) return e;
            }

            return null;
        }

        public int GetDay()
        {
            var dayProbabilities = configuration.DayProbabilities;
            var probabilitySum = dayProbabilities.Sum();
            var probability = Random.Range(0, 1f);
            float totalProbability = 0;
            for (var i = 0; i < dayProbabilities.Count; i++)
            {
                totalProbability += dayProbabilities[i] / probabilitySum;
                if (probability <= totalProbability) return i;
            }

            return dayProbabilities.Count - 1;
        }

        public int GetTime()
        {
            var timeProbabilities = configuration.DayTimeProbabilities;
            var probabilitySum = timeProbabilities.Sum();
            var probability = Random.Range(0, 1f);
            float totalProbability = 0;
            for (var i = 0; i < timeProbabilities.Count; i++)
            {
                totalProbability += timeProbabilities[i] / probabilitySum;
                if (probability <= totalProbability) return i;
            }

            return timeProbabilities.Count - 1;
        }

        #endregion

        public VCoopLevel GetNextLevel()
        {
            return configuration.CoopLevels[CurrentLevel + 1];
        }

        public string GetLevelName(int targetValue)
        {
            return configuration.CoopLevels[targetValue].levelName;
        }
    }
}