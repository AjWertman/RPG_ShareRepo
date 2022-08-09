using RPGProject.Combat;
using System;
using System.Collections.Generic;
using UnityEngine;

public static class AIBehavior
{
    public static void PlanMove(Fighter _fighter)
    {
        List<Ability> abilities = _fighter.GetKnownAbilities();

        List<Ability> usableAbilities = GetUsableAbilities(_fighter, abilities);

    }

    private static List<Ability> GetUsableAbilities(Fighter _fighter, List<Ability> abilities)
    {
        List<Ability> usableAbilities = new List<Ability>();



        return usableAbilities;
    }
}
