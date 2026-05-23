using _01_Scripts._10_Core.DependencyInjection;
using UnityEngine;

namespace _01_Scripts._20_Features.Targeting
{
    /// <summary>
    /// Mandatory identifier that is attached to all Entities involves in the battle (Ships & Hostile entities).
    /// Attaching this script to an entity makes it visible on the BattlefieldRadar, allowing for game entities to properly interact with each other - identify friends and foes.
    /// </summary>
    public class RadarTransponder : MonoBehaviour, ITargetable
    {
        [Header("Transponder Settings")]
        [Tooltip("Which side of the war this entity belongs to")]
        [field: SerializeField] public Faction Faction { get; private set; } = Faction.Friendly;
        
        [Tooltip("The tactical value of this target to the opposing faction")]
        [field: SerializeField] public TargetPriority Priority { get; private set; } = TargetPriority.Standard;
        
        public Transform TargetTransform => transform;
        public bool IsTargetable => isActiveAndEnabled;
        
        private IActorTracker _battlefieldRadar;

        private void OnEnable() {
            _battlefieldRadar?.RegisterTarget(this);
        }

        private void OnDisable() {
            _battlefieldRadar?.UnregisterTarget(this);
        }

        private void Awake() {
            _battlefieldRadar = ServiceLocator.Get<IActorTracker>();
        }
        
        private void Start() {
            _battlefieldRadar?.RegisterTarget(this);
        }
    }
}