﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Create GameData/Skill/Skill", order = -4989)]
    public partial class Skill : BaseSkill
    {
        public enum SkillAttackType : byte
        {
            None,
            Normal,
            BasedOnWeapon,
        }

        public enum SkillBuffType : byte
        {
            None,
            BuffToUser,
            BuffToNearbyAllies,
            BuffToNearbyCharacters,
            BuffToTarget,
            Toggle,
        }

        public SkillType skillType;

        [Header("Attack")]
        public SkillAttackType skillAttackType;
        public GameEffectCollection hitEffects;
        public DamageInfo damageInfo;
        public DamageIncremental damageAmount;
        public DamageEffectivenessAttribute[] effectivenessAttributes;
        public DamageInflictionIncremental[] weaponDamageInflictions;
        public DamageIncremental[] additionalDamageAmounts;
        [FormerlySerializedAs("increaseDamageWithBuffs")]
        public bool increaseDamageAmountsWithBuffs;
        public bool isDebuff;
        public Buff debuff;

        [Header("Buffs")]
        public SkillBuffType skillBuffType;
        public IncrementalFloat buffDistance;
        public Buff buff;

        [Header("Summon")]
        public SkillSummon summon;

        [Header("Mount")]
        public SkillMount mount;

        [Header("Craft")]
        public ItemCraft itemCraft;
        
        [System.NonSerialized]
        private Dictionary<Attribute, float> cacheEffectivenessAttributes;
        public Dictionary<Attribute, float> CacheEffectivenessAttributes
        {
            get
            {
                if (cacheEffectivenessAttributes == null)
                    cacheEffectivenessAttributes = GameDataHelpers.CombineDamageEffectivenessAttributes(effectivenessAttributes, new Dictionary<Attribute, float>());
                return cacheEffectivenessAttributes;
            }
        }

        public override bool Validate()
        {
            return GameDataMigration.MigrateBuffArmor(buff, out buff) ||
                GameDataMigration.MigrateBuffArmor(debuff, out debuff);
        }

        public override void ApplySkill(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand, CharacterItem weapon, int hitIndex, Dictionary<DamageElement, MinMaxFloat> damageAmounts, Vector3 aimPosition)
        {
            // Craft item
            if (skillType == SkillType.CraftItem &&
                skillUser is BasePlayerCharacterEntity)
            {
                BasePlayerCharacterEntity castedCharacter = skillUser as BasePlayerCharacterEntity;
                GameMessage.Type gameMessageType;
                if (!itemCraft.CanCraft(castedCharacter, out gameMessageType))
                    skillUser.gameManager.SendServerGameMessage(skillUser.ConnectionId, gameMessageType);
                else
                    itemCraft.CraftItem(castedCharacter);
                return;
            }

            // Apply skills only when it's active skill
            if (skillType != SkillType.Active)
                return;

            // Apply buff, summons at server only
            if (skillUser.IsServer)
            {
                ApplySkillBuff(skillUser, skillLevel);
                ApplySkillSummon(skillUser, skillLevel);
                ApplySkillMount(skillUser, skillLevel);
            }

            // Apply attack skill
            if (IsAttack())
            {
                if (skillUser.IsServer)
                {
                    // Increase damage with ammo damage
                    Dictionary<DamageElement, MinMaxFloat> increaseDamages;
                    skillUser.ReduceAmmo(weapon, isLeftHand, out increaseDamages);
                    if (increaseDamages != null)
                        damageAmounts = GameDataHelpers.CombineDamages(damageAmounts, increaseDamages);
                }

                // Launch damage entity to apply damage to other characters
                skillUser.LaunchDamageEntity(
                    isLeftHand,
                    weapon,
                    GetDamageInfo(skillUser, isLeftHand),
                    damageAmounts,
                    this,
                    skillLevel,
                    aimPosition,
                    Vector3.zero);
            }
        }

        protected void ApplySkillBuff(BaseCharacterEntity skillUser, short skillLevel)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;

            List<BaseCharacterEntity> tempCharacters;
            switch (skillBuffType)
            {
                case SkillBuffType.BuffToUser:
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    break;
                case SkillBuffType.BuffToNearbyAllies:
                    tempCharacters = skillUser.FindAliveCharacters<BaseCharacterEntity>(buffDistance.GetAmount(skillLevel), true, false, false);
                    foreach (BaseCharacterEntity applyBuffCharacter in tempCharacters)
                    {
                        applyBuffCharacter.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    }
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    break;
                case SkillBuffType.BuffToNearbyCharacters:
                    tempCharacters = skillUser.FindAliveCharacters<BaseCharacterEntity>(buffDistance.GetAmount(skillLevel), true, false, true);
                    foreach (BaseCharacterEntity applyBuffCharacter in tempCharacters)
                    {
                        applyBuffCharacter.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    }
                    skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    break;
                case SkillBuffType.BuffToTarget:
                    BaseCharacterEntity targetEntity;
                    if (skillUser.TryGetTargetEntity(out targetEntity) && !targetEntity.IsDead())
                        targetEntity.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    break;
                case SkillBuffType.Toggle:
                    int indexOfBuff = skillUser.IndexOfBuff(DataId, BuffType.SkillBuff);
                    if (indexOfBuff >= 0)
                        skillUser.Buffs.RemoveAt(indexOfBuff);
                    else
                        skillUser.ApplyBuff(DataId, BuffType.SkillBuff, skillLevel);
                    break;
            }
        }

        protected void ApplySkillSummon(BaseCharacterEntity skillUser, short skillLevel)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;
            int i = 0;
            int amountEachTime = summon.amountEachTime.GetAmount(skillLevel);
            for (i = 0; i < amountEachTime; ++i)
            {
                CharacterSummon newSummon = CharacterSummon.Create(SummonType.Skill, DataId);
                newSummon.Summon(skillUser, summon.level.GetAmount(skillLevel), summon.duration.GetAmount(skillLevel));
                skillUser.Summons.Add(newSummon);
            }
            int count = 0;
            for (i = 0; i < skillUser.Summons.Count; ++i)
            {
                if (skillUser.Summons[i].dataId == DataId)
                    ++count;
            }
            int maxStack = summon.maxStack.GetAmount(skillLevel);
            int unSummonAmount = count > maxStack ? count - maxStack : 0;
            CharacterSummon tempSummon;
            for (i = unSummonAmount; i > 0; --i)
            {
                int summonIndex = skillUser.IndexOfSummon(DataId, SummonType.Skill);
                tempSummon = skillUser.Summons[summonIndex];
                if (summonIndex >= 0)
                {
                    skillUser.Summons.RemoveAt(summonIndex);
                    tempSummon.UnSummon(skillUser);
                }
            }
        }

        protected void ApplySkillMount(BaseCharacterEntity skillUser, short skillLevel)
        {
            if (skillUser.IsDead() || !skillUser.IsServer || skillLevel <= 0)
                return;

            skillUser.Mount(mount.mountEntity);
        }

        protected DamageInfo GetDamageInfo(BaseCharacterEntity skillUser, bool isLeftHand)
        {
            switch (skillAttackType)
            {
                case SkillAttackType.Normal:
                    return damageInfo;
                case SkillAttackType.BasedOnWeapon:
                    return skillUser.GetWeaponDamageInfo(ref isLeftHand);
            }
            return default(DamageInfo);
        }

        public override SkillType GetSkillType()
        {
            return skillType;
        }

        public override bool IsAttack()
        {
            return skillAttackType != SkillAttackType.None;
        }

        public override bool IsBuff()
        {
            return skillType == SkillType.Passive || skillBuffType != SkillBuffType.None;
        }

        public override bool IsDebuff()
        {
            return IsAttack() && isDebuff;
        }

        public override float GetCastDistance(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            if (!IsAttack())
                return buffDistance.GetAmount(skillLevel);
            if (skillAttackType == SkillAttackType.Normal)
                return GetDamageInfo(skillUser, isLeftHand).GetDistance();
            return skillUser.GetAttackDistance(isLeftHand);
        }

        public override float GetCastFov(BaseCharacterEntity skillUser, short skillLevel, bool isLeftHand)
        {
            if (!IsAttack())
                return 360f;
            if (skillAttackType == SkillAttackType.Normal)
                return GetDamageInfo(skillUser, isLeftHand).GetFov();
            return skillUser.GetAttackFov(isLeftHand);
        }

        public override KeyValuePair<DamageElement, MinMaxFloat> GetBaseAttackDamageAmount(ICharacterData skillUser, short skillLevel, bool isLeftHand)
        {
            switch (skillAttackType)
            {
                case SkillAttackType.Normal:
                    return GameDataHelpers.MakeDamage(damageAmount, skillLevel, 1f, GetEffectivenessDamage(skillUser));
                case SkillAttackType.BasedOnWeapon:
                    return skillUser.GetWeaponDamage(ref isLeftHand);
            }
            return new KeyValuePair<DamageElement, MinMaxFloat>();
        }

        public override Dictionary<DamageElement, float> GetAttackWeaponDamageInflictions(ICharacterData skillUser, short skillLevel)
        {
            if (!IsAttack())
                return new Dictionary<DamageElement, float>();
            return GameDataHelpers.CombineDamageInflictions(weaponDamageInflictions, new Dictionary<DamageElement, float>(), skillLevel);
        }

        public override Dictionary<DamageElement, MinMaxFloat> GetAttackAdditionalDamageAmounts(ICharacterData skillUser, short skillLevel)
        {
            if (!IsAttack())
                return new Dictionary<DamageElement, MinMaxFloat>();
            return GameDataHelpers.CombineDamages(additionalDamageAmounts, new Dictionary<DamageElement, MinMaxFloat>(), skillLevel, 1f);
        }

        public override bool IsIncreaseAttackDamageAmountsWithBuffs(ICharacterData skillUser, short skillLevel)
        {
            return increaseDamageAmountsWithBuffs;
        }

        protected float GetEffectivenessDamage(ICharacterData skillUser)
        {
            return GameDataHelpers.GetEffectivenessDamage(CacheEffectivenessAttributes, skillUser);
        }

        public override sealed Buff GetBuff()
        {
            if (!IsBuff())
                return new Buff();
            return buff;
        }

        public override sealed Buff GetDebuff()
        {
            if (!IsDebuff())
                return new Buff();
            return debuff;
        }

        public override sealed SkillSummon GetSummon()
        {
            return summon;
        }

        public override sealed SkillMount GetMount()
        {
            return mount;
        }

        public override sealed ItemCraft GetItemCraft()
        {
            return itemCraft;
        }

        public override sealed GameEffectCollection GetHitEffect()
        {
            return hitEffects;
        }

        public override sealed bool RequiredTarget()
        {
            return skillBuffType == SkillBuffType.BuffToTarget;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacterEntities(new BaseCharacterEntity[] { summon.monsterEntity });
            GameInstance.AddMountEntities(new MountEntity[] { mount.mountEntity });
            GameInstance.AddItems(new Item[] { itemCraft.CraftingItem });
        }
    }
}
