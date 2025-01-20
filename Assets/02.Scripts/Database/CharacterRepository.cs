using System.Collections.Generic;
using UnityEngine;

namespace GetyourCrown.Database
{
    [CreateAssetMenu(fileName = "CharacterRepository", menuName = "Scriptable Objects/CharacterRepository")]
    public class CharacterRepository : ScriptableObject
    {
        [field: SerializeField] public List<CharacterSpec> spec;

        public CharacterSpec Get(int characterId)
        {
            int index = spec.FindIndex(spec => spec.id == characterId);

            if (index < 0)
            {
                throw new System.Exception($"[{nameof(CharacterRepository)}] : Failed to get Character Spec. Wrong id {characterId}.");
            }
            else
            {
                return spec[index];
            }
        }
    }
}
