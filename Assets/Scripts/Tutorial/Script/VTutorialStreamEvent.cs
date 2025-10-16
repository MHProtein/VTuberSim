using System.Collections.Generic;
using NUnit.Framework;
using VTuber.BattleSystem.Effect.Conditions;
using VTuber.Core.Foundation;

namespace Tutorial.Script
{
    public class VTutorialStreamEventConfiguration : VScriptableObject
    {
        public uint baseEventID;
        public List<VAttributeCondition> conditions;
        public List<uint> deck;
        public Dictionary<int, List<uint>> turnHandCards;
    }
}