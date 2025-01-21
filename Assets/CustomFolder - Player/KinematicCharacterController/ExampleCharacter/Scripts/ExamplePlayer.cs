using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Photon.Pun;
using GetyourCrown.UI;

namespace GetyourCrown.CharacterContorller
{
    public class ExamplePlayer : MonoBehaviour
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
        bool _isAttack = false;
        bool _isPick = false;
        bool _iskick = false;
        bool _isESC = false;

        [SerializeField] float _isAttackDelayTime = 0f;
        [SerializeField] float _isPickDelayTime = 0f;
        [SerializeField] float _isKickDelayTime = 0f;

        Animator _animator;


        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            Character = GetComponent<ExampleCharacterController>();
            _animator = Character.GetComponent<Animator>();
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

            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0) && _isESC == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UI_Option uI_Option = UI_Manager.instance.Resolve<UI_Option>();
                uI_Option.onHide += () => {_isESC = false; };
                uI_Option.onShow += () => {_isESC = true; };
                if (_isESC)
                {
                    uI_Option.Hide();
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    uI_Option.Show();
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
            characterInputs.Attack = Input.GetMouseButtonDown(0);
            characterInputs.Kickable = Input.GetMouseButtonDown(1);
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

            //
            if (characterInputs.JumpDown)
            {
                Debug.Log("Jump Call");

                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
                {
                    float aniTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                    if (aniTime < 1 && aniTime > 0)
                    {
                        return;
                    }
                }
                _animator.SetBool("IsGrounded", false);
            }
            else
            {
                if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !Character._isGround)
                {
                    float aniTime = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

                    if (aniTime >= 1)
                    {
                        _animator.SetBool("IsGrounded", true);
                    }
                }
            }

            // 진행중인 애니메이션이 있으면 return

            if (characterInputs.Pickable && _isPick == false)
            {
                Debug.Log("LC Input in");
                _animator.SetInteger("State", 2);
                _animator.SetBool("IsDirty", true);
                Character.TryPickUp();
                StartCoroutine(PickDelay());
                //characterInputs.Pickable = false;
            }
            else
            {
                _animator.SetInteger("State", 0);
                _animator.SetBool("IsDirty", false);
            }


            // 플레이어 Attack
            if (characterInputs.Attack && _isAttack == false)
            {
                _animator.SetInteger("State", 1);
                _animator.SetBool("IsDirty", true);
                Character.TryAttack();
                StartCoroutine(AttackDelay());

            }

            // 왕관 Kick
            if (characterInputs.Kickable && _iskick == false)
            {
                _animator.SetInteger("State", 3);
                _animator.SetBool("IsDirty", true);
                Character.TryKick();
                StartCoroutine(KickDelay());
            }
            // Apply inputs to character
            Character.SetInputs(ref characterInputs);
        }


        IEnumerator AttackDelay()
        {
            _isAttack = true;
            yield return new WaitForSeconds(_isAttackDelayTime);
            _isAttack = false;
        }

        IEnumerator PickDelay()
        {
            _isPick = true;
            yield return new WaitForSeconds(_isPickDelayTime);
            _isPick = false;
        }

        IEnumerator KickDelay()
        {
            _iskick = true;
            yield return new WaitForSeconds(_isKickDelayTime);
            _iskick = false;
        }
    }
}