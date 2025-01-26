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

        protected override async void Start()
        {
            base.Start();

            try
            {
                await UnityServices.Instance.InitializeAsync();
            }
            catch (Exception e)
            {
                Debug.LogError($"Unity Services �ʱ�ȭ ����: {e.Message}");
                ConfirmWindowShow("Unity Services �ʱ�ȭ �� ������ �߻��߽��ϴ�.");
            }
            _id.characterLimit = 20;
            _password.characterLimit = 16;
            
        }

        public override void Show()
        {
            base.Show();
                
            _create.onClick.AddListener(async() => 
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
                else if (_password.text != _confirmPassword.text)
                {
                    ConfirmWindowShow("��й�ȣ�� Ȯ�����ּ���.");
                }
                else if (!IsPasswordValid(_password.text))
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
                ConfirmWindowShow("������ �����Ͽ����ϴ�.");
                _id.text = string.Empty;
                _password.text = string.Empty;
                _confirmPassword.text = string.Empty;
                AuthenticationService.Instance.SignOut(); //���� ������ �ڵ� �α����� �Ǿ� �ٽ� �α����ϰ� �ϱ� ���� �α׾ƿ�
                Hide();
            }
            catch (AuthenticationException e)
            {
                Debug.Log($"Authentication failed: {e.Message}"); 
                if (e.Message.Contains("user already has a username/password account linked to it"))
                {
                    Debug.Log($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow("��Ʈ��ũ Ȥ�� ���񽺿� ������ �ֽ��ϴ�..");
                }
                else
                {
                    Debug.Log($"AuthenticationException e: {e.Message}");
                    ConfirmWindowShow($"���� ������ �����Ͽ����ϴ�. {e.Message}");
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
            }
            catch (Exception e)
            {
                Debug.Log($"Unexpected error saving user data: {e.Message}");
                ConfirmWindowShow($"���� ���� �� �����͸� �����ϴ� �� �����߽��ϴ�.\n {e.Message}");
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
