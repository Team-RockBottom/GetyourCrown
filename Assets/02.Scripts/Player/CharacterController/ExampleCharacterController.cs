using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KinematicCharacterController;
using Photon.Pun;
using Photon.Realtime;
using Crown;
using Augment;
using static UnityEngine.Rendering.HableCurve;
using GetyourCrown.Network;

namespace GetyourCrown.CharacterContorller
{
    public enum CharacterState
    {
        Default,
    }

    public enum OrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool Attack;
        public bool JumpDown;
        public bool Kickable;
        public bool Pickable;
        public bool CrouchUp;
        public bool Run;
    }

    public struct AICharacterInputs
    {
        public Vector3 MoveVector;
        public Vector3 LookVector;
    }

    public enum BonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
    }

    public class ExampleCharacterController : MonoBehaviour, ICharacterController
    {

        public static Dictionary<int, ExampleCharacterController> controllers
                = new Dictionary<int, ExampleCharacterController>();

        public int ownerActorNr => _photonView.OwnerActorNr;
        public int photonViewId => _photonView.ViewID;
        public bool isInitialized { get; private set; }
        public PickableObject pickable { get; set; }

        [SerializeField] Transform _crownPosition;

        Animator _animator;
        bool _isStun = false;
        bool _scoreCheckAlreadyStart = false;

        PhotonView _photonView;
        ExampleCharacterController _controller;
        ScoreCounter _scoreCounter;

        [SerializeField] LayerMask _kingLayer;


        public KinematicCharacterMotor Motor;

        [Header("Augment Attachment")]
        [SerializeField] int _augmentId = -1;
        [SerializeField] float rangeMultiple = 1;
        [SerializeField] float speedMultiple = 1;
        private AugmentRepository _augmentRepository;

        private UI_Augment _uiAugment;
        
        [Header("Stable Movement")]
        public float MaxStableWalkSpeed = 10f;
        public float MaxStableRunSpeed = 15f;
        public float StableMovementSharpness = 15f;
        public float OrientationSharpness = 10f;
        public OrientationMethod OrientationMethod = OrientationMethod.TowardsCamera;
        private bool _isRun = false;
        public bool _isGround = false;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 15f;
        public float AirAccelerationSpeed = 15f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public float JumpUpSpeed = 10f;
        public float JumpScalableForwardSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Misc")]
        public List<Collider> IgnoredColliders = new List<Collider>();
        public BonusOrientationMethod BonusOrientationMethod = BonusOrientationMethod.None;
        public float BonusOrientationSharpness = 10f;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;
        public Transform CameraFollowPoint;
        public float CrouchedCapsuleHeight = 1f;

        public CharacterState CurrentCharacterState { get; private set; }

        private Collider[] _probedColliders = new Collider[8];
        private RaycastHit[] _probedHits = new RaycastHit[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;

        private Vector3 lastInnerNormal = Vector3.zero;
        private Vector3 lastOuterNormal = Vector3.zero;

        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
            controllers.TryAdd(ownerActorNr, this);

            _scoreCounter = FindAnyObjectByType<ScoreCounter>();
            
            if (!_photonView.IsMine)
            {
                Motor.enabled = false;
                return;
            }

            TransitionToState(CharacterState.Default);
            Motor.CharacterController = this;
        }

        private void Start()
        {
            Debug.Log("CharacterController Start");
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogError("Photon is not connected!");
                return;
            }


            if (_photonView.IsMine)
            {
                UI_Augment.OnAugmentSelected += HandleAugmentSelected;
                _augmentRepository = FindAnyObjectByType<AugmentRepository>();

            }

        }

        private void Update()
        {
            if (_photonView.IsMine)
            {
                if (gameObject.layer == 17 && _scoreCheckAlreadyStart == false)
                {
                    _scoreCheckAlreadyStart = true;
                    _scoreCounter.CountUpStart();
                }

                if (gameObject.layer == 0 && _scoreCheckAlreadyStart == true)
                {
                    _scoreCheckAlreadyStart = false;
                    _scoreCounter.CountUpEnd();
                }
            }
        }

        private void OnDestroy()
        {
            controllers.Remove(ownerActorNr);
        }

        public void TransitionToState(CharacterState newState)
        {
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }


        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
            }
        }

        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
            }
        }


        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _moveInputVector = cameraPlanarRotation * moveInputVector;

                        switch (OrientationMethod)
                        {
                            case OrientationMethod.TowardsCamera:
                                _lookInputVector = cameraPlanarDirection;
                                break;
                            case OrientationMethod.TowardsMovement:
                                _lookInputVector = _moveInputVector.normalized;
                                break;
                        }

                        // Jumping input
                        if (inputs.JumpDown)
                        {
                            _timeSinceJumpRequested = 0f;
                            _jumpRequested = true;
                            _isGround = true;
                        }


                        if (inputs.Run)
                        {
                            _isRun = !_isRun;
                        }



                        break;
                    }
            }
        }


        public void SetInputs(ref AICharacterInputs inputs)
        {
            _moveInputVector = inputs.MoveVector;
            _lookInputVector = inputs.LookVector;
        }

        private Quaternion _tmpTransientRot;


        public void BeforeCharacterUpdate(float deltaTime)
        {
        }


        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            if (_isStun)
            {
                return;
            }
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_lookInputVector.sqrMagnitude > 0f && OrientationSharpness > 0f)
                        {
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (BonusOrientationMethod == BonusOrientationMethod.TowardsGravity)
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (BonusOrientationMethod == BonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (Motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = Motor.TransientPosition + (currentUp * Motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                Motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * Motor.Capsule.radius));
                            }
                            else
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -Gravity.normalized, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                            }
                        }
                        else
                        {
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-BonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        break;
                    }
            }
        }

        
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            if (_isStun)
            {
                return;
            }

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            float currentVelocityMagnitude = currentVelocity.magnitude;

                            Vector3 effectiveGroundNormal = Motor.GroundingStatus.GroundNormal;

                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, effectiveGroundNormal) * currentVelocityMagnitude;

                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(effectiveGroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            Vector3 targetMovementVelocity;

                            if (!_isRun)
                            {
                                targetMovementVelocity = reorientedInput * MaxStableWalkSpeed;
                            }
                            else
                            {
                                targetMovementVelocity = (reorientedInput * MaxStableRunSpeed) * speedMultiple;
                            }

                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1f - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        else
                        {
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                Vector3 addedVelocity = _moveInputVector * AirAccelerationSpeed * deltaTime;

                                Vector3 currentVelocityOnInputsPlane = Vector3.ProjectOnPlane(currentVelocity, Motor.CharacterUp);

                                if (currentVelocityOnInputsPlane.magnitude < MaxAirMoveSpeed)
                                {
                                    Vector3 newTotal = Vector3.ClampMagnitude(currentVelocityOnInputsPlane + addedVelocity, MaxAirMoveSpeed);
                                    addedVelocity = newTotal - currentVelocityOnInputsPlane;
                                }
                                else
                                {
                                    if (Vector3.Dot(currentVelocityOnInputsPlane, addedVelocity) > 0f)
                                    {
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, currentVelocityOnInputsPlane.normalized);
                                    }
                                }

                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    if (Vector3.Dot(currentVelocity + addedVelocity, addedVelocity) > 0f)
                                    {
                                        Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                        addedVelocity = Vector3.ProjectOnPlane(addedVelocity, perpenticularObstructionNormal);
                                    }
                                }

                                currentVelocity += addedVelocity;
                            }

                            currentVelocity += Gravity * deltaTime;

                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }

                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested)
                        {
                            if (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime))
                            {
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }

                                Motor.ForceUnground();

                                currentVelocity += (jumpDirection * JumpUpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                currentVelocity += (_moveInputVector * JumpScalableForwardSpeed);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                                _isGround = false;
                            }
                        }

                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                        break;
                    }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            if (_isStun)
            {
                return;
            }

            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        {
                            if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                            {
                                if (!_jumpedThisFrame)
                                {
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders,
                                Motor.CollidableLayers,
                                QueryTriggerInteraction.Ignore) > 0)
                            {
                                Motor.SetCapsuleDimensions(0.5f, CrouchedCapsuleHeight, CrouchedCapsuleHeight * 0.5f);
                            }
                            else
                            {
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Count == 0)
            {
                return true;
            }

            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }

            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        protected void OnLanded()
        {
        }

        protected void OnLeaveStableGround()
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        public void Hit(GameObject gameObject)
        {

        }


        [SerializeField] LayerMask _crownLayer;




        private const float SPHERCAST_RADIUS = 1f;
        private const float SPHERCAST_MAXDISTANCE = 1f;
        private const float SPHERCAST_KICK_RADIUS = 2;
        private const float SPHERCAST_KICK_MAXDISTANCE = 2;
        [SerializeField] private float _kickPower = 3f;
        public void TryKick()
        {
            if (Physics.SphereCast(transform.position, SPHERCAST_KICK_RADIUS, transform.forward, out RaycastHit hit, SPHERCAST_KICK_MAXDISTANCE, _crownLayer))
            {
                KickableObject kickable = hit.collider.GetComponent<KickableObject>();
                kickable.Kick((hit.point - transform.position) * _kickPower);
            }
        }

        public void TryPickUp()
        {
            Collider[] cols = Physics.OverlapSphere(transform.position, 1f, _crownLayer);

            if (cols.Length > 0)
            {
                cols[0].GetComponent<PickableObject>().PickUp();
                return;
            }
        }

        public void TryAttack()
        {

            if (Physics.SphereCast(transform.position, SPHERCAST_RADIUS * rangeMultiple, transform.forward, out RaycastHit hit, SPHERCAST_MAXDISTANCE, _kingLayer))
            {
                PickableObject pickable = hit.collider.GetComponentInChildren<PickableObject>();
                pickable.Drop();
            }
        }
       

        internal Transform GetCrownPosition()
        {
            return _crownPosition;
        }


        public void AddController()
        {
            controllers.Add(_photonView.OwnerActorNr, this);
            Debug.Log(controllers.Count);
        }


        void HandleAugmentSelected(int augmentId)
        {
            Debug.Log("이벤트 발생해서 캐릭터 컨트롤러에서 호출");
            AugmentSpec augment = _augmentRepository._augmentDic[augmentId];

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInGamePlayPropertyKey.IS_AUGMENT_SELECTED, true }
            });

            switch (augmentId)
            {
                case 0:
                default:
                    break;
                case 1:
                    Debug.Log("Switch 1 Call");
                    speedMultiple = augment.speedIncrease;
                    break;
                case 2:
                    Debug.Log("Switch 2 Call");
                    rangeMultiple = augment.increaseValue;
                    break;
                case 3:
                    Debug.Log("Switch 3 Call");
                    break;
                case 4:
                    Debug.Log("Switch 4 Call");
                    break;
                case 5:
                    Debug.Log("Switch 5 Call");
                break;
            }
        }

    }
}