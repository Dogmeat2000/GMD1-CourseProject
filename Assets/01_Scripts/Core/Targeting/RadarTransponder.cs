using UnityEngine;
using _01_Scripts.Core.Interfaces;
using _01_Scripts.Core.Services;

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
        
        public bool IsTargetable => isActiveAndEnabled;
        private BattlefieldRadar _battlefieldRadar;

        private void OnEnable() {
            if (_battlefieldRadar) 
                _battlefieldRadar.RegisterTarget(this);
        }

        private void OnDisable() {
            if (_battlefieldRadar) 
                _battlefieldRadar.UnregisterTarget(this);
        }

        private void Awake() {
            _battlefieldRadar = ServiceLocator.Get<BattlefieldRadar>();
        }
        
        private void Start() {
            if (_battlefieldRadar) 
                _battlefieldRadar.RegisterTarget(this);
        }
    }
}