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

            await UnityServices.Instance.InitializeAsync();
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
                if (_id.text.Length < 5)
                {
                    ConfirmWindowShow("아이디: 5 ~ 20자만 사용 가능합니다.");
                    return;
                }
                else if (_password.text.Length < 8)
                {
                    ConfirmWindowShow("비밀번호: 8 ~ 16자만 사용 가능합니다.");
                    return;
                }
                else if (_password.text != _confirmPassword.text)
                {
                    ConfirmWindowShow("비밀번호를 확인해주세요.");
                    return;
                }

                CreateAccount(_id.text, _password.text);
                SaveUserData(_id.text);
                Hide();
            });

            _cancle.onClick.AddListener(Hide);
        }

        async Task CreateAccount(string id, string password)
        {
            if (!IsPasswordValid(password))
            {
                ConfirmWindowShow("비밀번호 조건을 만족하지 못합니다.");
                return;
            }

            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                await AuthenticationService.Instance.AddUsernamePasswordAsync(id, password);

                ConfirmWindowShow("계정을 생성하였ㅊ다.");
                _id.text = string.Empty;
                _password.text = string.Empty;
                _confirmPassword.text = string.Empty;
                Hide();
            }
            catch (AuthenticationException e)
            {
                Debug.LogError($"Authentication failed: {e.Message}"); 
                if (e.Message.Contains("user already has a username/password account linked to it"))
                {
                    Debug.LogError($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow("이미 사용 중인 아이디입니다.");
                }
                else
                {
                    Debug.LogError($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow($"계정 생성에 실패하였습니다. {e.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error: {e.Message}");
                ConfirmWindowShow("익명 로그인에 실패하였습니다. 네트워크를 확인해주세요.");
            }
        }

        async Task SaveUserData(string id)
        {
            try
            {
                await UnityServices.Instance.InitializeAsync();

                var userData = new Dictionary<string, object>()
                {
                    { "User", id },
                    { "Coin", 1000},
                };
                    
                await CloudSaveService.Instance.Data.ForceSaveAsync(userData);
            }
            catch (Exception e)
            {
                Debug.LogError($"CloudSave error: {e.Message}");  // 오류 메시지 로그 추가
                ConfirmWindowShow("클라우드에 세이브 실패하였습니다.");
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
