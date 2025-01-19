using ExitGames.Client.Photon;
using GetyourCrown.Database;
using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
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
        [Resolve] CharacterSlot _characterSlot;
        [Resolve] RawImage _characterPreview;
        GameObject _currentCharacter; 
        List<CharacterSlot> _characterSlots = new List<CharacterSlot>(DEFAULT_CHARACTER_LIST_SIZE);
        CharacterSpec[] _characterSpecs;
        Camera _characterCamera;
        RenderTexture _renderTexture;
        UI_Room _uiRoom;

        string _characterSpecFolder = "CharacterSpecs";
        bool isFirst = true;

        const int DEFAULT_CHARACTERSELECT = 0;
        const int DEFAULT_CHARACTER_LIST_SIZE = 20;

        protected override void Start()
        {
            base.Start();

            _uiRoom = FindFirstObjectByType<UI_Room>();
        }

        public void Show(UnityAction onConfirmed = null)
        {
            base.Show();

            _close.onClick.RemoveAllListeners();
            _close.onClick.AddListener(() =>
            {
                Player player = PhotonNetwork.LocalPlayer;
                bool isCharacterSelection = _uiRoom._roomPlayerInfoPairs[player.ActorNumber].slot.isCharacterSelect;

                player.SetCustomProperties(new Hashtable()
                {
                    { PlayerInRoomProperty.SHOW_CHARACTER_SELECTION, isCharacterSelection == false},
                });

                Hide();
            });

            _characterSlot.gameObject.SetActive(false);
            if (onConfirmed != null)
                _close.onClick.AddListener(onConfirmed);

            LoadCharacterSpecs();
            LoadCharacterSlot();

            if (isFirst)
            {
                SelectedCharacterPreview(_characterSpecs[DEFAULT_CHARACTERSELECT]);
                isFirst = false;
            }
        }

        private void LoadCharacterSpecs()
        {
            _characterSpecs = Resources.LoadAll<CharacterSpec>(_characterSpecFolder);

        }

        private void LoadCharacterSlot()
        {
            if (_characterSlots.Count == _characterSpecs.Length)
                return;

            for (int i = 0; i < _characterSpecs.Length; i++)
            {
                CharacterSlot slot = Instantiate(_characterSlot, _characterSlotContent);
                slot.gameObject.SetActive(true);
                slot.CharacterIndex = _characterSpecs[i].id;
                slot.CharacterImage = _characterSpecs[i].sprite;
                slot.isSelected = false;
                slot.OnCharacterSelect += CharacterSelected;
                slot.gameObject.SetActive(true);
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

            int selectedCharacterId = selectCharacter.CharacterIndex; // ���õ� ĳ����ID ����
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { PlayerInRoomProperty.CHARACTER_ID, selectedCharacterId }  // ĳ���;��̵� Ű�� ���õ� ĳ����ID ���� �� ����ȭ
            });

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
                _currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0); //ĳ���� ȸ�� (ī�޶� �ٶ󺸰�)


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
    }
}
