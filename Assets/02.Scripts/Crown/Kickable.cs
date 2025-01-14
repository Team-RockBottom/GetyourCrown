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
            // - ViaServer : �Ϲ������� �����͸� �����ϴ� Ŭ���̾�Ʈ�� ������ ���� ���������ʰ� �״�ν���������, ViaServer �ɼ��� ���� ������ Server ���ؼ� ����
            // - Buffered : Rpc�� �����س���, �ڴʰ� ������ �÷��̾ ���ؼ��� ȣ���ϰ� �Ѵ�. 
            photonView.RPC(nameof(InternalKick), RpcTarget.AllViaServer, force);
        }

        [PunRPC]
        private void InternalKick(Vector3 force)
        {
            _rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }
}
