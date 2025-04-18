using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine;
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

        public bool isCharacterSelect
        {
            get => _isCharacterSelectOpenValue;
            set
            {
                _isCharacterSelectOpenValue = value;
                _isCharacterSelectOpen.gameObject.SetActive(value);
                _isCharacterSelectOpenImage.gameObject.SetActive(value);
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

        public Texture playerCharacter
        {
            get => _playerCharacter.texture;
            set
            {
                _playerCharacter.texture = value;
            }
        }

        public bool isChat
        {
            get => _isChatValue;
            set
            {
                _isChatValue = value;
                _chat.gameObject.SetActive(value);
            }
        }

        public string chatMessage
        {
            get => _chatMessageValue;
            set
            {
                if (value == null)
                    return;
                _chatMessageValue = value;
                _chatMessage.text = value;
            }
        }

        bool _isReadyValue;
        bool _isCharacterSelectOpenValue;
        string _playerNameValue;
        bool _isMasterClientValue;
        bool _isChatValue;
        string _chatMessageValue;
        [Resolve] TMP_Text _isReady;
        [Resolve] TMP_Text _playerName;
        [Resolve] TMP_Text _isCharacterSelectOpen;
        [Resolve] Image _isMasterClient;
        [Resolve] Image _readyImage;
        [Resolve] Image _isCharacterSelectOpenImage;
        [Resolve] RawImage _playerCharacter;
        [Resolve] Image _chat;
        [Resolve] TMP_Text _chatMessage;
    }
}

