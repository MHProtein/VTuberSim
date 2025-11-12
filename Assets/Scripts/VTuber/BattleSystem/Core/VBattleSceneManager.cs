using System.Collections.Generic;
using System.IO;
using UnityEngine;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Foundation;

namespace VTuber.BattleSystem.Core
{
    public class VBattleSceneManager : VMonoBehaviour

    {
        [SerializeField] private GameObject battleRoot;
        [SerializeField] private VBattle _battle;
        [SerializeField] private VCharacterConfiguration _characterConfiguration;
        private VCharacter character;

        protected override void Awake()
        {
            base.Awake();

            var loader = new VDataLoader(Path.Combine(Application.streamingAssetsPath, "Configurations/Cards.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Raising.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Relics.xlsx"),
                Path.Combine(Application.streamingAssetsPath, "Configurations/Coop.xlsx"));
            character = new VCharacter(_characterConfiguration);
            var cardConfigs = loader.Load();
            var cards = new List<VCard>();

            foreach (var cardConfig in cardConfigs)
                for (var i = 0; i < 2; i++)
                {
                    var card = cardConfig.CreateCard();
                    if (card is not null)
                        cards.Add(card);
                }

            character.CardLibrary.AddCards(cards);
            InitializeBattle();
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public void InitializeBattle()
        {
            //battleRoot.SetActive(true);
            //_battle.InitializeBattle(false, character.AttributeManager,
            //character.CardLibrary, 10, 0, new List<int> {2, 6, 2}, 0, 0, new List<VBattleRelic>());
        }
    }
}