using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_CreateRoom : UI_Popup
    {
        [Resolve] TMP_InputField _roomName;
        [Resolve] TMP_InputField _roomMaxPlayers;
        [Resolve] Button _confirm;
        [Resolve] Button _cancle;

        const int ROOM_PLAYER_LIMIT_MAX = 4;
        const int ROOM_PLAYER_LIMIT_MIN = 2;

        protected override void Start()
        {
            base.Start();

            _roomMaxPlayers.onValueChanged.AddListener(value =>
            {
                if (int.TryParse(value, out int paresd))
                {
                    if (paresd > ROOM_PLAYER_LIMIT_MAX)
                        _roomMaxPlayers.SetTextWithoutNotify(ROOM_PLAYER_LIMIT_MAX.ToString());
                    if (paresd < ROOM_PLAYER_LIMIT_MIN)
                        _roomMaxPlayers.SetTextWithoutNotify(ROOM_PLAYER_LIMIT_MIN.ToString());
                }
                else
                {
                    _roomMaxPlayers.SetTextWithoutNotify(ROOM_PLAYER_LIMIT_MIN.ToString());
                }
            });
            
            UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();

            _confirm.onClick.AddListener(() =>
            {
                if (string.IsNullOrWhiteSpace(_roomName.text))
                {
                    uI_ConfirmWindow.Show("规 力格阑 涝仿秦林技夸.");
                    return;
                }

                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = int.Parse(_roomMaxPlayers.text);
                PhotonNetwork.CreateRoom(_roomName.text, roomOptions);
                Hide();
            });

            _cancle.onClick.AddListener(Hide);
        }

        public override void Show()
        {
            base.Show();

            _roomName.text = string.Empty;
            _roomMaxPlayers.text = ROOM_PLAYER_LIMIT_MIN.ToString();
        }
    }
}

