﻿using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public interface IDamageableEntity : IGameEntity
    {
        int CurrentHp { get; set; }
        Transform OpponentAimTransform { get; }
        bool IsDead();
        void ReceiveDamage(IAttackerEntity attacker, CharacterItem weapon, Dictionary<DamageElement, MinMaxFloat> damageAmounts, BaseSkill skill, short skillLevel);
        bool CanReceiveDamageFrom(IAttackerEntity attacker);
        void PlayHitEffects(IEnumerable<DamageElement> damageElements, BaseSkill skill);
    }
}