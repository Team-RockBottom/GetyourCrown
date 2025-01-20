using System;
using System.Collections.Generic;

namespace GetyourCrown.Database
{
    [Serializable]
    public struct CharaterSlotData
    {
        public int charaterId;
    }
    public class CharacterData
    {
        public CharacterData(int capacity)
        {
            slotDataList = new List<CharaterSlotData>(capacity);
        }

        public List<CharaterSlotData> slotDataList;
    }
}
