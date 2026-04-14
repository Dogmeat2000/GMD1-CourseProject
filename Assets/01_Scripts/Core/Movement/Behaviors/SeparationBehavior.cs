using _01_Scripts.Core.Interfaces;
using UnityEngine;

namespace _01_Scripts.Core.Movement.Behaviors
{
    public class SeparationBehavior : IMovementBehavior
    {
        private readonly float _repelRadius;
        private readonly LayerMask _allyLayer;
        private readonly Collider[] _neighbors = new Collider[50];

        public SeparationBehavior(float repelRadius, LayerMask allyLayer) {
            _repelRadius = repelRadius;
            _allyLayer = allyLayer;
        }

        public Vector3 CalculateDirection(Transform entityTransform, Vector3 targetPosition) {
            Vector3 repelForce = Vector3.zero;
            
            int hits = Physics.OverlapSphereNonAlloc(entityTransform.position, _repelRadius, _neighbors, _allyLayer);
            int count = 0;
            
            for (int i = 0; i < hits; i++) {
                Collider neighbor = _neighbors[i];
                if (neighbor.transform == entityTransform) 
                    continue;

                Vector3 awayFromNeighbor = entityTransform.position - neighbor.transform.position;
                repelForce += awayFromNeighbor.normalized / awayFromNeighbor.magnitude;
                count++;
            }

            if (count > 0) {
                repelForce /= count;
            }

            return repelForce.normalized;
        }
    }
}