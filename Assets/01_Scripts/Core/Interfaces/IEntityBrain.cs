namespace _01_Scripts.Core.Interfaces
{
    // TODO: Add class descriptor
    public interface IEntityBrain
    {
        /// <summary>
        /// Called when the entity is spawned or initialized.
        /// </summary>
        void WakeUp();

        /// <summary>
        /// Called when the entity is destroyed, disabled, or returned to a pool.
        /// </summary>
        void ShutDown();
    }
}