using GetyourCrown.UI.UI_Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_MainMenu : UI_Screen
    {
        [Resolve] Button _start;
        [Resolve] Button _option;
        [Resolve] Button _exit;

        protected override void Start()
        {
            base.Start();

            _start.onClick.AddListener(() =>
            {
                 SceneManager.LoadScene("TestScene - UI_MultiPlay");
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
    }  
}
