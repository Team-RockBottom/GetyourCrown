using GetyourCrown.UI.UI_Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class CharacterSlot
    {
        public int index { get; set; }


        int _characterId;
        string _characterName;
        [Resolve] Image _characterImage;
        [Resolve] GameObject _charaterPrefab;
    }
}
