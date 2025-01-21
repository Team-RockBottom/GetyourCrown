using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Augment
{
    public class UI_Augment : MonoBehaviour
    {
        [SerializeField] AugmentRepository _augmentRepository;
        [SerializeField] Augmentslot[] _augmentPrefab = new Augmentslot[3];


        private void Start()
        {
            _augmentRepository.GetComponent<AugmentRepository>();
        }

        /// <summary>
        /// 슬롯 초기화
        /// </summary>
        public void AugmentSlotRefresh()
        {

            int[] beforeAugmentIds = new int[3];
            beforeAugmentIds[0] = 99;
            beforeAugmentIds[1] = 99;
            beforeAugmentIds[2] = 99;

            Debug.Log(_augmentPrefab.Length);
            for (int i = 0; i < _augmentPrefab.Length; i++)
            {
                int randomAugmentId = Random.Range(1, _augmentRepository._augmentDic.Count);

                for (int j = 0; j < beforeAugmentIds.Length; j++)
                {
                    if (beforeAugmentIds[j] == randomAugmentId)
                    {
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
                Debug.Log(i);

                _augmentPrefab[i].nameValue = _augmentRepository._augmentDic[randomAugmentId].augmentName;
                _augmentPrefab[i].descriptionValue = _augmentRepository._augmentDic[randomAugmentId].augmentDescripction;
                _augmentPrefab[i].iconimage = _augmentRepository._augmentDic[randomAugmentId].augmentIcon;
                beforeAugmentIds[i] = randomAugmentId;
            }
        }


        /// <summary>
        /// 선택한 증강 적용 버튼에 구독
        /// </summary>
        /// <returns> 증강의 ID </returns>
        //public int TransferSelectedAugmentData()
        //{
        //    int SelectedAugmentData;
        //    return a;
        //}
    }
}
