namespace _01_Scripts._20_Features.Weapons
{
    public interface IRangedWeapon
    {
        /// <summary>
        /// Fires the concrete ranged weapon.
        /// </summary>
        void Fire();
        
        /// <summary>
        /// Action to be performed when player releases the trigger.
        /// </summary>
        void ReleaseTrigger();
    }
}
