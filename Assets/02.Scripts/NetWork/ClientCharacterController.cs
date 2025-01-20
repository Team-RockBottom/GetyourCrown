//using Photon;
//using Photon.Pun;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;
//using UnityEngine.InputSystem;

//namespace Practices.PhotonPunClient.Network
//{
//    public class ClientCharacterController : MonoBehaviour, IPunInstantiateMagicCallback//, IPunObservable
//    {
//        // 멀티톤
//        public static Dictionary<int, ClientCharacterController> controllers
//            = new Dictionary<int, ClientCharacterController>();

//        public int ownerActorNr => _photonView.OwnerActorNr;
//        public int photonViewId => _photonView.ViewID;
//        public bool isInitialized { get; private set; }
//        public Pickable pickable { get; set; }

//        PhotonView _photonView;
//        NavMeshAgent _agent;
//        InputActions _inputActions;
//        [SerializeField] LayerMask _groundMask;
//        [SerializeField] LayerMask _pickable;
//        [SerializeField] LayerMask _kickable;
//        [SerializeField] Transform _rightHand;
//        [SerializeField] Transform _leftHand;


//        private void Awake()
//        {
//            _photonView = GetComponent<PhotonView>();
//            _agent = GetComponent<NavMeshAgent>();
//        }

//        public Transform GetEmptyHand()
//        {
//            return _rightHand; // 일단은 오른손 줌
//        }

//        public void OnPhotonInstantiate(PhotonMessageInfo info)
//        {
//            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
//            {
//                { PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, true }
//            });

//            isInitialized = true;

//            if (_photonView.IsMine)
//            {
//                _agent.enabled = true;
//                _inputActions = new InputActions();
//                _inputActions.Player.Fire.performed += OnLeftClick;
//                _inputActions.Player.MouseRight.performed += OnRightClick;
//                _inputActions.Enable();
//            }
//            else
//            {
//                _agent.enabled = false;
//            }

//            controllers.Add(_photonView.OwnerActorNr, this);
//            Debug.Log("Instantiated");
//        }

//        void OnLeftClick(InputAction.CallbackContext context)
//        {
//            if (pickable)
//            {
//                pickable.Drop();
//                return;
//            }
//            else
//            {
//                Collider[] cols = Physics.OverlapSphere(transform.position, 1f, _pickable);

//                if (cols.Length > 0)
//                {
//                    cols[0].GetComponent<Pickable>().PickUp();
//                    return;
//                }
//            }

//            if (Physics.SphereCast(transform.position, 1f, transform.forward, out RaycastHit hit, 1f, _kickable))
//            {
//                Kickable kickable = hit.collider.GetComponent<Kickable>();
//                kickable.Kick((hit.point - transform.position) * 3f);
//            }
//        }

//        void OnRightClick(InputAction.CallbackContext context)
//        {
//            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
//            Debug.DrawRay(ray.origin, ray.direction);

//            if (Physics.Raycast(ray, out RaycastHit hit, float.PositiveInfinity, _groundMask))
//            {
//                _agent.SetDestination(hit.point);
//            }
//        }

//        /// <summary>
//        /// Owner 만 Writing 함 (데이터 송신)
//        /// Others 들이 Reading 함 (데이터 수신)
//        /// </summary>
//        /// <param name="stream"></param>
//        /// <param name="info"></param>
//        //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
//        //{
//        //    if (stream.IsWriting)
//        //    {
//        //        stream.SendNext(transform.position);
//        //        stream.SendNext(transform.rotation);
//        //    }
//        //    else
//        //    {
//        //        _positionCached = (Vector3)stream.ReceiveNext();
//        //        _rotationCached = (Quaternion)stream.ReceiveNext();
//        //    }
//        //}
//    }
//}