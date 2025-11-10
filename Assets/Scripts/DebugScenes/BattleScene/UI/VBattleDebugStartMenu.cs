using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Character;
using VTuber.Core.Foundation;
using VTuber.Core.ScriptSystem;

namespace DebugScenes.BattleScene.UI
{
    public class VBattleDebugStartMenu : VUIBehaviour
    {
        [SerializeField] private TMP_Dropdown characterDropdown;
        [SerializeField] private TMP_Dropdown scriptDropdown;
        [SerializeField] private TMP_InputField turnCount;
        [SerializeField] private Toggle isMultiplyMultiplier;
        [SerializeField] private Button startBattleButton;
        [SerializeField] private VBattleDebugRelicSelection relicSelection;
        private List<VCharacterConfiguration> _characterConfig;

        private List<VScriptConfiguration> _scriptConfig;

        protected override void Awake()
        {
            base.Awake();
            startBattleButton.onClick.AddListener(StartBattle);
            turnCount.text = "10";
        }

        public void Initialize(List<VScriptConfiguration> scriptConfigs,
            List<VCharacterConfiguration> characterConfigs)
        {
            _scriptConfig = scriptConfigs;
            _characterConfig = characterConfigs;

            foreach (var vCharacterConfiguration in characterConfigs)
                characterDropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = vCharacterConfiguration.characterName
                });
            characterDropdown.RefreshShownValue();


            foreach (var vScriptConfiguration in scriptConfigs)
                scriptDropdown.options.Add(new TMP_Dropdown.OptionData
                {
                    text = vScriptConfiguration.scriptName
                });
            scriptDropdown.RefreshShownValue();

            relicSelection.Initialize();
        }

        public void StartBattle()
        {
            VBattleDebugSceneManager.Instance.StartBattle(isMultiplyMultiplier.isOn,
                _characterConfig.Find(character =>
                    character.characterName == characterDropdown.options[characterDropdown.value].text),
                int.Parse(turnCount.text),
                relicSelection.GetSelected()
            );
        }
    }
}