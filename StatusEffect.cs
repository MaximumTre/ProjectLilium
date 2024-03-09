using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kryz.CharacterStats;
using JetBrains.Annotations;

namespace SolarFalcon
{
    [System.Serializable]
    public class StatusEffects
    {
        public StatModifier
            WeakAdditive = new StatModifier(.20f, StatModType.PercentAdd),
            StrongAdditive = new StatModifier(.40f, StatModType.PercentAdd),
            ExtremeAdditive = new StatModifier(.60f, StatModType.PercentAdd),

            WeakSubtractive = new StatModifier(.20f, StatModType.PercentSub),
            StrongSubtractive = new StatModifier(.40f, StatModType.PercentSub),
            ExtremeSubtractive = new StatModifier(.60f, StatModType.PercentSub),
            
            PlusTen = new StatModifier(10, StatModType.Flat),
            PlusTwenty = new StatModifier(20, StatModType.Flat),
            PlusThirty = new StatModifier(30, StatModType.Flat),
            PlusForty = new StatModifier(40, StatModType.Flat),
            PlusFifty = new StatModifier(50, StatModType.Flat),
            PlusHundred = new StatModifier(100, StatModType.Flat),
            PoisonSub = new StatModifier(.30f, StatModType.PercentSub),
            CurseSubtractive = new StatModifier(0.5f, StatModType.PercentSub),
            BlessingAdditive = new StatModifier(0.5f, StatModType.PercentAdd);
    }
}
