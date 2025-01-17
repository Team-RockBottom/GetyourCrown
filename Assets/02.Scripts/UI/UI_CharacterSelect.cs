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
            //캐릭터 슬롯이 이미 생성 되있다면 리턴
            //이유: resolve를 하고 있고 Destroy를 하게 되면 두번째 캐릭터 변경을 누르면 오류가 발생
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
            //다시 선택하면 캐릭터 제거
            if (_currentCharacter != null)
                Destroy(_currentCharacter);

            if (spec.prefab != null)
            {
                _currentCharacter = Instantiate(spec.prefab, _characterPreviewPos); //캐릭터 생성
                _currentCharacter.transform.localPosition = new Vector3(0, -1, 0);
                _currentCharacter.transform.localScale = Vector3.one; //캐릭터 크기 설정
                _currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0); //캐릭터 회전 (카메라에 맞게 설정)


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

        void CharacterDrag(InputAction.CallbackContext context)
        {
            
        }
    }
}
