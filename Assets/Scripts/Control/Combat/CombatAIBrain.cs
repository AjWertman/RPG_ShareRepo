using RPGProject.Combat;
using RPGProject.Core;
using RPGProject.GameResources;
using System.Collections.Generic;
using UnityEngine;

namespace RPGProject.Combat
{
    public class CombatAIBrain : MonoBehaviour
    {
        CombatAIBehavior[] combatAIBehaviors = null;

        List<Ability> abilities = new List<Ability>();
        public void PlanNextMove(List<Fighter> _allFighters)
        {
            //Fighter randomTarget = GetRandomTarget();
            //if (randomTarget == null) return;

            //Health targetHealth = randomTarget.GetHealthComponent();

            //How many AP to spend to get in attack range? 
            //Is target below X percentage of health? Can I kill them on this turn? 
            //What is the best move for me to use on my target? Would my best moves be overkill?
            //Do I know my targets strengths/weaknesses? Can I exploit them?

            //Whats my role in combat? 
            ///IDEA - create different behaviors (Tank, mDamage, rDamage, Healer/Support)
            ///Each has a percentage of different actions (based on combatAIBrain.GetRandomTarget()) and randomly executes each one
            ///Should above list be Dynamically changing based on battle conditions?
            ///

            //Do any of my teammates need support?
            //Whats my health at? Should/Can I heal myself?
            //How can I make my/my teammates next turn easier
            //Whos the best teammate of mine? How can I make them even better?

            //If I am gonna move to X, is there anything in my way? Does it hurt or benefit me?
            //Am I too close/far from my enemies? Should I move somewhere else?
        }

    

    }
}