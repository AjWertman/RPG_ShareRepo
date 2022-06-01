using RPGProject.Saving;
using UnityEngine;

namespace RPGProject.Movement
{
    public class PlayerMover : MonoBehaviour, ISaveable
    {
        [SerializeField] float moveSpeed = 100f;
        [SerializeField] float turnSmoothness = .1f;
        [SerializeField] float gravity = 20f;

        Animator animator = null;
        CharacterController characterController = null;

        Vector3 direction = Vector3.zero;
        float turnSmoothVelocity;

        bool canMove = false;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
        }

        public void Move()
        {
            if (canMove)
            {
                direction = new Vector3(Input.GetAxisRaw("Horizontal"), -0, Input.GetAxisRaw("Vertical")).normalized;
                float magnitude = (direction.x * direction.x + direction.z * direction.z);
                if (magnitude >= .01f)
                {
                    float aimAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
                    float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, aimAngle, ref turnSmoothVelocity, turnSmoothness);
                    transform.rotation = Quaternion.Euler(0, angle, 0);

                    Vector3 moveDirection = Quaternion.Euler(0f, aimAngle, 0f) * Vector3.forward;

                    characterController.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);
                }
            }

            UpdateAnimator();
            characterController.Move(Vector3.down.normalized * gravity * Time.deltaTime);
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = characterController.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float forwardSpeed = localVelocity.z;

            animator.SetFloat("forwardSpeed", forwardSpeed);
        }

        public void SetCanMove(bool _canMove)
        {
            canMove = _canMove;
        }

        public bool CanMove()
        {
            return canMove;
        }

        public CharacterController GetCharacterController()
        {
            return characterController;
        }

        public object CaptureState()
        {
            return new SerializableVector3(transform.position);
        }

        public void RestoreState(object state)
        {
            SerializableVector3 position = (SerializableVector3)state;
            CharacterController characterControllerBackup = GetComponent<CharacterController>();

            characterControllerBackup.enabled = false;

            transform.position = position.ToVector3();

            characterControllerBackup.enabled = true;
        }
    }
}