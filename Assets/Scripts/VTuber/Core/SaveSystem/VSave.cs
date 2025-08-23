using System;
using System.Collections.Generic;
using VTuber.BattleSystem.Card;
using VTuber.Character;
using VTuber.Core.Managers;
using VTuber.Core.RaisingEffect;
using VTuber.Reincarnation;
using VTuber.Relic;

namespace VTuber.BattleSystem.Core.SaveSystem
{
    [Serializable]
    public class VSave
    {
        //5 Accounts in total
        //10-cards, 10-relics, 10-attributes
        public int accountCount;
        public int[,] accountData;
        public string[,] effectParameters;
        
        public VSave(VCharacter character)
        {
            accountData = new int[100, 30];
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    accountData[i, j] = -1;
                }
            }

            effectParameters = new string[100, 10];
            accountCount = character.Accounts.Count;
            for (int i = 0; i < character.Accounts.Count; i++)
            {
                var account = character.Accounts[i];
                for (int j = 0; j < account.Cards.Count; j++)
                {
                    accountData[i, j] = (int)account.Cards[j].configID;
                }
                
                for (int j = 0; j < account.Relics.Count; j++)
                {
                    accountData[i, j + 10] = (int)account.Relics[j].ConfigId;
                }
                
                for (int j = 0; j < account.Effects.Count; j++)
                {
                    accountData[i, j + 20] = (int)account.Effects[j].Id;
                }
            }
            
            for (int i = 0; i < character.Accounts.Count; i++)
            {
                var account = character.Accounts[i];
                for (int j = 0; j < account.Effects.Count; j++)
                {
                    effectParameters[i, j] = account.EffectItems[j].parameter;
                }
            }
        }

        public List<VAccount> LoadAccounts()
        {
            List<VAccount> accounts = new List<VAccount>();
            for (int i = 0; i < accountCount; i++)
            {
                List<VCard> cards = new List<VCard>();
                List<VRelic> relics = new List<VRelic>();
                List<VRaisingEffect> effects = new List<VRaisingEffect>();
                for (int j = 0; j < 10; j++)
                {
                    if (accountData[i, j] == -1)
                        break;
                    cards.Add(VDataManager.Instance.CreateCardByID((uint)accountData[i, j]));
                }
                
                for (int j = 10; j < 20; j++)
                {
                    if (accountData[i, j] == -1)
                        break;
                    relics.Add(VDataManager.Instance.CreateRelicByID((uint)accountData[i, j]));
                    
                }
                
                for (int j = 20; j < 30; j++)
                {
                    if (accountData[i, j] == -1)
                        break;
                    effects.Add(VDataManager.Instance.CreateRaisingEffectByID
                        ((uint)accountData[i, j], effectParameters[i, j], effectParameters[i, j]));
                }
                
                accounts.Add(new VAccount(cards, relics, effects));
            }
            return accounts;
        }
    }
}