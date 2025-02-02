using GetyourCrown.UI.UI_Utilities;
using System;
using TMPro;
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

        public int CharacterPrice
        {
            get => _characterPriceValue;
            set
            {
                _characterPriceValue = value;
                //_characterPrice.text = _characterPriceValue.ToString();
            }
        }
        
        public bool CharacterLocked
        {
            get => _isLockedValue;
            set
            {
                _isLockedValue = value;
                _isLocked.gameObject.SetActive(value);
            }
        }

        public event Action<CharacterSlot> OnCharacterSelect;

        bool _isSelectedValue;
        int _characterPriceValue;
        bool _isLockedValue;
        [Resolve] Button _characterSelectButton;
        [Resolve] Image _isSelected;
        //[Resolve] TMP_Text _characterPrice;
        [Resolve] Image _isLocked;

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
            if (_isLockedValue)
            {
                UI_CharacterBuy _uiCharacterBuy = UI_Manager.instance.Resolve<UI_CharacterBuy>();
                _uiCharacterBuy.CharacterInfo(CharacterImage, CharacterPrice);
                _uiCharacterBuy.Show();
            }

            OnCharacterSelect?.Invoke(this);
        }
    }
}
