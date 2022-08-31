using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// An interface placed on anything that is interactable in combat. 
    /// </summary>
    public interface CombatTarget
    {
        /// <summary>
        /// Gets the aim transform of this combat target. 
        /// The aim transform is what projectiles and other things will lock on to.
        /// In addition, it also is the transform the battle camera will follow
        /// </summary>
        Transform GetAimTransform();

        /// <summary>
        /// Returns the type of this combat target.
        /// </summary>
        T GetComponent<T>();
    }
}
