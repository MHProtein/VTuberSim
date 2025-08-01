using System.Collections.Generic;
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
            
            VResourcesLoader loader = new VResourcesLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
            character = new VCharacter(_characterConfiguration);
            var cardConfigs = loader.Load();
            List<VCard> cards = new List<VCard>();

            foreach (var cardConfig in cardConfigs)
            {
                for (int i = 0; i < 2; i++)
                {
                    var card = cardConfig.CreateCard();
                    if(card is not null)
                        cards.Add(card);
                }
            }
            character.CardLibrary.AddCards(cards);
            InitializeBattle();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        public void InitializeBattle()
        {
            //battleRoot.SetActive(true);
            _battle.InitializeBattle(character.AttributeManager, character.CardLibrary, 10);
        }

        protected override void Start()
        {
            base.Start();
        }
    }
}