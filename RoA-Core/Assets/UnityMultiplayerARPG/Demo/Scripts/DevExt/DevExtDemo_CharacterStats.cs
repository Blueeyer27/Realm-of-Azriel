using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial struct CharacterStats
    {
        [Header("Demo Developer Extension")]
        public float testStats;

        [DevExtMethods("Add")]
        public CharacterStats DevExtDemo_Add(CharacterStats b)
        {
            testStats = testStats + b.testStats;
            return this;
        }

        [DevExtMethods("Multiply")]
        public CharacterStats DevExtDemo_Multiply(float multiplier)
        {
            testStats = testStats * multiplier;
            return this;
        }

        [DevExtMethods("MultiplyStats")]
        public CharacterStats DevExtDemo_MultiplyStats(CharacterStats b)
        {
            testStats = testStats * b.testStats;
            return this;
        }
    }
}
