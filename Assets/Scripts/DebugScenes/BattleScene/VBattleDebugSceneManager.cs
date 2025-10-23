using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DebugScenes.BattleScene.UI;
using SlayTheSpire.System.SavingSystem;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Serialization;
using VTuber.BattleSystem.Core;
using VTuber.BattleSystem.Core.UI;
using VTuber.Character;
using VTuber.CoopSystem;
using VTuber.Core.Foundation;
using VTuber.Core.Managers;
using VTuber.Core.ScriptSystem;
using VTuber.Core.StateMachine;
using VTuber.EventSystem;
using VTuber.Reincarnation;
using VTuber.Relic;
using VTuber.ScheduleSystem.Schedule;
using VTuber.ScheduleSystem.UI;
using VTuber.Store;
using VTuber.Store.UI;

namespace DebugScenes.BattleScene
{
    public class VBattleDebugSceneManager : VSingletonMonobehaviour<VBattleDebugSceneManager>
    {         
        [SerializeField] private List<VCooperatorConfiguration> cooperatorConfigurations;
        [SerializeField] private VReincarnationConfiguration reincarnationConfiguration;
        
        [Space(5)]
        [Header("Battle")]
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private VBattle battle;
        [SerializeField] private GameObject battleUI;

        [Space(5)] [Header("MainMenu")]
        [SerializeField] private AssetLabelReference scriptLabel;
        [SerializeField] private AssetLabelReference characterLabel;
        [SerializeField] VBattleDebugStartMenu startMenu;
        
        private List<VScriptConfiguration> _scripts;
        private List<VCharacterConfiguration> _characterConfigs;
        
        public VCharacter Character => _character;
        private VCharacter _character;
        private VStateMachine _stateMachine;
        private VScript _script;

        private bool _charactersLoaded;
        private bool _scriptsLoaded;

        protected override void Awake()
        {
            base.Awake();
            VDataLoader loader;

            loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/Cards.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Raising.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Relics.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Coop.xlsx"));
            
            _scripts = new List<VScriptConfiguration>();
            _characterConfigs = new List<VCharacterConfiguration>();
            Addressables.LoadAssetsAsync<VScriptConfiguration>(scriptLabel, scriptConfig =>
            {
                _scripts.Add(scriptConfig);
            }).Completed += handle =>
            {
                _scriptsLoaded = true;
                if (_scriptsLoaded && _charactersLoaded)
                {
                    startMenu.Initialize(_scripts, _characterConfigs);
                    _scriptsLoaded = false;
                    _charactersLoaded = false;
                }
            };
            
            Addressables.LoadAssetsAsync<VCharacterConfiguration>(characterLabel, characterConfig =>
            {
                _characterConfigs.Add(characterConfig);
            }).Completed += handle =>
            {
                _charactersLoaded = true;
                if (_scriptsLoaded && _charactersLoaded)
                {
                    startMenu.Initialize(_scripts, _characterConfigs);
                    _scriptsLoaded = false;
                    _charactersLoaded = false;
                }
            };
            
            VResourcesManager.Instance.LoadSprites();
            loader.Load();
            VResourcesManager.Instance.LoadDialogs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            VBattleRootEventCenter.Instance.RegisterListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        private void OnBattleEnd(Dictionary<string, object> messagedict)
        {
            startMenu.gameObject.SetActive(true);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            VBattleRootEventCenter.Instance.RemoveListener(VBattleEventKey.OnBattleEnd, OnBattleEnd);
        }

        public void StartBattle(bool isPhaseEnding, VCharacterConfiguration characterConfiguration, int initialTurnCount, List<VBattleRelic> relics)
        {
            startMenu.gameObject.SetActive(false);
            _character = new VCharacter(characterConfiguration);
            _character.Initialize(false);
         
            _character.CardLibrary.AddCards(VDataManager.Instance.CardConfigurations.Select(config => config.Value.CreateCard()).ToList());
            
            List<int> abilityTurnCounts = new List<int>();
            int avgTurn = initialTurnCount / 3;
            abilityTurnCounts.Add(avgTurn);
            abilityTurnCounts.Add(avgTurn);
            abilityTurnCounts.Add(initialTurnCount - 2 * avgTurn);
            battle.InitializeBattle(true, isPhaseEnding, _character.AttributeManager, _character.CardLibrary, initialTurnCount,
                0, abilityTurnCounts, null, 1000, 1000, 0, 0, relics);
        }
    }
}