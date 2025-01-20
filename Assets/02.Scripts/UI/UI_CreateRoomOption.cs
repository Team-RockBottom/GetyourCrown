using Photon.Pun;
using Photon.Realtime;
using Practices.UGUI_Management.UI;
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class UI_CreateRoomOption : UI_Popup
    {
        [Resolve] TMP_InputField _roomName;
        [Resolve] TMP_InputField _roomMaxPlayers;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;

        const int ROOM_MAX_PLAYERS_LIMIT_MAX = 8;
        const int ROOM_MAX_PLAYERS_LIMIT_MIN = 1;


        protected override void Start()
        {
            base.Start();

            _roomMaxPlayers.onValueChanged.AddListener(value =>
            {
                if (int.TryParse(value, out int parsed))
                {
                    if (parsed > ROOM_MAX_PLAYERS_LIMIT_MAX)
                        _roomMaxPlayers.SetTextWithoutNotify(ROOM_MAX_PLAYERS_LIMIT_MAX.ToString()); // text property에 쓰면 무한루프걸림.
                    if (parsed < ROOM_MAX_PLAYERS_LIMIT_MIN)
                        _roomMaxPlayers.SetTextWithoutNotify(ROOM_MAX_PLAYERS_LIMIT_MIN.ToString()); // text property에 쓰면 무한루프걸림.
                }
                else
                {
                    _roomMaxPlayers.SetTextWithoutNotify(ROOM_MAX_PLAYERS_LIMIT_MIN.ToString());
                }
            });

            _confirm.onClick.AddListener(() =>
            {
                RoomOptions roomOptions = new RoomOptions();
                roomOptions.MaxPlayers = int.Parse(_roomMaxPlayers.text);
                PhotonNetwork.CreateRoom(_roomName.text, roomOptions);
                Hide();
            });

            _cancel.onClick.AddListener(Hide);
        }

        public override void Show()
        {
            base.Show();

            _roomName.text = string.Empty;
            _roomMaxPlayers.text = ROOM_MAX_PLAYERS_LIMIT_MIN.ToString();
        }
    }
}