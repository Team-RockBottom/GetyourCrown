using GetyourCrown.UI;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace GetyourCrown.Workflow
{
    public class UI_TestWorkflow : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(C_TestWorkflow());    
        }

        IEnumerator C_TestWorkflow()
        {
            UI_Manager ui_Manager = UI_Manager.instance;

            yield return new WaitUntil(() => PhotonNetwork.IsConnected);

            ui_Manager.Resolve<UI_Lobby>().Show();
            ui_Manager.Resolve<UI_Room>().Hide();
        }
    }
}
