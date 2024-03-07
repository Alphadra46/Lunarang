using System;
using System.Collections;
using Enum;
using UnityEngine;
using Random = UnityEngine.Random;

public class SC_DoT_States
{
    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="applicator"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    public IEnumerator PoisonDoT(SC_DebuffsBuffsComponent applicator, SC_DebuffsBuffsComponent self)
    {
          
        var duration = (applicator.poisonDuration * (1 + (applicator.dotDurationBonus / 100)));
        
        self.poisonCurrentStacks += applicator.poisonStackByHit;
        
        yield return new WaitForSeconds(applicator.poisonTick);
        
        while (duration > 0)
        {

            Debug.Log("Poison Tick");
            Debug.Log("Poison Stack :" + self.poisonCurrentStacks);
            
            for (var i = 0; i < self.poisonCurrentStacks; i++)
            {
                var rawDamage = (applicator.isPlayer ? applicator._playerStats.currentStats.currentATK : applicator._aiStats.currentStats.currentATK) * 0.1f;
        
                var effDamage = Mathf.Round(rawDamage) * (1 + (applicator.poisonDMGBonus + (applicator.isPlayer ? applicator._playerStats.currentStats.dotDamageBonus : 0))/100);
                Debug.Log(effDamage);
        
                var effCrit = effDamage * (1 + (applicator.dotCritDamage/100));
                
                var isCritical = Random.Range(0, 100) < applicator.dotCritRate ? true : false;
                self.GetComponent<IDamageable>().TakeDoTDamage(isCritical ? effCrit : effDamage, isCritical, Enum_Debuff.Poison);
            }
            
            duration -= applicator.poisonTick;
            
            // Debug.Log("Duration :" + duration);
            
            yield return new WaitForSeconds(applicator.poisonTick);
        }

        self.poisonCurrentStacks = 0;
        self.currentDebuffs.Remove(Enum_Debuff.Poison);
        if(self._modifierPanel != null) self._modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Poison);

    }
    
    /// <summary>
    /// Coroutine for the poison debuff, apply damage every ticks during a certain duration.
    /// </summary>
    /// <param name="applicator"></param>
    /// <returns></returns>
    public IEnumerator FrozenState(SC_DebuffsBuffsComponent applicator, SC_DebuffsBuffsComponent self)
    {

        var duration = (applicator.freezeDuration * 
                        (1 + (applicator.freezeDurationBonus / 100)));

        // Bonus from Skills when Frozen
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Freeze"))
        {

            self._aiStats.currentStats.damageTaken += float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_2_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_4_Freeze"))
        {

            self._aiStats.currentStats.damageTaken += float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_2_Freeze"))
        {
            var newStatModifier = new SC_StatModification
            {
                StatToModify = StatTypes.ATK,
                ModificationType = StatModificationType.Numerical,
                ModificationValue = float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_2_Freeze")
                    .buffsParentEffect["atkBonus"]),
                timer = float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_2_Freeze")
                    .buffsParentEffect["atkBonusDuration"])
            };


            applicator.StartCoroutine(applicator.BuffStatTemp(newStatModifier)
            );

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_3_4_Freeze"))
        {

            var newStatModifier = new SC_StatModification
            {
                StatToModify = StatTypes.ATKSPD,
                ModificationType = StatModificationType.Numerical,
                ModificationValue = float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_4_Freeze")
                    .buffsParentEffect["atkSpdBonus"]),
                timer = float.Parse(SC_SkillManager.instance
                    .FindChildSkillByName("ChildSkill_3_4_Freeze")
                    .buffsParentEffect["atkSpdBonusDuration"])
            };
            
            applicator.StartCoroutine(applicator.BuffStatTemp(newStatModifier)
            );

        }

        // Freeze Effect
        if (self.isPlayer)
        {
            SC_PlayerController.instance.FreezeMovement(true);
            SC_PlayerController.instance.FreezeDash(true);
        }
        else
        {
            if(self._aiStats.TryGetComponent(out AI_StateMachine stateMachine))
                stateMachine.TransitionToState(AI_StateMachine.EnemyState.Freeze);
        }
        
        yield return new WaitForSeconds(duration);

        // Bonus from Skills when Unfrozen
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("Fracture Glaciaire"))
        {

            Debug.Log("AOOOOE");
            
            var rawDamage = MathF.Round((applicator.unfreezeAoEMV/100) * applicator._playerStats.currentStats.currentATK, MidpointRounding.AwayFromZero);
            var effDamage = rawDamage * (1 + (applicator._playerStats.currentStats.dotDamageBonus / 100));
            var effCrit = effDamage * (1 + (applicator.dotCritDamage / 100));

            var pos = new Vector3(self.transform.position.x, 0.4f, self.transform.position.z);
            
            var ennemiesInAoE =
                Physics.OverlapSphere(pos, applicator.unfreezeAoESize,
                    SC_ComboController.instance.layerAttackable);

            foreach (var e in ennemiesInAoE)
            {
                if (!e.TryGetComponent(out IDamageable damageable)) continue;
                var isCritical = Random.Range(0, 100) < applicator.dotCritRate ? true : false;
                damageable.TakeDoTDamage(isCritical ? effCrit : effDamage, isCritical, Enum_Debuff.Freeze);

                if(SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_1_Freeze") && 
                   Random.Range(1, 100) < float.Parse(SC_SkillManager.instance
                       .FindChildSkillByName("ChildSkill_1_1_Freeze")
                       .buffsParentEffect["freezeHitRate"]))
                {
                    e.GetComponent<SC_DebuffsBuffsComponent>().ApplyDebuff(Enum_Debuff.Freeze, applicator);
                }
                
            }
            
        }

        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_1_2_Freeze"))
        {

            var newStatModifier = new SC_StatModification
            {
                StatToModify = StatTypes.DMGTaken,
                ModificationType = StatModificationType.Percentage,
                ModificationValue = float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Freeze").buffsParentEffect["dmgTaken"]),
                timer = float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_1_2_Freeze").buffsParentEffect["duration"])
            };
            
            self.StartCoroutine(self.BuffStatTemp(newStatModifier));

        }

        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("Immobilisation Glaciale"))
        {
            self.ApplyDebuff(Enum_Debuff.Slowdown, applicator);
        }
        
        
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_2_Freeze"))
        {

            self._aiStats.currentStats.damageTaken -= float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        if (applicator.isPlayer && SC_SkillManager.instance.CheckHasSkillByName("ChildSkill_2_4_Freeze"))
        {

            self._aiStats.currentStats.damageTaken -= float.Parse(SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_4_Freeze")
                .buffsParentEffect["dmgTaken"]);

        }
        
        // Unfreeze Effect
        if (self.isPlayer)
        {
            SC_PlayerController.instance.FreezeMovement(false);
            SC_PlayerController.instance.FreezeDash(false);
        }
        else
        {
            if(self._aiStats.TryGetComponent(out AI_StateMachine stateMachine))
                stateMachine.TransitionToState(AI_StateMachine.EnemyState.Patrol);
        }
        
        self.currentDebuffs.Remove(Enum_Debuff.Freeze);
        if(self._modifierPanel != null) self._modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Freeze);

    }
    
    public IEnumerator Slowdown(SC_DebuffsBuffsComponent applicator, SC_DebuffsBuffsComponent self)
    {

        var duration = applicator.slowdownDuration * (1 + ((SC_SkillManager.instance.FindChildSkillByName("ChildSkill_2_3_Freeze").buffsParentEffect
            .TryGetValue("slowdownDurationBonus", out var value))
            ? float.Parse(value)
            : 0) / 100);
        

        if (applicator.isPlayer)
        {
            self._aiStats.currentStats.speedModifier += -30;
            self._aiStats.currentStats.atkSpeedModifier += -30;
        }
        else
        {
            self._playerStats.currentStats.speedModifier += -30;
            self._playerStats.currentStats.atkSpeedModifier += -30;
        }
        
        yield return new WaitForSeconds(duration);
        
        if (applicator.isPlayer)
        {
            self._aiStats.currentStats.speedModifier -= -30;
            self._aiStats.currentStats.atkSpeedModifier -= -30;
        }
        else
        {
            self._playerStats.currentStats.speedModifier -= -30;
            self._playerStats.currentStats.atkSpeedModifier -= -30;
        }
        
        self.currentDebuffs.Remove(Enum_Debuff.Slowdown);
        if(self._modifierPanel != null) self._modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Slowdown);
        
    }

    public void Burn(SC_DebuffsBuffsComponent applicator, SC_DebuffsBuffsComponent self)
    {
        
        if(self.burnCurrentStacks < self.burnMaxStack)
        {
            switch (self.burnHitToProc)
            {
                case > 0:
                    self.burnHitToProc--;
                    return;
                case 0:
                    self.StartCoroutine(BurnDoT(applicator, self));
                    self.burnHitToProc = applicator.burnHitRequired-1;

                    self.burnCurrentStacks++;
                    break;
            }
        }
        else if (self.burnCurrentStacks == self.burnMaxStack)
        {
            self.burnCurrentStacks = 0;
            self.burnHitToProc = 0;
            self.currentDebuffs.Remove(Enum_Debuff.Burn);
            if(self._modifierPanel != null) self._modifierPanel.debuffRemoved?.Invoke(Enum_Debuff.Burn);
        }
        
    }

    public IEnumerator BurnDoT(SC_DebuffsBuffsComponent applicator, SC_DebuffsBuffsComponent self)
    {

        var duration = applicator.burnTick;
        
        SC_ComboController.instance.CreateAoE(new Vector3(self.transform.position.x, self.transform.localScale.y, self.transform.position.z), applicator.burnAoESize, (applicator.burnAoEMV/100), true);

        while (duration > 0)
        {
            var rawDamage = (applicator.isPlayer ? applicator._playerStats.currentStats.currentATK : applicator._aiStats.currentStats.currentATK) *
                            (applicator.burnDoTMV/100);
        
            var effDamage = Mathf.Round(rawDamage) * (1 + (applicator.burnDMGBonus + (applicator.isPlayer ? applicator._playerStats.currentStats.dotDamageBonus : 0))/100);
            Debug.Log(effDamage);
        
            var effCrit = effDamage * (1 + (applicator.dotCritDamage/100));
                
            var isCritical = Random.Range(0, 100) < applicator.dotCritRate ? true : false;
            self.GetComponent<IDamageable>().TakeDoTDamage(isCritical ? effCrit : effDamage, isCritical, Enum_Debuff.Burn);
            
            duration -= applicator.burnTick/4.5f;
            
            yield return new WaitForSeconds(applicator.burnTick/4.5f);
        }
        
        
    }
    
}
