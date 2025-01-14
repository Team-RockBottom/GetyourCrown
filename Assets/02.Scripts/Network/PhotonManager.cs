using Photon.Pun;
using UnityEngine;

namespace GetyourCrown.Network
{
    public class PhotonManager : MonoBehaviourPunCallbacks
    {
        public static PhotonManager instance
        {
            get
            {
                if (s_instance == null)
                {
                    s_instance = new GameObject(nameof(PhotonManager)).AddComponent<PhotonManager>();
                }

                return s_instance;
            }
        }

        static PhotonManager s_instance;

        private void Awake()
        {
            if (s_instance)
            {
                Destroy(gameObject);
                return;
            }
            else
            {
                s_instance = this;
            }

            if (PhotonNetwork.IsConnected == false)
            {
#if UNITY_EDITOR
                PhotonNetwork.LogLevel = PunLogLevel.Full;
                Application.runInBackground = true;
#endif
                PhotonNetwork.AuthValues = new Photon.Realtime.AuthenticationValues(Random.Range(0, 999999999).ToString());
                PhotonNetwork.NickName = "Guest" + Random.Range(0, 100).ToString();
                bool isConnected = PhotonNetwork.ConnectUsingSettings();
                Debug.Assert(isConnected, $"[{nameof(PhotonManager)}] Failed to connect to photon pun server");
            }

            DontDestroyOnLoad(gameObject);
        }

        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();

            PhotonNetwork.AutomaticallySyncScene = true;
            Debug.Log($"[{nameof(PhotonManager)}] Connected to master server");
            PhotonNetwork.JoinLobby();
        }

        public override void OnJoinedLobby()
        {
            base.OnJoinedLobby();
            Debug.Log($"[{nameof(PhotonManager)}] Joined Lobby");
        }
    }
}
