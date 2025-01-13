using GetYourCrown.UI.Utility;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GetYourCrown.UI
{
    public class UI_MainMenu : UI_Screan
    {
        [Resolve] Button _start;
        [Resolve] Button _option;
        [Resolve] Button _exit;

        protected override void Start()
        {
            base.Start();

            _start.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("Lobby");
            });

            _exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }  
}
