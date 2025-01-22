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
                    ConfirmWindowShow("�г����� �Է����ּ���.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(_password.text))
                {
                    ConfirmWindowShow("��й�ȣ�� �Է����ּ���.");
                    return;
                }
                if (_id.text.Length < 5)
                {
                    ConfirmWindowShow("���̵�: 5 ~ 20�ڸ� ��� �����մϴ�.");
                    return;
                }
                else if (_password.text.Length < 8)
                {
                    ConfirmWindowShow("��й�ȣ: 8 ~ 16�ڸ� ��� �����մϴ�.");
                    return;
                }
                else if (_password.text != _confirmPassword.text)
                {
                    ConfirmWindowShow("��й�ȣ�� Ȯ�����ּ���.");
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
                ConfirmWindowShow("��й�ȣ ������ �������� ���մϴ�.");
                return;
            }

            try
            {
                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();
                }

                await AuthenticationService.Instance.AddUsernamePasswordAsync(id, password);

                ConfirmWindowShow("������ �����Ͽ�����.");
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
                    ConfirmWindowShow("�̹� ��� ���� ���̵��Դϴ�.");
                }
                else
                {
                    Debug.LogError($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow($"���� ������ �����Ͽ����ϴ�. {e.Message}");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unexpected error: {e.Message}");
                ConfirmWindowShow("�͸� �α��ο� �����Ͽ����ϴ�. ��Ʈ��ũ�� Ȯ�����ּ���.");
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
                Debug.LogError($"CloudSave error: {e.Message}");  // ���� �޽��� �α� �߰�
                ConfirmWindowShow("Ŭ���忡 ���̺� �����Ͽ����ϴ�.");
            }
        }

        void ConfirmWindowShow(string message)
        {
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
            uI_ConfirmWindow.Show(message);
        }

        private bool IsPasswordValid(string password)
        {
            // ��й�ȣ�� �ּ� 8��, �ִ� 16�� �������� Ȯ��
            if (password.Length < 8 || password.Length > 16)
            {
                ConfirmWindowShow("��й�ȣ�� 8�� �̻� 16�� �����̾�� �մϴ�.");
                return false;
            }
            // �빮�� ���� ���� Ȯ��
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                ConfirmWindowShow("��й�ȣ�� �ּ� 1���� �빮�ڸ� �����ؾ� �մϴ�.");
                return false;
            }
            // �ҹ��� ���� ���� Ȯ��
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                ConfirmWindowShow("��й�ȣ�� �ּ� 1���� �ҹ��ڸ� �����ؾ� �մϴ�.");
                return false;
            }
            // ���� ���� ���� Ȯ��
            if (!Regex.IsMatch(password, @"[0-9]"))
            {
                ConfirmWindowShow("��й�ȣ�� �ּ� 1���� ���ڸ� �����ؾ� �մϴ�.");
                return false;
            }
            // Ư������ ���� ���� Ȯ��
            if (!Regex.IsMatch(password, @"[\W_]+"))
            {
                ConfirmWindowShow("��й�ȣ�� �ּ� 1���� Ư�����ڸ� �����ؾ� �մϴ�.");
                return false;
            }
            // ��� ������ �����ϸ� ��ȿ��
            return true;
        }
    }
}
