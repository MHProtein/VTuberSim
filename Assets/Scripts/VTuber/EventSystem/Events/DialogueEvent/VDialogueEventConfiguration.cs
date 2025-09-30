using System;
using System.Collections.Generic;
using Sirenix.Utilities;
using Spire.Xls;
using VTuber.BattleSystem.Card;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;

namespace VTuber.ScheduleSystem.Events.DialogueEvent
{
    public class VStreamEventHeaderIndex
    {
        public const int DialogueNode = 11;
        public const int Effect1 = 12;
        public const int Effect5Param = 21;
    }
    
    public class VDialogueEventConfiguration : VScheduleEventConfiguration
    {
        public readonly string dialogueNode;
        public readonly List<VRaisingEffect> effects;

        public VDialogueEventConfiguration(CellRange row) : base(row)
        {
            dialogueNode = row.Columns[VStreamEventHeaderIndex.DialogueNode].Value;
            effects = new List<VRaisingEffect>();
            if (!dialogueNode.IsNullOrWhitespace())
                return;
            for (int i = VStreamEventHeaderIndex.Effect1; i <= VStreamEventHeaderIndex.Effect5Param; i += 2)
            {
                var effectIDStr = row.Columns[i].Value;
                if(effectIDStr.IsNullOrWhitespace())
                    continue;
                uint effect = Convert.ToUInt32(effectIDStr);
                effects.Add(VDataManager.Instance.CreateRaisingEffectByID(effect, row.Columns[i + 1].Value, row.Columns[i + 1].Value));
            }
        }

        public override VScheduleEvent CreateEvent()
        {
            return new VDialogueEvent(this);
        }
    }
}