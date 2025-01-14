using Photon.Pun;
using UnityEngine;

namespace Practices.PhotonPunClient.Network
{
    [RequireComponent(typeof(PhotonRigidbodyView))]
    public class Kickable : PunAutoSyncMonobehaviour
    {
        Rigidbody _rigidbody;


        protected override void Awake()
        {
            base.Awake();

            _rigidbody = GetComponent<Rigidbody>();
        }

        public void Kick(Vector3 force)
        {
            // RpcTarget 
            // - ViaServer : 일반적으로 데이터를 전송하는 클라이언트는 서버를 통해 수신하지않고 그대로실행하지만, ViaServer 옵션을 쓰면 무조건 Server 통해서 수행
            // - Buffered : Rpc를 저장해놓고, 뒤늦게 참여한 플레이어에 대해서도 호출하게 한다. 
            photonView.RPC(nameof(InternalKick), RpcTarget.AllViaServer, force);
        }

        [PunRPC]
        private void InternalKick(Vector3 force)
        {
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
