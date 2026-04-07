using System;
using UnityEngine;

namespace _01_Scripts.Core
{
    public class HealthManager : MonoBehaviour
    {
        [Header("Health Settings")]
        [Tooltip("The maximum health this entity can have")]
        [SerializeField] private int maxHealth = 100;
        
        [Tooltip("The current/starting health this entity is initialized with")]
        [SerializeField] private int currentHealth = 100;
        
        public int CurrentHealth  => currentHealth;
        public int MaxHealth => maxHealth;

        public event Action<int, int, GameObject> OnHealthChanged; // Broadcasts: CurrentHealth, MaxHealth
        public event Action<GameObject> OnZeroHealth;  // Broadcasts: GameObject that destroyed this entity.
        
        private void Awake() {
            currentHealth = maxHealth;
        }
        
        public void TakeDamage(int amount, GameObject instigator = null) {
            AdjustHealth(-Mathf.Abs(amount), instigator);
        }
        
        public void Heal(int amount) {
            AdjustHealth(Mathf.Abs(amount), null);
        }
        
        private void AdjustHealth(int changeAmount, GameObject instigator) {
            int previousHealth = currentHealth;
            
            currentHealth = Mathf.Clamp(currentHealth + changeAmount, 0, maxHealth);

            if (currentHealth == previousHealth) 
                return;
            
            OnHealthChanged?.Invoke(currentHealth, maxHealth, instigator);
                
            if (currentHealth == 0) {
                OnZeroHealth?.Invoke(instigator);
            }
        }
    }
}
