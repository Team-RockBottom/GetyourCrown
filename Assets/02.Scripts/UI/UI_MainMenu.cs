using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.CompilerServices;

namespace GetyourCrown.UI
{
    public class UI_MainMenu : UI_Screen
    {
        [Resolve] Button _start;
        [Resolve] Button _option;
        [Resolve] Button _exit;
        private const string IS_LOGIIN = "IsLogIn";

        protected override void Start()
        {
            base.Start();

            bool isLogIn = PlayerPrefs.GetInt(IS_LOGIIN, 0) == 1;
            if (!isLogIn)
            {
                UI_Login uI_Login = UI_Manager.instance.Resolve<UI_Login>();
                uI_Login.Show();
            }

            _start.onClick.AddListener(() =>
            {
                StartCoroutine(C_NetworkConnection());
                Hide();
            });

            _option.onClick.AddListener(() =>
            {
                UI_Option uI_Option = UI_Manager.instance.Resolve<UI_Option>();
                uI_Option.Show();
            });

            _exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        IEnumerator C_NetworkConnection()
        {
            UI_Manager ui_Manager = UI_Manager.instance;

            yield return new WaitUntil(() => PhotonNetwork.IsConnected);

            ui_Manager.Resolve<UI_Lobby>().Show();
            ui_Manager.Resolve<UI_Room>().Hide();
        }
    }  
}
