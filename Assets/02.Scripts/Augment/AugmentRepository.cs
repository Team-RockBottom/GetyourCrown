using System.Collections.Generic;
using UnityEngine;

namespace Augment
{
    public class AugmentRepository : MonoBehaviour
    {
        [SerializeField] List<AugmentSpec> _augmentSpecs = new List<AugmentSpec>();
        public IDictionary<int, AugmentSpec> IaugmentDic => _augmentDic;
        public Dictionary<int, AugmentSpec> _augmentDic = new Dictionary<int, AugmentSpec>();

        private void Awake()
        {
            foreach (AugmentSpec spec in _augmentSpecs)
            {
                _augmentDic.Add(spec.augmentId, spec);
            }
        }

        public AugmentSpec Get(int id)
        {
            return _augmentDic[id];
        }


    }
}
