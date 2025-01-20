using UnityEngine;
using Practices.UGUI_Management.Utilities;
using TMPro;
using UnityEngine.UI;

namespace Practices.PhotonPunClient.UI
{
    public class RoomPlayerInfoSlot : ComponentResolvingBehaviour
    {
        public int actorNumber { get; set; }

        public bool isReady
        {
            get => _isReadyValue;
            set
            {
                _isReadyValue = value;
                _isReady.gameObject.SetActive(value);
            }
        }

        public string playerName
        {
            get => _playerNameValue;
            set
            {
                _playerName.text = value;
            }
        }

        public bool isMasterClient
        {
            get => _isMasterClientValue;
            set
            {
                _isMasterClientValue = value;
                _isMasterClient.gameObject.SetActive(value);
            }
        }


        bool _isReadyValue;
        string _playerNameValue;
        bool _isMasterClientValue;
        [Resolve] TMP_Text _isReady;
        [Resolve] TMP_Text _playerName;
        [Resolve] Image _isMasterClient;
    }
}