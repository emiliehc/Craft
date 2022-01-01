using hongjun.dev.mc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace dev.hongjun.mc
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject cameraSubObject;
        private InputActions inputActions;
        private CharacterController charController;
        
        public float lookSensitivity = 50f;

        private float pitch = 0f;
        private float yaw = 0f;

        private float roll = 0f;

        // movement code
        public float movementSpeed = 10f;

        //Velocity
        private Vector3 velocity;

        //Gravity
        public float gravity = -9.8f;
        public float playerHeight = 0.2f;
        public bool isGrounded = false;
        public LayerMask groundLayer;

        public float jumpHeight = 1.5f;

        private void OnGUI()
        {
            if (Event.current.Equals(Event.KeyboardEvent("[esc]")))
            {
            }
        }

        private void OnJump()
        {
            var moveDelta = inputActions.Player.Movement.ReadValue<Vector2>();
            Debug.Log("Jumping!");
            if (moveDelta.magnitude == 0) return;
            var trans = transform;
            var translationDelta = (trans.right * moveDelta.x) + (trans.forward * moveDelta.y);
            //charController.Move(translationDelta * 60f * movementSpeed * Time.deltaTime);
        }

        private static void OnShoot()
        {
            Debug.Log("Shooting!");
        }


        private void OnEnable()
        {
            inputActions ??= new InputActions();
            inputActions.Enable();
        }

        // Start is called before the first frame update
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            charController = GetComponent<CharacterController>();
            if (charController == null)
            {
                Debug.LogError("Missing character controller on player!");
            }
        }

        // Update is called once per frame
        private void Update()
        {
            //Rotating camera with look axis
            var lookDelta = inputActions.Player.Look.ReadValue<Vector2>();
            //if there is any new look
            //Debug.Log(transform.position);
            if (lookDelta.magnitude != 0)
            {
                lookDelta *= lookSensitivity * Time.deltaTime;

                pitch -= lookDelta.y;
                yaw += lookDelta.x;

                pitch = Mathf.Clamp(pitch, -89f, 89f);

                cameraSubObject.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
                transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
                transform.Rotate(Vector3.up * lookDelta.x);
            }


            //for movement for now
            var moveDelta = inputActions.Player.Movement.ReadValue<Vector2>();
            //if there is new movement
            if (moveDelta.magnitude != 0)
            {
                var translationDelta = (transform.right * moveDelta.x) + (transform.forward * moveDelta.y);
                charController.Move(translationDelta * movementSpeed * Time.deltaTime);
            }


            //increment downwards vel
            velocity.y += 2.5f * gravity * Time.deltaTime;
            // if exists obj within sphere around us, is part of ground layer mask
            //reset down vel to 0
            isGrounded = IsOnGround();
            if (isGrounded && velocity.y < 0)
            {
                velocity.y = 0f;
            }

            charController.Move(velocity * Time.deltaTime);
        }

        private bool IsOnGround()
        {
            return Physics.Raycast(transform.position, Vector3.down, 0.01f);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Death")) return;
            GetComponent<PlayerController>().enabled = false;
            transform.position = new Vector3(0f, 5f, 0f);
            cameraSubObject.transform.position = new Vector3(0f, 5f, 0f);
            GetComponent<PlayerController>().enabled = true;
        }
    }
}