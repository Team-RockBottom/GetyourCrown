using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using KinematicCharacterController.Examples;

namespace KinematicCharacterController.Examples
{
    public class ExamplePlayer : MonoBehaviour
    {
        public ExampleCharacterController Character;
        public ExampleCharacterCamera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        [SerializeField] LayerMask _playerLayerMask;
        [SerializeField] LayerMask _kickableLayerMask;

        private const float SPHERCAST_RADIUS = 1f;
        private const float SPHERCAST_MAXDISTANCE = 1f;
        [SerializeField] private float _kickPower = 3f;

        Animator _animator;

        private void Awake()
        {
            _animator = Character.GetComponent<Animator>();
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            // Handle rotating the camera along with physics movers
            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);

            // 플레이어 Attack
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit;

                if (Physics.SphereCast(transform.position, 0.5f,transform.forward, out hit, _playerLayerMask))
                {
                    hit.collider.gameObject.GetComponent<ExamplePlayer>();
                    //TODO : Kickable 호출 및 애니메이션
                }
            }

            // 왕관 Kick
            if (Input.GetMouseButtonDown(1))
            {
                if (Physics.SphereCast(transform.position, SPHERCAST_RADIUS, transform.forward, out RaycastHit hit, SPHERCAST_MAXDISTANCE, _kickableLayerMask))
                {
                    KickableObject kickable = hit.collider.GetComponent<KickableObject>();
                    kickable.Kick((hit.point - transform.position) * _kickPower);
                }
            }
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.Run = Input.GetKey(KeyCode.LeftShift);
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.Attack  = Input.GetMouseButtonDown(0);
            characterInputs.Pickable = Input.GetKeyDown(KeyCode.E);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            float axisCheck = Mathf.Abs(Input.GetAxisRaw(VerticalInput)) + Mathf.Abs(Input.GetAxisRaw(HorizontalInput));
            
            //걷기 뛰기 애니메이션 세팅
            if (axisCheck <= 0)
            {
                _animator.SetFloat("Speed", 0);
            }
            else if (axisCheck > 0)
            {
                if (characterInputs.Run)
                    _animator.SetFloat("Speed", 1f);
                else
                    _animator.SetFloat("Speed", 0.5f);
            }

            if (characterInputs.JumpDown)
            {
                _animator.SetBool("IsGrounded", false);
            }
            else
            {
                _animator.SetBool("IsGrounded", true);
            }

            if (characterInputs.Pickable)
            {
                _animator.SetInteger("State", 2);
                _animator.SetBool("IsDirty",true);
                //characterInputs.Pickable = false;
            }
            else
            {
                _animator.SetInteger("State", 0);
                _animator.SetBool("IsDirty", false);
            }



            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }
    }
}