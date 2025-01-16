using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine.UI;

namespace GetyourCrown.UI
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
                _readyImage.gameObject.SetActive(value);
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
        [Resolve] Image _readyImage;
        //캐릭터 프리펩 넣기
    }
}

