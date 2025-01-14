using KinematicCharacterController.Examples;
using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.Utilities;
using System.Collections;
using UnityEngine;

namespace Practices.PhotonPunClient.Network
{
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class Pickable : PunAutoSyncMonobehaviour, IPunOwnershipCallbacks
    {
        Rigidbody _rigidbody;
        bool _isPickedUp;
        bool _isPickingUp;
        Coroutine _pickUpRoutine;
        bool _isOwned;


        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void PickUp()
        {
            if (_isPickedUp)
                return;

            if (_isPickingUp)
                return;

            _isPickingUp = true;
            _pickUpRoutine = StartCoroutine(C_PickUp());
        }

        public void Drop()
        {
            if (photonView.IsMine == false)
                return;

            if (_isPickedUp == false)
                return;

            photonView.RPC(nameof(InternalDrop), RpcTarget.All, PhotonNetwork.LocalPlayer);
        }

        public IEnumerator C_PickUp()
        {
            // Server - Client 구조에서 Client 는 기본적으로 모든 데이터를 조작할 권한이 없다는게 전제.
            // 다른 NetworkObject 를 조작하고싶은 Client 가 있다면 서버를 통해서 기존 권한자에게 요청하고 승낙받아야함.
            // PhotonNetwork.Instantiate 처럼 Client 가 서버에게 특정 NetworkObject 를 만들어서 사용하겠다고 요청하면 권한이 부여됨.
            if (_isOwned)
            {
                photonView.OwnershipTransfer = OwnershipOption.Request;
            }
            else
            {
                photonView.OwnershipTransfer = OwnershipOption.Takeover;
            }

            photonView.RequestOwnership();

            yield return new WaitUntil(() => photonView.IsMine);

            photonView.RPC(nameof(InternalPickUp), RpcTarget.All, PhotonNetwork.LocalPlayer);
            _isPickingUp = false;
        }

        [PunRPC]
        private void InternalPickUp(Player picker)
        {
            if (ExampleCharacterController.controllers.TryGetValue(picker.ActorNumber, out ExampleCharacterController controller))
            {
                _rigidbody.isKinematic = true;
                controller.pickable = this;
                Transform hand = controller.GetEmptyHand();
                transform.SetParent(hand);
                transform.localPosition = Vector3.zero;
                _isPickedUp = true;
                _isOwned = true;
            }
        }

        [PunRPC]
        private void InternalDrop(Player dropper)
        {
            if (ExampleCharacterController.controllers.TryGetValue(dropper.ActorNumber, out ExampleCharacterController controller))
            {
                _rigidbody.isKinematic = false;
                controller.pickable = null;
                transform.SetParent(null);
                _isPickedUp = false;
                _isOwned = false;
            }
        }

        public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
        {
            if (targetView.IsMine)
                return;

            // 현재 View 가 대상인지 확인
            if (targetView != photonView)
                return;

            targetView.TransferOwnership(requestingPlayer);
        }

        public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
        {
            if (targetView != photonView)
                return;

            // 이전에 들고있던 오너에 대한 처리 필요함
        }

        public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
        {
            if (targetView != photonView)
                return;

            // 내가 줍기 (소유권 이전) 요청했는데 실패한거면
            if (PhotonNetwork.LocalPlayer == senderOfFailedRequest)
            {
                // 줍고있었다면 줍기 취소
                if (_isPickingUp)
                {
                    StopCoroutine(_pickUpRoutine);
                    _isPickingUp = false;
                }
            }
        }
    }
}