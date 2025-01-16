using UnityEngine;

namespace GetyourCrown.Database
{
    [CreateAssetMenu(fileName = "CharacterSpec", menuName = "Scriptable Objects/CharacterSpec")]
    public class CharacterSpec : ScriptableObject
    {
        [field: SerializeField] public int id { get; private set; }
        [field: SerializeField] public string name { get; private set; }
        [field: SerializeField] public int price { get; private set; }
        [field: SerializeField] public Sprite image { get; private set; }
        [field: SerializeField] public GameObject prefab { get; private set; }
    }
}

