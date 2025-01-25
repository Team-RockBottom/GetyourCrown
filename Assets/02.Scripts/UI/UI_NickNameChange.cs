using GetyourCrown.Database;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_NickNameChange : UI_Popup
    {
        [Resolve] TMP_InputField _nickNameChange;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;

        private const int MinNicknameLength = 3;
        private const int MaxNicknameLength = 15;

        protected override void Start()
        {
            base.Start();

            _nickNameChange.characterLimit = MaxNicknameLength;

            _confirm.onClick.AddListener(async () =>
            {
                string newNickname = _nickNameChange.text;

                if (string.IsNullOrWhiteSpace(newNickname))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("�г����� �Է����ּ���.");
                    return;
                }

                if (newNickname.Length < MinNicknameLength)
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show($"�г����� {MinNicknameLength}���� �̻��̾�� �մϴ�.");
                    return;
                }

                bool success = await ChangeNickname(newNickname);

                if (success)
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("�г����� ���������� ����Ǿ����ϴ�.");
                    Hide();
                }
                else
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("�г��� ���濡 �����߽��ϴ�.");
                }
            });

            _cancel.onClick.AddListener(Hide);
        }

        public override void Show()
        {
            base.Show();

            _nickNameChange.text = DataManager.instance.Nickname;
        }

        private async Task<bool> ChangeNickname(string newNickname)
        {
            try
            {
                await DataManager.instance.SaveNicknameAsync(newNickname);
                DataManager.instance.CurrentPlayerData.Nickname = newNickname;
                PhotonNetwork.NickName = newNickname;
                return true;
            }
            catch (Exception e)
            {
                Debug.Log(e);
                return false;
            }
        }
    }
}
