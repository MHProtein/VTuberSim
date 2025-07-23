using UnityEngine;
using Yarn.Unity;
using System.Collections.Generic;
using VTuber.Core.Foundation;
namespace VTuber.Dialogue
{
    public class YarnBridge : VMonoBehaviour
    {
        public PlayerStatus playerStatus;

        private Dictionary<string, string> messageDictionary = new Dictionary<string, string>
        {
            { "Greeting", "Hello, hero!" },
            { "Farewell", "Goodbye, traveler." }
        };

        [YarnCommand("add_exp")]
        public void AddExperienceCommand(string expString)
        {
            Debug.Log($"starting add_exp!");
            if (int.TryParse(expString, out int exp))
            {
                playerStatus.AddExperience(exp);
            }
            else
            {
                Debug.LogWarning($"Invalid EXP value: {expString}");
            }
        }
        [YarnCommand("leap")]
        public void Leap() {
            Debug.Log($"{name} is leaping!");
        }
        
        [YarnFunction("say_message")]
        public static string GetMessage(string key)
        {
            // 静态 function，返回字典对应值
            Dictionary<string, string> messages = new Dictionary<string, string>
            {
                { "Greeting", "Welcome to the world of Yarn!" },
                { "Warning", "Low stamina detected!" },
                { "Victory", "You have won the battle!" }
            };

            if (messages.TryGetValue(key, out var value))
            {
                return value;
            }

            return $"[Unknown message key: {key}]";
        }
    }




}
