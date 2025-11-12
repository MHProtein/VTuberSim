using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.BattleSystem.Core.ScriptSystem;
using VTuber.Character;
using VTuber.Character.Attributes;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Core.ScriptSystem
{
    public struct VScoreResult
    {
        public int score;
        public string scoreLevelName;
    }

    public struct VScriptSaveData
    {
        public string scriptConfigurationName;
        public int currentPhaseIndex;
        public int weekIndex;
    }

    public class VScript
    {
        protected int _weekIndex;
        protected VScriptConfiguration configuration;
        protected VPhase currentPhase;
        public List<VKPI> kpis;

        public VScript(VScriptConfiguration configuration)
        {
            this.configuration = configuration;
            kpis = new List<VKPI>();
            foreach (var kpi in configuration.kpis)
                kpis.Add(new VKPI(kpi.EventType, kpi.RequiredAmount, kpi.AbilityIndex, true));
        }

        protected List<VPhase> Phases => configuration.phases;

        public VPhase CurrentPhase => currentPhase;

        public int WeekIndex => _weekIndex;

        public List<uint> EventList => configuration.eventIDs;
        public List<uint> StreamEventList => configuration.streamEventIDs;

        public VScriptSaveData Save()
        {
            return new VScriptSaveData
            {
                scriptConfigurationName = configuration.name,
                currentPhaseIndex = Phases.IndexOf(currentPhase),
                weekIndex = _weekIndex
            };
        }

        public static VScript Load(VScriptSaveData data, VScriptConfiguration scriptConfig)
        {
            var script = new VScript(scriptConfig);
            script.currentPhase = script.Phases[data.currentPhaseIndex];
            script._weekIndex = data.weekIndex;
            return script;
        }

        public VScheduleEvent BeginScript()
        {
            for (var i = 0; i < Phases.Count - 1; i++) Phases[i].nextPhase = Phases[i + 1];
            currentPhase = Phases[0];
            return currentPhase.GetStartEvent();
        }

        public List<VSpecialEventData> GetSpecialEvents(int weekIndex)
        {
            var events = new List<VSpecialEventData>();
            foreach (var phase in Phases)
                if (phase.IsInPhase(weekIndex))
                    events.AddRange(phase.GetSpecialEventData(weekIndex));
            return events;
        }

        public virtual VScheduleEvent NextWeek()
        {
            if (currentPhase.nextPhase is null)
                return null;
            _weekIndex++;
            if (currentPhase.nextPhase.IsInPhase(_weekIndex))
            {
                currentPhase = currentPhase.nextPhase;
                VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnPhaseBegin, new Dictionary<string, object>());
                return currentPhase.GetStartEvent();
            }

            return null;
        }

        public VScoreResult CalculateScore(VCharacter character, int popularity, bool success)
        {
            var singingAbility = character.AttributeManager.Attributes["CASingingAbility"].Value;
            var gamingAbility = character.AttributeManager.Attributes["CAGamingAbility"].Value;
            var chattingAbility = character.AttributeManager.Attributes["CAChattingAbility"].Value;
            var follower = character.AttributeManager.Attributes["CAFollowerCount"].Value;
            var highestMembershipCount =
                (character.AttributeManager.Attributes["CAMembershipCount"] as VMembershipCountAttribute).highestValue;

            float popularityCoefficient = 0;
            foreach (var range in configuration.popularityCoefficient)
                if (range.IsInRange(popularity))
                    popularityCoefficient = range.value;

            var score = Mathf.CeilToInt(
                (singingAbility + gamingAbility + chattingAbility) * configuration.abilityCoefficient +
                follower * configuration.followerCoefficient
                + highestMembershipCount * configuration.membershipCoefficient
                + popularityCoefficient * popularity);
            if (success)
                score += configuration.successBonus;

            var scoreLevel = configuration.scoreLevels.Find(level => level.InLevel(score));
            VDebug.Log("歌力： " + singingAbility);
            VDebug.Log("游戏力： " + gamingAbility);
            VDebug.Log("杂谈力： " + chattingAbility);
            VDebug.Log("关注人数： " + follower);
            VDebug.Log("最高舰长数： " + highestMembershipCount);
            VDebug.Log("直播热度： " + popularity);
            VDebug.Log("分数： " + score);
            VDebug.Log("等级： " + scoreLevel);
            return new VScoreResult
            {
                score = score,
                scoreLevelName = scoreLevel.name
            };
        }

        public int GetPhaseIndex(VPhase phase)
        {
            return Phases.IndexOf(phase);
        }

        public VPhase GetPhase(int index)
        {
            return Phases[index];
        }
    }
}