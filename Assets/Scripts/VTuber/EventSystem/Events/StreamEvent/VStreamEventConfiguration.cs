using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Core.KPIs;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Core;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventHeaderIndex
    {    
        public const int TurnCount = 12;
        public const int Target = 13;
        public const int ExtraTarget = 14;
        public const int InitialViewers = 15;
        public const int MainAbility = 16;
        public const int AbilityTurns = 17;
        public const int SuccessEvent = 18;
        public const int FailEvent = 19;
        public const int AttributeBonus = 20;
        public const int KPI1 = 21;
        public const int KPI2 = 22;
        public const int KPI3 = 23;
    }
    
    public class VStreamEventConfiguration : VDialogueEventConfiguration
    {
        public int initialTurnCount;
        public int targetPopularity;
        public int extraTargetPopularity;
        public int attributeBonus;
        public int initialViewers;
        public int successEvent;
        public int failureEvent;
        public bool isPhaseEndingEvent = false;
        public int mainAttributeIndex;
        public List<int> abilityTurnCounts;
        public List<VKPI> kpis;

        public VStreamEventConfiguration(CellRange row) : base(row)
        {
            initialTurnCount = int.Parse(row.Columns[VStreamEventHeaderIndex.TurnCount].Value);
            targetPopularity = int.Parse(row.Columns[VStreamEventHeaderIndex.Target].Value);
            
            var extraTargetPopularityStr = row.Columns[VStreamEventHeaderIndex.ExtraTarget].Value;
            if (!extraTargetPopularityStr.IsNullOrWhitespace())
                extraTargetPopularity = int.Parse(row.Columns[VStreamEventHeaderIndex.ExtraTarget].Value);
            
            initialViewers = int.Parse(row.Columns[VStreamEventHeaderIndex.InitialViewers].Value);
            
            mainAttributeIndex = int.Parse(row.Columns[VStreamEventHeaderIndex.MainAbility].Value);
            string abilityTurnsStr = row.Columns[VStreamEventHeaderIndex.AbilityTurns].Value;
            abilityTurnCounts = abilityTurnsStr.Split(',').Select(int.Parse).ToList();
            
            var successEventStr = row.Columns[VStreamEventHeaderIndex.SuccessEvent].Value;
            var failureEventStr = row.Columns[VStreamEventHeaderIndex.FailEvent].Value;
            attributeBonus = int.Parse(row.Columns[VStreamEventHeaderIndex.AttributeBonus].Value);
            successEvent = successEventStr.IsNullOrWhitespace() ? -1 : int.Parse(successEventStr);
            failureEvent = failureEventStr.IsNullOrWhitespace() ? -1 : int.Parse(failureEventStr);

            kpis = new List<VKPI>();
            for (int i = VStreamEventHeaderIndex.KPI1; i <= VStreamEventHeaderIndex.KPI3; i += 1)
            {
                var kpiStr = row.Columns[i].Value;
                if (kpiStr.IsNullOrWhitespace())
                    continue;
                var kpiParams = kpiStr.Split(',').ToList();
                var kpiType = Enum.Parse<VEventType>(kpiParams[0]);
                if (kpiType == VEventType.Stream)
                {
                    kpis.Add(new VKPI(kpiType, int.Parse(kpiParams[2]), int.Parse(kpiParams[1]), false));
                }
                else
                {
                    kpis.Add(new VKPI(kpiType, int.Parse(kpiParams[1]), -1, false));
                }
            }
            isPhaseEndingEvent = kpis.Count > 0;
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VStreamEvent(this);
        }
    }
}