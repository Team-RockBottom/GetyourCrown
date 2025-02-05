using GetyourCrown.Database;
using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_Login : UI_Popup
    {
        [Resolve] TMP_InputField _id;
        [Resolve] TMP_InputField _password;
        [Resolve] Button _login;
        [Resolve] Button _create;
        [Resolve] Button _guest;
        [Resolve] Button _exit;
        private const string IS_LOGIIN = "IsLogIn";

        protected override async void Start()
        {
            base.Start();
            _guest.interactable = false;
            _id.characterLimit = 20;
            _password.characterLimit = 16;

            await UnityServices.InitializeAsync();

            _login.onClick.AddListener(async () =>
            {

                if (string.IsNullOrWhiteSpace(_id.text))
                {
                    ConfirmWindowShow("닉네임을 입력해주세요.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(_password.text))
                {
                    ConfirmWindowShow("비밀번호를 입력해주세요.");
                    return;
                }

                bool checkLogin = await LogInAsync(_id.text, _password.text);
                if (checkLogin)
                {
                    await DataManager.instance.LoadPlayerDataAsync();
                    PhotonManager.instance.SetNickname(DataManager.instance.Nickname);
                }
            });

            _create.onClick.AddListener(() =>
            {
                UI_CreateAccount _uiCreateAccount = UI_Manager.instance.Resolve<UI_CreateAccount>();
                _uiCreateAccount.Show();
            });

            //_guest.onClick.AddListener(async() =>
            //{
            //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //    DataManager.instance.isGuest = true;
            //    DataManager.instance.GuestData();
            //    Hide();
            //});

            _exit.onClick.AddListener(() =>
            {
                Application.Quit();
            });
        }

        public override void Show()
        {
            base.Show();
        }

        void ConfirmWindowShow(string message)
        {
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            uI_ConfirmWindow.Show(message);
        }

        async Task<bool> LogInAsync(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(_id.text, _password.text);
                Hide();
                PlayerPrefs.SetInt(IS_LOGIIN, 1);
                PlayerPrefs.Save();
                return true;
            }
            catch (RequestFailedException e)
            {
                if (e.Message.Contains("WRONG_USERNAME_PASSWORD"))
                {
                    ConfirmWindowShow("아이디 또는 비밀번호가 잘못되었습니다.");
                    _password.text = string.Empty;
                }

                else if (e.Message.Contains("Invalid username or password"))
                {
                    ConfirmWindowShow("아이디 또는 비밀번호가 잘못되었습니다.");
                    _password.text = string.Empty;
                }

                else
                {
                    ConfirmWindowShow($"알 수 없는 오류가 발생했습니다.\n다시 시도 해주세요.\nErrorCode: {e.ErrorCode}");
                    Debug.Log($"{e.ErrorCode}");
                }
                return false;
            }
        }
    }
}

