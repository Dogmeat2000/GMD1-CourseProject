namespace _01_Scripts.Core.Interfaces
{
    public interface IRangedPlayerWeapon : IRangedWeapon
    {
        /// <summary>
        /// Action to be performed when player releases the trigger.
        /// </summary>
        void ReleaseTrigger();
    }
}
