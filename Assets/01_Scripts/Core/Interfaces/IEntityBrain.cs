namespace _01_Scripts.Core.Interfaces
{
    /// <summary>
    /// Primary interface that AI controlled attackers must implement.
    /// </summary>
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