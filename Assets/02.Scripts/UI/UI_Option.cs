using GetYourCrown.UI.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace GetYourCrown.UI
{
    public class UI_Option : UI_Screan
    {
        [Resolve] Toggle _fullScrean;
        [Resolve] Toggle _vSync;
        [Resolve] Button _confirm;
        [Resolve] Button _cancel;
    }
}
