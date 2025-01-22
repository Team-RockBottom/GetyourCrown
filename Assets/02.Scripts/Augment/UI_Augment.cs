using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

namespace Augment
{
    public class UI_Augment : MonoBehaviour
    {
        [SerializeField] AugmentRepository _augmentRepository;
        [SerializeField] Augmentslot[] _augmentPrefab = new Augmentslot[3];
        [SerializeField] Button[] _augmentationButtons = new Button[3];
        private int selectedAugmentId = -1;

        public static event Action<int> OnAugmentSelected;
        
        
        private void Start()
        {
            _augmentRepository.GetComponent<AugmentRepository>();
            AugmentSlotRefresh();

            for (int i = 0; i < _augmentationButtons.Length; i++)
            {
                int index = i; // 각 버튼의 인덱스를 유지하기 위해 복사
                _augmentationButtons[i].onClick.AddListener(() => SelectAugment(_augmentPrefab[i].id));
            }
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

            for (int i = 0; i < _augmentPrefab.Length; i++)
            {
                int randomAugmentId = UnityEngine.Random.Range(1, _augmentRepository._augmentDic.Count);

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

                _augmentPrefab[i].nameValue = _augmentRepository._augmentDic[randomAugmentId].augmentName;
                _augmentPrefab[i].descriptionValue = _augmentRepository._augmentDic[randomAugmentId].augmentDescripction;
                _augmentPrefab[i].iconimage = _augmentRepository._augmentDic[randomAugmentId].augmentIcon;
                _augmentPrefab[i].id = _augmentRepository._augmentDic[randomAugmentId].augmentId;
                beforeAugmentIds[i] = randomAugmentId;
            }
        }
    
        void SelectAugment(int augmentIndex)
        {
            selectedAugmentId = augmentIndex;
            Debug.Log($"Selected Augment ID: {selectedAugmentId}");

            // 이벤트 발생
            OnAugmentSelected?.Invoke(selectedAugmentId);
        }
    }
}
