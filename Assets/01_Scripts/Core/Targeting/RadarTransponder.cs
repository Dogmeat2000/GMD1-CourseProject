using UnityEngine;
using _01_Scripts.Core.Interfaces;

namespace _01_Scripts.Core.Targeting
{
    public class RadarTransponder : MonoBehaviour, ITargetable
    {
        [Header("Transponder Settings")]
        [Tooltip("Which side of the war this entity belongs to")]
        [field: SerializeField] public Faction Faction { get; private set; } = Faction.Friendly;

        [Tooltip("The tactical value of this target to the opposing faction")]
        [field: SerializeField] public TargetPriority Priority { get; private set; } = TargetPriority.Standard;

        public Transform TargetTransform => transform;
        
        public bool IsTargetable => gameObject.activeInHierarchy; 

        private void OnEnable() {
            if (BattlefieldRadar.Instance) BattlefieldRadar.Instance.RegisterTarget(this);
        }

        private void OnDisable() {
            if (BattlefieldRadar.Instance) BattlefieldRadar.Instance.UnregisterTarget(this);
        }
        
        private void Start() {
            if (BattlefieldRadar.Instance) BattlefieldRadar.Instance.RegisterTarget(this);
        }
    }
}