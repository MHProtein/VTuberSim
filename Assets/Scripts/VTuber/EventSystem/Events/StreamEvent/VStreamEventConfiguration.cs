using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using Spire.Xls;
using UnityEngine.Serialization;
using VTuber.Core.Managers;
using VTuber.ScheduleSystem.Events.DialogueEvent;

namespace VTuber.ScheduleSystem.Events
{
    public class VStreamEventHeaderIndex
    {    
        public const int TurnCount = 11;
        public const int Target = 12;
        public const int ExtraTarget = 13;
        public const int InitialViewers = 14;
        public const int MainAbility = 15;
        public const int AbilityTurns = 16;
        public const int SuccessEvent = 17;
        public const int FailEvent = 18;
        public const int AttributeBonus = 19;
        public const int PhaseEndingConditions = 20;
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
        public List<VPhaseEndingCondition> phaseEndingConditions;

        public VStreamEventConfiguration(CellRange row) : base(row)
        {
            initialTurnCount = int.Parse(row.Columns[VStreamEventHeaderIndex.TurnCount].Value);
            targetPopularity = int.Parse(row.Columns[VStreamEventHeaderIndex.Target].Value);
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
            phaseEndingConditions = new List<VPhaseEndingCondition>();
            string phaseEndingConditionsStr = row.Columns[VStreamEventHeaderIndex.PhaseEndingConditions].Value;
            if (!phaseEndingConditionsStr.IsNullOrWhitespace())
            {
                string[] conditions = phaseEndingConditionsStr.Split(',');
                isPhaseEndingEvent = true;
                foreach (var condition in conditions)
                {
                    if (int.TryParse(condition, out int conditionID))
                    {
                        if (conditionID == -1)
                        {
                            phaseEndingConditions.Clear();
                            break;
                        }
                        else
                        {
                            phaseEndingConditions.Add(VResourcesManager.Instance.GetPhaseEndingConditionByID((uint)conditionID));
                        }
                    }
                }
            }
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VStreamEvent(this);
        }
    }
}