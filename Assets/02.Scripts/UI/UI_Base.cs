using GetyourCrown.UI.UI_Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UI_Base : ComponentResolvingBehaviour
    {
        public int sortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }


        public bool inputActionsEnabled
        {
            get => playerInputActions.asset.enabled;
            set
            {
                if (value)
                    playerInputActions.Enable();
                else
                    playerInputActions.Disable();
            }
        }

        protected UI_Manager manager;
        protected PlayerInputActions playerInputActions;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        EventSystem _eventSystem;
        PointerEventData _pointerEventData;
        List<RaycastResult> _raycastResultsBuffer;

        public event Action onShow;
        public event Action onHide;

        protected override void Awake()
        {
            base.Awake();

            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current; //eventsystem.current는 null을 반환할수 있어 null 체크 추가 생각해보기
            _pointerEventData = new PointerEventData(_eventSystem);
            _raycastResultsBuffer = new List<RaycastResult>(1);
            playerInputActions = new PlayerInputActions();
            manager = UI_Manager.instance;
            manager.Register(this);
        }

        protected virtual void Start() { }

        public virtual void Show()
        {
            _canvas.enabled = true;
            onShow?.Invoke();
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
            onHide?.Invoke();
        }

        public bool TryGraphicRaycast<T>(Vector2 pointerPos, out T result)
            where T : Component
        {
            _pointerEventData.position = pointerPos;
            _raycastResultsBuffer.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _raycastResultsBuffer);

            if (_raycastResultsBuffer.Count > 0)
            {
                if (_raycastResultsBuffer[0].gameObject.TryGetComponent(out result))
                    return true;
            }

            result = default;
            return false;
        }
    }
}

