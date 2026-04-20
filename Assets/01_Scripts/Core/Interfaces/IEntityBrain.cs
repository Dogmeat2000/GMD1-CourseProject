namespace _01_Scripts.Core.Interfaces
{
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