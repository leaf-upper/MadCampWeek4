﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillUIController : MonoBehaviour
{
    [SerializeField]
    private SkillUI[] skillUi;

    public void UseSkill(SkillType type, float coolTime)
    {
        int target = (int)type;
        skillUi[target].UseSkill(coolTime);
    }

    public void updateAmmo(SkillType type, int ammo)
    {
        int target = (int)type;
        skillUi[target].updateAmmo(ammo);
    }

    public void DisableAmmoText(SkillType type)
    {
        int target = (int)type;
        skillUi[target].DisableAmmo();
    }

    public enum SkillType
    {
        Attack = 0,
        Skill = 1,
        MakeWall = 2,
        Trap = 3
    }

}
