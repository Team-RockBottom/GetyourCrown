using GetyourCrown.UI.UI_Utilities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine.UI;
using UnityEngine;
using System.Text.RegularExpressions;

namespace GetyourCrown.UI
{
    public class UI_CreateAccount : UI_Popup
    {
        [Resolve] TMP_InputField _id;
        [Resolve] TMP_InputField _password;
        [Resolve] TMP_InputField _confirmPassword;
        [Resolve] Button _create;
        [Resolve] Button _cancle;

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

        public override async void Show()
        {
            base.Show();
                
            _create.onClick.AddListener(() => 
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
                else if (_password.text != _confirmPassword.text)
                {
                    ConfirmWindowShow("비밀번호를 확인해주세요.");
                }
                else if (!IsPasswordValid(_password.text))
                {
                    IsPasswordValid(_password.text);
                    return;
                }


                CreateAccountAsync(_id.text, _password.text);
                Hide();
            });

            _cancle.onClick.AddListener(Hide);
        }

        async Task CreateAccountAsync(string id, string password)
        {
            try
            {
                await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(id, password);
                ConfirmWindowShow("계정을 생성하였습니다.");
                _id.text = string.Empty;
                _password.text = string.Empty;
                _confirmPassword.text = string.Empty;
                SaveUserDatAsync(id);
                AuthenticationService.Instance.SignOut(); //계정 생성시 자동로그인이 되어 다시 로그인하게 하기 위해 로그아웃
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

        async Task SaveUserDatAsync(string id)
        {
            try
            {
                var userData = new Dictionary<string, object>()
                {
                    { "User", id },
                    { "Coin", 1000},
                };

                await CloudSaveService.Instance.Data.ForceSaveAsync(userData);

            }
            catch (AuthenticationException e)
            {
                Debug.Log($"Authentication failed: {e.Message}");
                if (e.Message.Contains("user already has a username/password account linked to it"))
                {
                    ConfirmWindowShow("이미 사용 중인 아이디입니다.");
                }
                else
                {
                    ConfirmWindowShow($"계정 생성에 실패하였습니다: {e.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected error: {e.Message}");
                ConfirmWindowShow("계정 생성 중 예상치 못한 오류가 발생하였습니다.");
            }
        }

        void ConfirmWindowShow(string message)
        {
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            uI_ConfirmWindow.Show(message);
        }

        private bool IsPasswordValid(string password)
        {
            // 비밀번호가 최소 8자, 최대 16자 사이인지 확인
            if (password.Length < 8 || password.Length > 16)
            {
                ConfirmWindowShow("비밀번호는 8자 이상 16자 이하이어야 합니다.");
                return false;
            }
            // 대문자 포함 여부 확인
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 대문자를 포함해야 합니다.");
                return false;
            }
            // 소문자 포함 여부 확인
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 소문자를 포함해야 합니다.");
                return false;
            }
            // 숫자 포함 여부 확인
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 숫자를 포함해야 합니다.");
                return false;
            }
            // 특수문자 포함 여부 확인
            if (!Regex.IsMatch(password, @"[\W_]+"))
            {
                ConfirmWindowShow("비밀번호는 최소 1개의 특수문자를 포함해야 합니다.");
                return false;
            }
            // 모든 조건을 만족하면 유효함
            return true;
        }
    }
}
