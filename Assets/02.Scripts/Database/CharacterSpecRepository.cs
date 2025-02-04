using GetyourCrown.Database;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterSpecRepository", menuName = "Scriptable Objects/CharacterSpecRepository")]
public class CharacterSpecRepository : ScriptableObject
{
    [field: SerializeField] public List<CharacterSpec> specs;
}
