using GetyourCrown.Database;
using GetyourCrown.UI.UI_Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_CharacterSelect : UI_Popup
    {
        [Resolve] Button _close;
        [Resolve] RectTransform _characterSlotContent;
        [Resolve] RectTransform _characterPreviewPos;
        [Resolve] RawImage _characterPreview;
        GameObject _currentCharacter;
        List<CharacterSlot> _characterSlots = new List<CharacterSlot>(DEFAULT_CHARACTER_LIST_SIZE);
        CharacterSpec[] _characterSpecs;
        Camera _characterCamera;
        RenderTexture _renderTexture;

        string _characterSpecFolder = "CharacterSpecs";
        string _charaterSlotPrefab = "UI/CharacterSlot";

        const int DEFAULT_CHARACTERSELECT = 0;
        const int DEFAULT_CHARACTER_LIST_SIZE = 20;

        protected void Start()
        {
            base.Start();

            playerInputActions.UI.Click.performed += CharacterDrag;
        }
        public void Show(UnityAction onConfirmed = null)
        {
            base.Show();

            _close.onClick.RemoveAllListeners();
            _close.onClick.AddListener(Hide);

            if (onConfirmed != null)
                _close.onClick.AddListener(onConfirmed);

            CharacterSlot characterSlot = Resources.Load<CharacterSlot>(_charaterSlotPrefab);
            CharacterSlot slot = Instantiate(characterSlot, _characterSlotContent);

            LoadCharacterSpecs();
            LoadCharacterSlot();
            SelectedCharacterPreview(_characterSpecs[DEFAULT_CHARACTERSELECT]);
        }

        private void LoadCharacterSpecs()
        {
            _characterSpecs = Resources.LoadAll<CharacterSpec>(_characterSpecFolder);
        }

        private void LoadCharacterSlot()
        {
            //ĳ���� ������ �̹� ���� ���ִٸ� ����
            //����: resolve�� �ϰ� �ְ� Destroy�� �ϰ� �Ǹ� �ι�° ĳ���� ������ ������ ������ �߻�
            if (_characterSlots.Count == _characterSpecs.Length)
                return;


            for (int i = 0; i < _characterSpecs.Length; i++)
            {
                CharacterSlot characterSlot = Resources.Load<CharacterSlot>(_charaterSlotPrefab);
                CharacterSlot slot = Instantiate(characterSlot, _characterSlotContent);

                slot.CharacterIndex = _characterSpecs[i].id;
                slot.CharacterImage = _characterSpecs[i].sprite;
                slot.isSelected = false;
                slot.OnCharacterSelect += CharacterSelected;
                _characterSlots.Add(slot);
            }

            if (_characterSlots.Count > 0)
            {
                _characterSlots[DEFAULT_CHARACTERSELECT].isSelected = true;
            }
        }

        private void CharacterSelected(CharacterSlot selectCharacter)
        {
            foreach (var slot in _characterSlots)
            {
                slot.isSelected = false;
            }

            selectCharacter.isSelected = true;

            CharacterSpec selectedCharacter = _characterSpecs[selectCharacter.CharacterIndex];

            SelectedCharacterPreview(selectedCharacter);
        }

        private void SelectedCharacterPreview(CharacterSpec spec)
        {
            //�ٽ� �����ϸ� ĳ���� ����
            if (_currentCharacter != null)
                Destroy(_currentCharacter);

            if (spec.prefab != null)
            {
                _currentCharacter = Instantiate(spec.prefab, _characterPreviewPos); //ĳ���� ����
                _currentCharacter.transform.localPosition = new Vector3(0, -1, 0);
                _currentCharacter.transform.localScale = Vector3.one; //ĳ���� ũ�� ����
                _currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0); //ĳ���� ȸ�� (ī�޶� �°� ����)


                // ĳ���͸� �������� ī�޶� ����
                if (_characterCamera == null)
                {
                    // RawImage ���� ���� ī�޶� ����
                    _characterCamera = new GameObject("CharacterCamera").AddComponent<Camera>();
                    // ī�޶��� �⺻ ����� �����ʰ� ���� �����Ǿ� ����ī�޶�� ���ļ� ĳ����ī�޶��� ����� ������ ����
                    AudioListener audioListener = _characterCamera.GetComponent<AudioListener>();

                    if (audioListener != null)
                    {
                        Destroy(audioListener);  // ����� ������ ����
                    }

                    _renderTexture = new RenderTexture(512, 512, 32); //���� �ػ� ����
                    _characterCamera.targetTexture = _renderTexture;  // ī�޶��� ���� Ÿ���� RenderTexture�� ����
                    _characterPreview.texture = _renderTexture;  // RawImage�� ������ �ؽ�ó ����
                }

                _characterCamera.clearFlags = CameraClearFlags.SolidColor;  // Skybox�� �����ϰ� ����

                // ī�޶� �������� �����Ͽ� ĳ���͸� RawImage�� ǥ��
                _characterCamera.Render();
            }
        }

        void CharacterDrag(InputAction.CallbackContext context)
        {
            
        }
    }
}
