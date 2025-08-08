// using UnityEngine;
// using Yarn.Unity;
// using System.Collections.Generic;
// using VTuber.BattleSystem.Card;
// using VTuber.Character;
// using VTuber.Core.Foundation;
// using VTuber.Core.Managers;
//
// namespace VTuber.Dialogue
// {
//     public class YarnBridge : VMonoBehaviour
//     {
//
//         [SerializeField] private VCharacterConfiguration _characterConfiguration;
//         private VCharacter _character;
//         protected override void Awake()
//         {
//             base.Awake();            
//             VResourcesLoader loader = new VResourcesLoader(@"Assets\Resources\Configurations\NewCards.xlsx");
//             _character = new VCharacter(_characterConfiguration);
//             var cardConfigs = loader.Load();
//             List<VCard> cards = new List<VCard>();
//
//             foreach (var cardConfig in cardConfigs)
//             {
//                 for (int i = 0; i < 2; i++)
//                 {
//                     var card = cardConfig.CreateCard();
//                     if(card is not null)
//                         cards.Add(card);
//                 }
//             }
//             _character.CardLibrary.AddCards(cards);
//         }
//
//         private Dictionary<string, string> messageDictionary = new Dictionary<string, string>
//         {
//             { "Greeting", "Hello, hero!" },
//             { "Farewell", "Goodbye, traveler." }
//         };
//
//
//         
//         [YarnCommand("leap")]
//         public void Leap() {
//             Debug.Log($"{name} is leaping!");
//         }
//
//         [YarnFunction("say_message")]
//         public static string GetMessage(string key)
//         {
//             // 静态 function，返回字典对应值
//             Dictionary<string, string> messages = new Dictionary<string, string>
//             {
//                 { "Greeting", "Welcome to the world of Yarn!" },
//                 { "Warning", "Low stamina detected!" },
//                 { "Victory", "You have won the battle!" }
//             };
//
//             if (messages.TryGetValue(key, out var value))
//             {
//                 return value;
//             }
//
//             return $"[Unknown message key: {key}]";
//         }
//
//         public VCharacter currentCharacter;
//
//         [YarnCommand("add_stamina")]
//         public void AddStamina(int amount)
//         {
//         //     if (currentCharacter.AttributeManager.TryGetAttribute("CAStamina", out var stamina))
//         //     {
//         //         stamina.AddValue(amount);
//         //         Debug.Log($"角色体力增加：{amount}");
//         //     }
//             VDebug.Log("addstamina"+amount);
//         }
//
//         [YarnCommand("set_pressure")]
//         public void SetPressure(int value)
//         {
//         //     if (currentCharacter.AttributeManager.TryGetAttribute("CAPressure", out var pressure))
//         //     {
//         //         pressure.SetValue(value);
//         //         Debug.Log($"角色压力设置为：{value}");
//         //     }
//         VDebug.Log("pressure");
//         }
//         
//     }
// }
