using GetyourCrown.UI.UI_Utilities;
using System;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;
using GetyourCrown.Database;

namespace GetyourCrown.UI
{
    public class UI_CreateAccount : UI_Popup
    {
        [Resolve] TMP_InputField _id;
        [Resolve] TMP_InputField _password;
        [Resolve] TMP_InputField _confirmPassword;
        [Resolve] Button _create;
        [Resolve] Button _cancle;

        private const int DEFAULT_CHARACTER_ID = 0;
        protected override async void Start()
        {
            base.Start();

            try
            {
                await UnityServices.Instance.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Unity Services 초기화 실패: {e.Message}");
                ConfirmWindowShow("Unity Services 초기화 중 문제가 발생했습니다.");
            }
            _id.characterLimit = 20;
            _password.characterLimit = 16;
            
        }

        public override void Show()
        {
            base.Show();
                
            _create.onClick.AddListener(async() => 
            {
                if (!IsPasswordValid(_password.text))
                {
                    IsPasswordValid(_password.text);
                    return;
                }

                await CreateAccountAsync(_id.text, _password.text);
                Hide();
            });

            _cancle.onClick.AddListener(Hide);
        }

        async Task CreateAccountAsync(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, password);
                await DefaultUserDataAsync(id, 1000);
                ConfirmWindowShow("계정을 생성하였습니다.");
                _id.text = string.Empty;
                _password.text = string.Empty;
                _confirmPassword.text = string.Empty;
                AuthenticationService.Instance.SignOut(); //계정 생성시 자동 로그인이 되어 다시 로그인하게 하기 위해 로그아웃
                Hide();
            }
            catch (AuthenticationException e)
            {
                Debug.Log($"Authentication failed: {e.Message}"); 
                if (e.Message.Contains("user already has a username/password account linked to it"))
                {
                    Debug.Log($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow("네트워크 혹은 서비스에 문제가 있습니다..");
                }
                else
                {
                    Debug.Log($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow($"계정 생성에 실패하였습니다. {e.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected error: {e.Message}");
                ConfirmWindowShow(e.Message);
            }
        }

        async Task DefaultUserDataAsync(string id, int coins)
        {
            try
            {
                await DataManager.instance.SaveNicknameAsync(id);
                await DataManager.instance.SaveCoinsAsync(coins);
                await DataManager.instance.DefaultCharacterAsync();
                await DataManager.instance.SaveLastCharacterAsync(DEFAULT_CHARACTER_ID);
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected error saving user data: {e.Message}");
                ConfirmWindowShow($"계정 생성 중 데이터를 저장하는 데 실패했습니다.\n {e.Message}");
            }
        }

        void ConfirmWindowShow(string message)
        {
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            uI_ConfirmWindow.Show(message);
        }

        private bool IsPasswordValid(string password)
        {
            if (string.IsNullOrWhiteSpace(_id.text))
            {
                ConfirmWindowShow("닉네임을 입력해주세요.");
                return false;
            }
            else if (string.IsNullOrWhiteSpace(_password.text))
            {
                ConfirmWindowShow("비밀번호를 입력해주세요.");
                return false;
            }
            else if (_password.text != _confirmPassword.text)
            {
                ConfirmWindowShow("비밀번호를 확인해주세요.");
                return false;
            }
            // 비밀번호가 최소 8자, 최대 16자 사이인지 확인
            else if (password.Length < 8 || password.Length > 16)
            {
                ConfirmWindowShow("비밀번호는 8자 이상 16자 이하이어야 합니다.");
                return false;
            }
            // 대문자 포함 여부 확인
            else if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 대문자를 포함해야 합니다.");
                return false;
            }
            // 소문자 포함 여부 확인
            else if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 소문자를 포함해야 합니다.");
                return false;
            }
            // 숫자 포함 여부 확인
            else if (!Regex.IsMatch(password, @"[0-9]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 숫자를 포함해야 합니다.");
                return false;
            }
            // 특수문자 포함 여부 확인
            else if (!Regex.IsMatch(password, @"[\W_]+"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 특수문자를 포함해야 합니다.");
                return false;
            }
            // 모든 조건을 만족하면 유효함
            return true;
        }
    }
}
