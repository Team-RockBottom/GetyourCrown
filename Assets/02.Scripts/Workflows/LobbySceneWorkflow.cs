using Photon.Pun;
using Practices.PhotonPunClient.UI;
using Practices.UGUI_Management.UI;
using System.Collections;
using UnityEngine;

namespace Practices.GameClient.Workflows
{
    public class LobbySceneWorkflow : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(C_Workflow());
        }

        IEnumerator C_Workflow()
        {
            UI_Manager uiManager = UI_Manager.instance;

            // Photon server 에 접속완료 될때까지 기다림.
            yield return new WaitUntil(() => PhotonNetwork.IsConnected);

            uiManager.Resolve<UI_Lobby>()
                     .Show();
        }
    }
}