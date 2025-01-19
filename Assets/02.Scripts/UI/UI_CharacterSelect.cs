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

            int selectedCharacterId = selectCharacter.CharacterIndex; // 선택된 캐릭터ID 저장
            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { PlayerInRoomProperty.CHARACTER_ID, selectedCharacterId }  // 캐릭터아이디 키로 선택된 캐릭터ID 저장 및 동기화
            });

            CharacterSpec selectedCharacter = _characterSpecs[selectCharacter.CharacterIndex];
            SelectedCharacterPreview(selectedCharacter);
        }

        private void SelectedCharacterPreview(CharacterSpec spec)
        {
            //다시 선택하면 캐릭터 제거
            if (_currentCharacter != null)
                Destroy(_currentCharacter);

            if (spec.prefab != null)
            {
                _currentCharacter = Instantiate(spec.prefab, _characterPreviewPos); //캐릭터 생성
                _currentCharacter.transform.localPosition = new Vector3(0, -1, 0);
                _currentCharacter.transform.localScale = Vector3.one; //캐릭터 크기 설정
                _currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0); //캐릭터 회전 (카메라에 바라보겐)


                // 캐릭터를 렌더링할 카메라 설정
                if (_characterCamera == null)
                {
                    // RawImage 렌더 해줄 카메라 생성
                    _characterCamera = new GameObject("CharacterCamera").AddComponent<Camera>();
                    // 카메라의 기본 오디오 리스너가 같이 생성되어 메인카메라와 겹쳐서 캐릭터카메라의 오디오 리스너 제거
                    AudioListener audioListener = _characterCamera.GetComponent<AudioListener>();

                    if (audioListener != null)
                    {
                        Destroy(audioListener);  // 오디오 리스너 제거
                    }

                    _renderTexture = new RenderTexture(512, 512, 32); //렌더 해상도 설정
                    _characterCamera.targetTexture = _renderTexture;  // 카메라의 렌더 타겟을 RenderTexture로 설정
                    _characterPreview.texture = _renderTexture;  // RawImage에 렌더할 텍스처 설정
                }

                _characterCamera.clearFlags = CameraClearFlags.SolidColor;  // Skybox를 제외하고 렌더

                // 카메라 렌더링을 실행하여 캐릭터를 RawImage에 표시
                _characterCamera.Render();
            }
        }
    }
}
