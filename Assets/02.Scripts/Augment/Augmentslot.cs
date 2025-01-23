using UnityEngine;
using TMPro;
using UnityEngine.UI;
using GetyourCrown.Network;
using Photon.Pun;
using GetyourCrown.CharacterContorller;
using Photon.Realtime;

namespace Augment
{
    public class Augmentslot : MonoBehaviour
    {
        [SerializeField] TMP_Text _nameText;
        [SerializeField] TMP_Text _descriptionText;
        [SerializeField] Image _iconImage;
        //[SerializeField] Button _button;
        private int _idValue;
        private string _descriptionValue;
        private string _nameValue;

        public Sprite iconimage
        {
            get => _iconImage.sprite;
            set => _iconImage.sprite = value;
        }

        public int id
        {
            get => _idValue;
            set => _idValue = value;
        }

        public string descriptionValue
        {
            get => _descriptionValue;
            set => _descriptionText.text = value;
        }

        public string nameValue
        {
            get => _nameValue;
            set => _nameText.text = value;
        }

        private void Awake()
        {
            //_button.onClick.AddListener(TransferSelectedAugmentId);
        }

        private void TransferSelectedAugmentId()
        {
            //TODO >> 자기 자신의 Charactercontroller 인지 확인하고 맞다면 Id 전달하기

            //controller.AugmentDataReceive(_idValue);
        }
    }
}
