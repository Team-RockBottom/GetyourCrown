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
            UI_Manager manager = UI_Manager.instance;

            yield return new WaitUntil(() => PhotonNetwork.IsConnected);

            manager.Resolve<UI_Lobby>().Show();
        }
    }
}
