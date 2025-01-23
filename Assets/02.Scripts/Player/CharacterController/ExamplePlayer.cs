using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Photon.Pun;
using GetyourCrown.UI;

namespace GetyourCrown.CharacterContorller
{
    public class ExamplePlayer : MonoBehaviour, IAnimationController
    {
        PhotonView _photonView;

        ExampleCharacterController Character;
        ExampleCharacterCamera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        [SerializeField] LayerMask _playerLayerMask;


        /// <summary>
        /// 딜레이를 위한 bool타입변수
        /// </summary>
        bool _isWorking = false;
        bool _inAir = false;
        bool _isESC = false;

        [SerializeField] float _isAttackDelayTime = 0f;
        [SerializeField] float _isPickDelayTime = 0f;
        [SerializeField] float _isKickDelayTime = 0f;


        Animator _animator;

        readonly int STATE_HASH = Animator.StringToHash("State");
        readonly int IS_DIRTY_HASH = Animator.StringToHash("IsDirty");
        readonly int SPEED_HASH = Animator.StringToHash("Speed");
        readonly int IS_GROUNDED_HASH = Animator.StringToHash("IsGrounded");
        readonly int IS_STUN = Animator.StringToHash("IsStun");
        readonly int IS_HIT = Animator.StringToHash("IsHit");
        readonly int AUGMENT = Animator.StringToHash("Augment");

        const float MOVING_STOP = 0;
        const float MOVING_WALK = 0.5f;
        const float MOVING_RUN = 1;
        [SerializeField] const float DEFAULT_DELAY_TIME =1f;


        public State currentState { get; private set; }

        public bool isTransitioning => _animator.GetBool(IS_DIRTY_HASH);

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            Character = GetComponent<ExampleCharacterController>();
            _animator = Character.GetComponent<Animator>();

            StateMachineBehaviourBase[] behaviours = _animator.GetBehaviours<StateMachineBehaviourBase>();

            for (int i = 0; i < behaviours.Length; i++)
            {
                behaviours[i].onStateEntered += state => currentState = state;
            }
        }
        private void Start()
        {

            if (_photonView.IsMine)
            {
                CharacterCamera = Camera.main.GetComponent<ExampleCharacterCamera>();
            }
            else
            {
                return;
            }

            //Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());

            _uI_Option = UI_Manager.instance.Resolve<UI_Option>();
            _uI_Option.onHide += () => { _isESC = false; };
            _uI_Option.onShow += () => { _isESC = true; };
            _uI_Option.Show();
            _uI_Option.Hide();
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            if (_isWorking)
            {
                return;
            }

            //if (Input.GetMouseButtonDown(0))
            //{
            //    Cursor.lockState = CursorLockMode.Locked;
            //}
            if (Input.GetMouseButtonDown(0) && _isESC == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {

                if (_isESC)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    _uI_Option.Hide();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    _uI_Option.Show();
                }
            }
            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

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
            //if (Cursor.lockState != CursorLockMode.Locked)
            //{
            //    lookInputVector = Vector3.zero;
            //}

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);


        }

        private void HandleCharacterInput()
        {
            if (_isWorking)
            {
                return;
            }

            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

            // Build the CharacterInputs struct
            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            characterInputs.Run = Input.GetKey(KeyCode.LeftShift);
            characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
            characterInputs.Attack = Input.GetMouseButtonDown(0);
            characterInputs.Kickable = Input.GetMouseButtonDown(1);
            characterInputs.Pickable = Input.GetKeyDown(KeyCode.E);
            characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

            float axisCheck = Mathf.Abs(Input.GetAxisRaw(VerticalInput)) + Mathf.Abs(Input.GetAxisRaw(HorizontalInput));

            if (axisCheck <= 0)
            {
                _animator.SetFloat(SPEED_HASH, MOVING_STOP);
            }
            else if (axisCheck > 0)
            {
                if (characterInputs.Run)
                    _animator.SetFloat(SPEED_HASH, MOVING_RUN);
                else
                    _animator.SetFloat(SPEED_HASH, MOVING_WALK);
            }

            if (characterInputs.JumpDown)
            {
                if (_inAir)
                {
                    return;
                }
                _inAir = true;
                StartCoroutine(Jumping());
            }


            if (characterInputs.Pickable)
            {
                _isWorking = true;
                StartCoroutine(Picking());
            }


            // 플레이어 Attack
            if (characterInputs.Attack)
            {
                _isWorking = true;
                StartCoroutine(Attacking());
            }

            // 왕관 Kick //시작할때 눌리는 버그 있음
            if (characterInputs.Kickable)
            {
                _isWorking = true;
                StartCoroutine(Kicking());
            }
            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }

        IEnumerator Jumping()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Jump);
            _animator.SetBool(IS_GROUNDED_HASH, false);
            _animator.SetBool(IS_DIRTY_HASH, true);
            //TODO GroundCheck
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetBool(IS_GROUNDED_HASH, true);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _inAir = false;
        }

        IEnumerator Picking()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Pick);
            _animator.SetBool(IS_DIRTY_HASH, true);
            Character.TryPickUp();
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _isWorking = false;
        }

        IEnumerator Attacking()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Attack);
            _animator.SetBool(IS_DIRTY_HASH, true); 
            Character.TryAttack();
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _isWorking = false;
        }

        IEnumerator Kicking()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Kick);
            _animator.SetBool(IS_DIRTY_HASH, true);
            Character.TryKick();
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _isWorking = false;
        }


        public void ChangeState(State newState)
        {
            _animator.SetInteger(STATE_HASH, (int)newState);
            _animator.SetBool(IS_DIRTY_HASH, true);
        }
    }
}