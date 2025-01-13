using GetYourCrown.UI.Utility;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GetYourCrown.UI
{
    [RequireComponent(typeof(Canvas))]
    public abstract class UI_Base : ComponentResolvingBehaviour
    {
        public int SortingOrder
        {
            get => _canvas.sortingOrder;
            set => _canvas.sortingOrder = value;
        }


        protected UI_Manager _manager;
        Canvas _canvas;
        GraphicRaycaster _graphicRaycaster;
        EventSystem _eventSystem;
        PointerEventData _pointerEventData;
        List<RaycastResult> _raycastResultBuffer;


        protected override void Awake()
        {
            base.Awake();

            _canvas = GetComponent<Canvas>();
            _graphicRaycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = EventSystem.current;
            _pointerEventData = new PointerEventData(_eventSystem);
            _raycastResultBuffer = new List<RaycastResult>(1);
            _manager = UI_Manager.instance;
        }

        protected virtual void Start() { }

        public virtual void Show()
        {
            _canvas.enabled = true;
        }

        public virtual void Hide()
        {
            _canvas.enabled = false;
        }

        public bool TryGraphicRaycast<T>(Vector2 pointerPosition, out T result)
            where T : Component
        {
            _pointerEventData.position = pointerPosition;
            _raycastResultBuffer.Clear();
            _graphicRaycaster.Raycast(_pointerEventData, _raycastResultBuffer);

            if (_raycastResultBuffer.Count > 0)
            {
                if (_raycastResultBuffer[0].gameObject.TryGetComponent(out result))
                    return true;
            }

            result = default;
            return false;
        }
    }
}
