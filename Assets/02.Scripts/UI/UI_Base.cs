using GetyourCrown.UI.UI_Utilities;
using UnityEngine;

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

        Canvas _canvas;
    }
}

