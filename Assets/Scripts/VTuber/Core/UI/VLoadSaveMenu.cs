using System.Linq;
using SlayTheSpire.System.SavingSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VTuber.Core.Foundation;
using VTuber.Core.UI;

namespace VTuber.BattleSystem.Core.UI
{
    public class VLoadSaveMenu : VUIBehaviour
    {
        [SerializeField] public Button confirmButton;
        [SerializeField] public Button returnButton;
        [SerializeField] public Button deleteSaveButton;
        [SerializeField] private TMP_Text scriptName;
        [SerializeField] private Image scriptImage;
        [SerializeField] private TMP_Text characterName;
        [SerializeField] private TMP_Text currentPhase;
        [SerializeField] private TMP_Text currentWeek;
        [SerializeField] private TMP_Text lastPlayTime;
        [SerializeField] private VAbilityDetails abilityDetails;

        public void SetDetails(SaveData saveData)
        {
            var scriptConfig = VGameManager.Instance.GetScriptConfig(saveData.script.scriptConfigurationName);
            scriptName.text = scriptConfig.scriptName;
            scriptImage.sprite = scriptConfig.icon;

            var characterConfig =
                VGameManager.Instance.GetCharacterConfig(saveData.characterSaveData.characterConfigurationName);
            characterName.text = characterConfig.name;

            currentPhase.text = "当前阶段: " + scriptConfig.phases[saveData.script.currentPhaseIndex].phaseName;
            currentWeek.text = saveData.stateMachine.weekIndex + "/" +
                               (scriptConfig.phases.Last().endEventWeekIndex + 1) + "周";

            lastPlayTime.text = VStringUtils.GetTime(saveData.lastPlayTime);

            abilityDetails.SetDetails(saveData.characterSaveData);
        }
    }
}