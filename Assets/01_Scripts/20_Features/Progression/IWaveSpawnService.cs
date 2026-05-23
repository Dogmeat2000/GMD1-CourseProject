using System;
using _01_Scripts._10_Core.DependencyInjection;

namespace _01_Scripts._20_Features.Progression
{
    public interface IWaveSpawnService : IService
    {
        /// <summary>
        /// Broadcast whenever a wave has changed (i.e. transitions from wave 1 to wave 2).
        /// </summary>
        public event Action<int, int> OnWaveUpdated;
        
        /// <summary>
        /// Broadcast whenever the number of enemies in a wave have changed (i.e. from death, or during iterative spawning).
        /// </summary>
        public event Action<int> OnEnemyCountChanged;
        
        /// <summary>
        /// Broadcast whenever a new Status Message should be displayed (i.e. 'WAVE 5 INCOMING!' or 'WAVE 5 CLEARED').
        /// </summary>
        public event Action<string> OnStatusMessage;
        
        /// <summary>
        /// Broadcast when all waves have been cleared.
        /// </summary>
        public event Action OnAllWavesCleared;
    }
}
