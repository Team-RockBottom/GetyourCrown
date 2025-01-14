using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace Practices.PhotonPunClient.Network
{
    /// <summary>
    /// 레벨에 미리 배치해둬야하는 동기화필요한 NetworkObject 들은
    /// PhotonNetwork.Instantiate 보다 단순 이벤트로 ViewID 만 동기화해주는것이 성능이 좋다. 
    /// (이미 동일한 GameObject 들로 생성이 되어있기 때문에)
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public abstract class PunAutoSyncMonobehaviour : MonoBehaviour, IOnEventCallback
    {
        protected PhotonView photonView;

        protected virtual void Awake()
        {
            photonView = GetComponent<PhotonView>();
            SyncViewID();
        }

        protected virtual void OnEnable()
        {
            PhotonNetwork.AddCallbackTarget(this);
        }

        protected virtual void OnDisable()
        {
            PhotonNetwork.RemoveCallbackTarget(this);
        }

        void SyncViewID()
        {
            if (PhotonNetwork.IsMasterClient == false)
                return;

            if (photonView.Owner != null)
                return;

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                object raiseEventContent = new object[]
                {
                    photonView.ViewID,
                };

                RaiseEventOptions raiseEventOption = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                };

                PhotonNetwork.RaiseEvent(PhotonEventCode.SYNC_VIEW_ID,
                                         raiseEventContent,
                                         raiseEventOption,
                                         SendOptions.SendReliable);
            }
            else
            {
                throw new System.Exception($"[{nameof(PunAutoSyncMonobehaviour)}] Failed to sync view id...");
            }
        }

        public void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;

            if (eventCode == PhotonEventCode.SYNC_VIEW_ID)
                HandleSyncViewIdEvent(photonEvent);
        }

        void HandleSyncViewIdEvent(EventData photonEvent) 
        {
            object[] data = (object[])photonEvent.CustomData;
            int viewId = (int)data[0];
            photonView.ViewID = viewId;
        }
    }
}