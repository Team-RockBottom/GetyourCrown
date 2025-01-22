using GetyourCrown.UI.UI_Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_Login : UI_Popup
    {
        [Resolve] InputField _id;
        [Resolve] InputField _password;
        [Resolve] Button _login;
        [Resolve] Button _create;
        [Resolve] Button _guest;
        [Resolve] Button _exit;

        protected override void Start()
        {
            base.Start();
        }

        public void Show()
        {
            base.Show();

            _login.onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(_id.text))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("닉네임을 입력해주세요.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(_password.text))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("비밀번호를 입력해주세요.");
                    return;
                }
                /*else if()
                {
                    // todo => 로그인 계정이나 비밀번호가 맞지 않으면 컴펌 윈도우 
                }*/
                Hide();
            });

            _create.onClick.AddListener(() =>
            {
                UI_CreateAccount _uiCreateAccount = UI_Manager.instance.Resolve<UI_CreateAccount>();
                _uiCreateAccount.Show();
            });

            _guest.onClick.AddListener(() =>
            {
                UI_MainMenu _uiMainMenu = UI_Manager.instance.Resolve<UI_MainMenu>();
                _uiMainMenu.Show();
            });

            _exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }
    }
}

