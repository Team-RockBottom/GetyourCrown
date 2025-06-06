﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Photon.Pun;
using GetyourCrown.UI;
using GetyourCrown.Network;
using Photon.Realtime;
using TMPro;

namespace GetyourCrown.CharacterContorller
{
    public class ExamplePlayer : MonoBehaviour, IAnimationController
    {
        PhotonView _photonView;
        Rigidbody _rigidBody;

        ExampleCharacterController Character;
        ExampleCharacterCamera CharacterCamera;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";



        bool _isWorking = false;
        bool _inAir = false;
        bool _isStun = false;
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
        const float MOVING_WALK = 0.3f;
        const float MOVING_RUN = 0.6f;
        const float MOVING_RUN_AUGMENT  = 1f;
        [SerializeField] const float DEFAULT_DELAY_TIME = 1f;
        [SerializeField] TMP_Text _nickName;

        public State currentState { get; private set; }

        public bool isTransitioning => _animator.GetBool(IS_DIRTY_HASH);

        UI_Option _uI_Option;
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            Character = GetComponent<ExampleCharacterController>();
            _animator = Character.GetComponent<Animator>();
            _rigidBody = Character.GetComponent<Rigidbody>();

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

            // 자신의 닉네임을 TextMeshPro UI에 표시
            _nickName.text = _photonView.Owner.NickName;

            // 다른 플레이어들도 닉네임을 볼 수 있도록 업데이트
            _photonView.RPC(nameof(UpdateNickName), RpcTarget.AllBuffered, _photonView.Owner.NickName);

            CharacterCamera.SetFollowTransform(Character.CameraFollowPoint);

            CharacterCamera.IgnoredColliders.Clear();
            CharacterCamera.IgnoredColliders.AddRange(Character.GetComponentsInChildren<Collider>());

            _uI_Option = UI_Manager.instance.Resolve<UI_Option>();
            _uI_Option.onHide += () => { _isESC = false; };
            _uI_Option.onShow += () => { _isESC = true; };
            _uI_Option.Show();
            _uI_Option.Hide();


            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                {PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, true }
            });
        }

        [PunRPC]
        void UpdateNickName(string nickName)
        {
            _nickName.text = nickName;
        }

        private void Update()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            if (Character._augmentIsNotSelected)
            {
                return ;
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

        [PunRPC]
        void NickNameRotation()
        {
            Vector3 direction = Camera.main.transform.position - _nickName.gameObject.transform.position; //카메라와의 방향 계산
            direction.y = 0; //Y축 회전을 고정하여 UI가 위아래로 기울어지지 않도록 함
            Quaternion rotation = Quaternion.LookRotation(-direction); //UI가 카메라를 바라보도록 회전
            _nickName.gameObject.transform.rotation = rotation; //UIImage 회전 적용
        }

        private void LateUpdate()
        {
            if (!_photonView.IsMine)
            {
                return;
            }

            if (Character._augmentIsNotSelected)
            {
                return;
            }

            if (CharacterCamera.RotateWithPhysicsMover && Character.Motor.AttachedRigidbody != null)
            {
                CharacterCamera.PlanarDirection = Character.Motor.AttachedRigidbody.GetComponent<PhysicsMover>().RotationDeltaFromInterpolation * CharacterCamera.PlanarDirection;
                CharacterCamera.PlanarDirection = Vector3.ProjectOnPlane(CharacterCamera.PlanarDirection, Character.Motor.CharacterUp).normalized;
            }

            _photonView.RPC(nameof(NickNameRotation), RpcTarget.All);

            HandleCameraInput();
        }

        private void HandleCameraInput()
        {

            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            Vector3 lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);

            float scrollInput = -Input.GetAxis(MouseScrollInput);
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            CharacterCamera.UpdateWithInput(Time.deltaTime, scrollInput, lookInputVector);


        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();


            characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
            characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
            characterInputs.CameraRotation = CharacterCamera.Transform.rotation;
            if (_isWorking)
            {
                Character.SetInputs(ref characterInputs);
                return;
            }
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
                {
                    if (Character._selectedAugmentId == 1 && Character._maxSpeed)
                    {
                        _animator.SetFloat(SPEED_HASH, MOVING_RUN_AUGMENT);
                    }
                    else
                    {
                        _animator.SetFloat(SPEED_HASH, MOVING_RUN);
                    }
                }
                else
                {
                    _animator.SetFloat(SPEED_HASH, MOVING_WALK);
                }
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


            if (characterInputs.Attack)
            {
                _isWorking = true;
                StartCoroutine(Attacking());
            }

            if (characterInputs.Kickable)
            {
                _isWorking = true;
                StartCoroutine(Kicking());
            }
            Character.SetInputs(ref characterInputs);
        }

        IEnumerator Jumping()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Jump);
            _animator.SetBool(IS_GROUNDED_HASH, false);
            _animator.SetBool(IS_DIRTY_HASH, true);
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetBool(IS_GROUNDED_HASH, true);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _inAir = false;
        }

        IEnumerator Picking()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Pick);
            _animator.SetBool(IS_DIRTY_HASH, true);
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _isWorking = false;
        }

        IEnumerator Attacking()
        {
            if (Character._selectedAugmentId == 2)
            {
                _animator.SetInteger(AUGMENT, (int)State.Attack);
                _animator.SetInteger(STATE_HASH, (int)State.Attack);
            }
            else
            {
                _animator.SetInteger(STATE_HASH, (int)State.Attack);
            }
            _animator.SetBool(IS_DIRTY_HASH, true);
            yield return new WaitForSeconds(DEFAULT_DELAY_TIME);
            _animator.SetInteger(STATE_HASH, (int)State.Move);
            _isWorking = false;
        }

        IEnumerator Kicking()
        {
            _animator.SetInteger(STATE_HASH, (int)State.Kick);
            _animator.SetBool(IS_DIRTY_HASH, true);
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