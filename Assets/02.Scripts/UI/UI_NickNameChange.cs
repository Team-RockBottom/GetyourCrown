using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_NickNameChange : UI_Popup
    {
        [Resolve] TMP_InputField _nickNameChange;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;

        protected override void Start()
        {
            base.Start();

            _confirm.onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(_nickNameChange.text))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("�г����� �Է����ּ���.");
                    return;
                }

                PhotonNetwork.NickName = _nickNameChange.text;
                UI_Lobby uI_Lobby = UI_Manager.instance.Resolve<UI_Lobby>();
                uI_Lobby.NickNameChange();
                Hide();
            });

            _cancel.onClick.AddListener(Hide);
        }

        public override void Show()
        {
            base.Show();

            _nickNameChange.text = PhotonNetwork.NickName;
        }
    }
}
