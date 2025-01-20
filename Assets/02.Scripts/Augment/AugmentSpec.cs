using UnityEngine;
using UnityEngine.UI;

namespace Augment
{
    [CreateAssetMenu(fileName = "AugmentSpec", menuName = "Scriptable Objects/AugmentSpec")]
    public class AugmentSpec : ScriptableObject
    {
        [field: SerializeField] public int augmentId { get; set; }
        [field: SerializeField] public string augmentName { get; set; }
        [field: SerializeField] public string augmentDescripction { get; set; }
        [field: SerializeField] public Sprite augmentIcon { get; set; }
        [field: SerializeField] public float multiple { get; set; }
        [field: SerializeField] public float speedIncrease { get; set; }
        [field: SerializeField] public float increaseCoolDown { get; set; }
        [field: SerializeField] public float maxSpeedIncrease { get; set; }
        [field: SerializeField] public float scoreIncrease { get; set; }
    }
}