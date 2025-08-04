using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.ScheduleSystem.Events;

namespace VTuber.Dialogue
{
    public class YarnScheduleEvent : VScheduleEvent
    {
        private DialogueRunner _dialogueRunner;
        private string _yarnNodeName;

        public YarnScheduleEvent(VScheduleEventConfiguration config, DialogueRunner dialogueRunner, string yarnNodeName)
            : base(config)
        {
            _dialogueRunner = dialogueRunner;
            _yarnNodeName = yarnNodeName;
        }

        public override bool Execute(VCharacter player)
        {
            if (!CanExecute(player))
            {
                Debug.LogWarning($"无法执行事件：{EventName}，体力不足");
                return false;
            }

            Debug.Log($"开始执行 Yarn 对话节点：{_yarnNodeName}");

            // 绑定完成回调
            _dialogueRunner.onDialogueComplete.AddListener(OnDialogueComplete);
            _dialogueRunner.StartDialogue(_yarnNodeName);
            return true;
        }

        private void OnDialogueComplete()
        {
            _dialogueRunner.onDialogueComplete.RemoveListener(OnDialogueComplete);
            ApplyRewards(); // 可选：对角色加属性、扣体力等
            IsExecuted = true;
            GetNextEvent();
        }

        private void ApplyRewards()
        {
            Debug.Log("奖励已应用");
        }
    }
}
