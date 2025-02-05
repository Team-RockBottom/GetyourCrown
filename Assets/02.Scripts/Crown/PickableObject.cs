using Photon.Pun;
using Photon.Realtime;
using GetyourCrown.CharacterContorller;
using GetyourCrown.Network;
using UnityEngine;
using System.Collections;

namespace Crown
{
    public class PickableObject : PunAutoSyncMonobehaviour, IPunOwnershipCallbacks
    {
        Rigidbody _rigidbody;
        bool _isPickedUp = false;
        bool _isPickingUp = false;
        Coroutine _pickUpRoutine;
        bool _isOwned;

        const int DROP_FORCE = 10;

        Collider _collider;

        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();
            _collider = gameObject.GetComponent<Collider>();
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
            //if (photonView.IsMine == true)
            //    return;

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
                Debug.Log("InternalPickUp Call");
                _rigidbody.isKinematic = true;
                controller.pickable = this;
                Transform crownPosition = controller.GetCrownPosition();
                _collider.isTrigger = true;
                transform.SetParent(crownPosition);
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                gameObject.layer = 16;
                controller.gameObject.layer = 17;
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
                _collider.isTrigger = false;
                ExampleCharacterController parentController = GetComponentInParent<ExampleCharacterController>();
                parentController.gameObject.layer = 18;
                parentController._hasCrown = false;
                transform.SetParent(null);
                _rigidbody.AddForce(Vector3.forward * DROP_FORCE, ForceMode.Impulse);
                gameObject.layer = 15;
                _isPickedUp = false;
                _isOwned = false;
                StartCoroutine(C_CrownDropSlowMotionEffect());
            }
        }

        IEnumerator C_CrownDropSlowMotionEffect()
        {
            float timeScaleIncreaseValue = 0;
            Time.timeScale = 0.01f;
            while (Time.timeScale < 1)
            {
                timeScaleIncreaseValue += 0.1f;
                Time.timeScale += timeScaleIncreaseValue;
                yield return new WaitForSeconds(timeScaleIncreaseValue);
            }
            Time.timeScale = 1.0f;
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