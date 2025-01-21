using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Augment
{
    public class UI_Augment : MonoBehaviour
    {
        [SerializeField] AugmentRepository _augmentRepository;
        [SerializeField] Augmentslot[] _augmentPrefab;

        private void Awake()
        {
            AugmentSlotRefresh();
        }

        /// <summary>
        /// ���� �ʱ�ȭ
        /// </summary>
        public void AugmentSlotRefresh()
        {
            int[] beforeAugmentIds = new int[2];

            for (int i = 0; i < _augmentPrefab.Length; i++)
            {
                int randomAugmentId = Random.Range(0, _augmentRepository._augmentDic.Count - 1);

                for (int j = 0; j < beforeAugmentIds.Length; j++)
                {
                    if (beforeAugmentIds[j] == randomAugmentId)
                    {
                        return;
                    }
                }

                _augmentPrefab[i].nameValue = _augmentRepository._augmentDic[randomAugmentId].name;
                _augmentPrefab[i].descriptionValue = _augmentRepository._augmentDic[randomAugmentId].augmentDescripction;
                _augmentPrefab[i].iconimage = _augmentRepository._augmentDic[randomAugmentId].augmentIcon;
                beforeAugmentIds[i] = randomAugmentId;
            }
        }


        /// <summary>
        /// ������ ���� ���� ��ư�� ����
        /// </summary>
        /// <returns> ������ ID </returns>
        //public int TransferSelectedAugmentData()
        //{
        //    int SelectedAugmentData;
        //    return a;
        //}
    }
}
