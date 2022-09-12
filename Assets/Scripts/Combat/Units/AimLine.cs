using RPGProject.Core;
using UnityEngine;

namespace RPGProject.Combat
{
    /// <summary>
    /// The line that is used to indicate where the player is currently aiming their ability,
    /// and if they are in range to use the ability.
    /// </summary>
    public class AimLine : MonoBehaviour
    {
        [SerializeField] Material inRangeMaterial = null;
        [SerializeField] Material notInRangeMaterial = null;

        public Fighter hitFighter = null;

        LineRenderer lineRenderer = null;
        Raycaster raycaster = null;

        Transform parent = null;
        Vector3 currentTarget = Vector3.zero;

        bool hasDrawnAimLine = false;

        private void Awake()
        {
            raycaster = FindObjectOfType<Raycaster>();
            lineRenderer = GetComponent<LineRenderer>();
            parent = transform.parent;
        }

        /// <summary>
        /// Activates the aim line, setting its start and end position, and updating the color 
        /// to represent if the combatant is in range or not.
        /// </summary>
        public bool DrawAimLine(Transform _origin, Vector3 _targetPostion, Ability _selectedAbility)
        {
            float attackRange = _selectedAbility.attackRange;
            if (_origin == null || attackRange == 0)
            {
                ResetLine();
                return false;
            }

            bool isInstaHit = _selectedAbility.abilityType == AbilityType.InstaHit;

            if (currentTarget != _targetPostion)
            {
                currentTarget = _targetPostion;
                hasDrawnAimLine = false;
            }

            RaycastHit hit = raycaster.GetRaycastHit(_origin.position, _targetPostion);
            if (hit.collider == null) return false;

            if (!hasDrawnAimLine)
            {
                Vector3 parentEulers = parent.localEulerAngles;
                transform.localEulerAngles = new Vector3(parentEulers.x, -parentEulers.y, parentEulers.z);

                transform.position = Vector3.zero;
                //transform.parent = _origin;
                hasDrawnAimLine = true;
                gameObject.SetActive(true);

                lineRenderer.positionCount = 2;
                lineRenderer.SetPosition(0, _origin.position);
                lineRenderer.SetPosition(1, hit.point);
            }

            Fighter fighter = hit.collider.GetComponent<Fighter>();
            if (fighter == null)
            {
                hitFighter = null;
                if (RequiresFighter(_selectedAbility.targetingType))
                {
                    lineRenderer.material = notInRangeMaterial;
                    return false;
                }
            }
            else
            {
                if (Vector3.Distance(_origin.position, hit.point) > attackRange) lineRenderer.material = notInRangeMaterial;
                else lineRenderer.material = inRangeMaterial;
                hitFighter = fighter;
            }

            return true;
        }

        public void ResetLine()
        {
            if (!gameObject.activeSelf) return;
            lineRenderer.positionCount = 0;
            hasDrawnAimLine = false;
            currentTarget = Vector3.zero;
            hitFighter = null;
            gameObject.SetActive(false);
        }

        private bool RequiresFighter(TargetingType _targetingType)
        {
            if (_targetingType == TargetingType.GridBlocksOnly || _targetingType == TargetingType.Everything) return false;
            return true;
        }
    }
}
