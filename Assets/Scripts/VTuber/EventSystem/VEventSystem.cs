using System.Collections.Generic;
using UnityEngine;
using VTuber.Character;
using VTuber.Core.EventCenter;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using Yarn.Unity;

namespace VTuber.EventSystem
{
    public class VEventSystem : VMonoBehaviour
    {
        private VCharacter _character;
        
        [SerializeField] private DialogueRunner dialogueRunner;
        
        public void InitializeEvent(VCharacter character, string node)
        {
            _character = character;
            VDebug.Log(node);
            dialogueRunner.StartDialogue(node);
        }
        
        [YarnCommand("ApplyEffect")]
        public void ApplyEffect(uint id, string value)
        {
            var effect = VResourcesManager.Instance.CreateRaisingEffectByID(id, value);
            effect.ApplyEffect(_character);
        }
        
        public void OnDialogueComplete()
        {
            VRaisingRootEventCenter.Instance.Raise(VRaisingEventKey.OnEventEnd, new Dictionary<string, object>());
        }
    }
}