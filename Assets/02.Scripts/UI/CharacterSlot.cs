using GetyourCrown.UI.UI_Utilities;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class CharacterSlot : ComponentResolvingBehaviour
    {
        public int index { get; set; }

        public int CharacterIndex
        {
            get => index;
            set => index = value;
        }

        public Sprite CharacterImage
        {
            get => _characterSelectButton.GetComponent<Image>().sprite; 
            set => _characterSelectButton.GetComponent<Image>().sprite = value;
        }

        public bool isSelected
        {
            get => _isSelectedValue;
            set
            {
                _isSelectedValue = value;
                _isSelected.gameObject.SetActive(value);
            }
        }

        public event Action<CharacterSlot> OnCharacterSelect;

        bool _isSelectedValue;
        int _characterId;
        [Resolve] Button _characterSelectButton;
        [Resolve] Image _isSelected;

        /// <summary>
        /// 생성 되면서 버튼에 구독
        /// </summary>
        private void OnEnable()
        {
            _characterSelectButton.onClick.RemoveAllListeners();
            _characterSelectButton.onClick.AddListener(CharacterSelected);
        }

        void CharacterSelected()
        {
            OnCharacterSelect?.Invoke(this);
        }
    }
}
