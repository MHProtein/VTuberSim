using System;
using System.Collections.Generic;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Dialogue.UI;
using VTuber.ScheduleSystem.Events;
using VTuber.ScheduleSystem.Events.DialogueEvent;
using Yarn.Unity;

namespace VTuber.EventSystem
{
    public class VEventSystem : VMonoBehaviour
    {
        private VCharacter _character;
        private VDialogueEvent _currentEvent;
        
        [SerializeField] private DialogueRunner dialogueRunner;
        
        public void InitializeEvent(VCharacter character, VDialogueEvent e)
        {
            _character = character;
            _currentEvent = e;
            VDebug.Log(e.dialogueNode);
            dialogueRunner.StartDialogue(e.dialogueNode);
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RegisterListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnSelectPhaseEndingBegin, OnPickPhaseEndingBegin);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCardFrom3, OnBeginSelectCardFrom3);
            VRaisingRootEventCenter.Instance.RemoveListener(VRaisingEventKey.OnBeginSelectCard, OnBeginSelectCard);
        }
        
        private void OnBeginSelectCardFrom3(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenSelectFrom3Menu(messagedict["Cards"] as List<VCard>, messagedict["Action"] as Action<VCard>);
        }
        
        private void OnBeginSelectCard(Dictionary<string, object> messagedict)
        {
            VEventSystemUI.Instance.OpenCardLibrary(_character.CardLibrary.GetCards(), true, messagedict["Action"] as Action<VCard>);
        }
        
        private void OnPickPhaseEndingBegin(Dictionary<string, object> messagedict)
        {
            if (_currentEvent.Phase == null)
            {
                VDebug.LogError("_currentEvent.Phase ist null, lass es reparieren");
                return;
            }
            VEventSystemUI.Instance.InitializePhaseEndingSelectionMenu(_currentEvent.Phase.GetPhaseEndingEvents(_character));
        }

        [YarnCommand("ApplyEffect")]
        public void ApplyEffect(uint id, string value)
        {
            var effect = VResourcesManager.Instance.CreateRaisingEffectByID(id, value, value);
            effect.ApplyEffect(_character);
        }

        public void OnDialogueComplete()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, new Dictionary<string, object>());
            _currentEvent = null;
        }
    }
}