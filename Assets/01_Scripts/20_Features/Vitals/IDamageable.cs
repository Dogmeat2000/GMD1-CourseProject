using UnityEngine;

namespace _01_Scripts._20_Features.Vitals
{
    /// <summary>
    /// Universal contract for any entity that can sustain damage.
    /// </summary>
    public interface IDamageable
    {
        void TakeDamage(int amount, GameObject instigator = null);
    }
}