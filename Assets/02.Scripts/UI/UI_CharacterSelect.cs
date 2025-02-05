using GetyourCrown.Database;
using GetyourCrown.Network;
using GetyourCrown.UI.UI_Utilities;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GetyourCrown.UI
{
    public class UI_CharacterSelect : UI_Popup, IPointerDownHandler, IPointerUpHandler, IDragHandler
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
        public int _selectedCharacterId;
        public int _lockedSelectCharacterId;
        private Vector2 _lastMousePosition;
        private bool _isRotating = false;
        private Quaternion _currentCharacterRotation;
        private Quaternion _startRotation;
        private bool _isFirst = true;
        private bool _isCharacterSelectFirst = true;
        string _characterSpecFolder = "CharacterSpecs";

        private const int DEFAULT_CHARACTER_LIST_SIZE = 20;

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
                _characterPreviewPos.gameObject.SetActive(false);

                Hide();
            });
            _characterPreviewPos.gameObject.SetActive(true);

            _characterSlot.gameObject.SetActive(false);
            if (onConfirmed != null)
                _close.onClick.AddListener(onConfirmed);

            LoadCharacterSpecs();
            LoadCharacterSlot();

            if (_isFirst || _isCharacterSelectFirst)
            {
                SelectedCharacterPreview(_characterSpecs[DataManager.instance.LastCharacter]);
                _isFirst = false;
            }
            else
            {
                SelectedCharacterPreview(_characterSpecs[_selectedCharacterId]);
            }
        }

        private void LoadCharacterSpecs()
        {
            _characterSpecs = Resources.LoadAll<CharacterSpec>(_characterSpecFolder);
            //LoadAll은 리소스에서 무작위로 로드하기때문에 스펙에 넣고 id순으로 정렬
            _characterSpecs = _characterSpecs.OrderBy(spec => spec.id).ToArray();
        }

        private async void LoadCharacterSlot()
        {
            await DataManager.instance.LoadPlayerDataAsync();

            if (_characterSlots.Count == _characterSpecs.Length)
                return;

            for (int i = 0; i < _characterSpecs.Length; i++)
            {
                CharacterSlot slot = Instantiate(_characterSlot, _characterSlotContent);
                slot.gameObject.SetActive(true);
                slot.CharacterIndex = _characterSpecs[i].id;
                slot.CharacterImage = _characterSpecs[i].sprite;
                slot.CharacterPrice = _characterSpecs[i].price;
                slot.isSelected = false;
                slot.CharacterLocked = DataManager.instance.CurrentPlayerData.CharactersLocked[slot.CharacterIndex];
                slot.OnCharacterSelect += CharacterSelected;
                slot.gameObject.SetActive(true);
                _characterSlots.Add(slot);
            }

            if (_characterSlots.Count > 0)
            {
                _characterSlots[DataManager.instance.LastCharacter].isSelected = true;
            }
        }

        private void CharacterSelected(CharacterSlot selectCharacter)
        {
            if (selectCharacter.CharacterLocked)
            {
                int lockedCharacterId = selectCharacter.CharacterIndex;
                _lockedSelectCharacterId = lockedCharacterId;
                return; 
            }

            foreach (var slot in _characterSlots)
            {
                slot.isSelected = false;
            }

            selectCharacter.isSelected = true;
            int selectedCharacterId = selectCharacter.CharacterIndex; // 선택된 캐릭터ID 저장

            _selectedCharacterId = selectedCharacterId;

            PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable()
            {
                { PlayerInRoomProperty.CHARACTER_ID, selectedCharacterId }  // 캐릭터아이디 키로 선택된 캐릭터ID 저장 및 동기화
            });

            CharacterSpec selectedCharacter = _characterSpecs[selectCharacter.CharacterIndex];
            SelectedCharacterPreview(selectedCharacter);
            _isCharacterSelectFirst = false;
        }

        private void SelectedCharacterPreview(CharacterSpec spec)
        {
            //다시 선택하면 캐릭터 제거
            if (_currentCharacter != null)
                Destroy(_currentCharacter);

            if (spec.prefab != null)
            {
                _currentCharacter = Instantiate(spec.prefab, _characterPreviewPos); 
                _currentCharacter.transform.localPosition = new Vector3(0, -1, 0); 
                _currentCharacter.transform.localScale = Vector3.one;
                _currentCharacter.transform.localRotation = Quaternion.Euler(0, 180, 0);


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

        public void UpdateCharacterSlot()
        {
            foreach (var slot in _characterSlots)
            {
                if (DataManager.instance.CurrentPlayerData.CharactersLocked.ContainsKey(slot.CharacterIndex))
                {
                    slot.CharacterLocked = DataManager.instance.CurrentPlayerData.CharactersLocked[slot.CharacterIndex];
                }
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(_characterPreview.rectTransform, Input.mousePosition))
            {
                _startRotation = _currentCharacter.transform.rotation;
                _lastMousePosition = Input.mousePosition;
                _isRotating = true;
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_currentCharacter != null)
                StartCoroutine(C_RotateCharaterSlerp());

            _isRotating = false;
        }

        private IEnumerator C_RotateCharaterSlerp()
        {
            float elapsedTime = 0f;
            float returnDuration = 0.1f;
            Quaternion currentRotation = _currentCharacter.transform.rotation;

            while (elapsedTime < returnDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / returnDuration;
                _currentCharacter.transform.rotation = Quaternion.Slerp(currentRotation, _startRotation, t);
                yield return null;
            }

            _currentCharacter.transform.rotation = _startRotation;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isRotating)
            {
                Vector2 delta = (Vector2)Input.mousePosition - _lastMousePosition;
                float rotationSpeed = 0.2f;

                if (_currentCharacter != null)
                    _currentCharacter.transform.Rotate(Vector3.up, -delta.x * rotationSpeed, Space.World);

                _lastMousePosition = Input.mousePosition;
            }
        }
    }
}
