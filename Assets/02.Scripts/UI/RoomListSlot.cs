using UnityEngine;
using Practices.UGUI_Management.UI;
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class RoomListSlot : ComponentResolvingBehaviour
    {
        public bool isSelected
        {
            get => _isSelectedValue;
            set
            {
                _isSelectedValue = value;
                _isSelected.gameObject.SetActive(value);
            }
        }

        public int roomId
        {
            get => _roomIdValue;
            set
            {
                _roomIdValue = value;
                _roomId.text = value.ToString();
            }
        }

        public string roomName
        {
            get => _roomNameValue;
            set
            {
                _roomNameValue = value;
                _roomName.text = value.ToString();
            }
        }

        public int roomPlayerCount
        {
            get => _roomPlayerCountValue;
            set
            {
                _roomPlayerCountValue = value;
                _roomPlayerCount.text = value.ToString();
            }
        }

        public int roomMaxPlayers
        {
            get => _roomMaxPlayersValue;
            set
            {
                _roomMaxPlayersValue = value;
                _roomMaxPlayers.text = value.ToString();
            }
        }

        bool _isSelectedValue;
        int _roomIdValue;
        string _roomNameValue;
        int _roomPlayerCountValue;
        int _roomMaxPlayersValue;
        [Resolve] Image _isSelected;
        [Resolve] TMP_Text _roomId;
        [Resolve] TMP_Text _roomName;
        [Resolve] TMP_Text _roomPlayerCount;
        [Resolve] TMP_Text _roomMaxPlayers;
    }
}