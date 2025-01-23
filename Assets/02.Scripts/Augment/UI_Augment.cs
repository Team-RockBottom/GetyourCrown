using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;
using GetyourCrown.UI;

namespace Augment
{
    public class UI_Augment : MonoBehaviour
    {
        [SerializeField] AugmentRepository _augmentRepository;
        [SerializeField] Augmentslot[] _augmentPrefab = new Augmentslot[3];
        [SerializeField] Button[] _augmentationButtons;
        private int selectedAugmentId = -1;

        public static event Action<int> OnAugmentSelected; //선택 이벤트

        private void Awake()
        {
            _augmentRepository.GetComponent<AugmentRepository>();

            //for (int i = 0; i < _augmentationButtons.Length; i++)
            //{
            //    //버튼 등록
            //    _augmentationButtons[i].onClick.AddListener(() => SelectAugment(_augmentPrefab[i].id));
            //}
        }



        /// <summary>
        /// 슬롯 초기화
        /// </summary>
        public void AugmentSlotRefresh()
        {
            Debug.Log("AugmentSlotRefresh Call");
            int[] beforeAugmentIds = new int[3];
            beforeAugmentIds[0] = 99;
            beforeAugmentIds[1] = 99;
            beforeAugmentIds[2] = 99;

            for (int i = 0; i < _augmentPrefab.Length; i++)
            {
                int index = i;
                bool duplicate = false;
                int randomAugmentId = UnityEngine.Random.Range(1, _augmentRepository._augmentDic.Count + 1);
                Debug.Log($"Random Augment Count : {randomAugmentId}");

                for (int j = 0; j < beforeAugmentIds.Length; j++)
                {
                    if (beforeAugmentIds[j] == randomAugmentId)
                    {
                        i--;
                        duplicate = true;
                        break;
                    }
                }

                if (duplicate)
                {
                    continue;
                }

                _augmentationButtons[index].onClick.AddListener(() => SelectAugment(randomAugmentId));
                _augmentPrefab[i].nameValue = _augmentRepository._augmentDic[randomAugmentId].augmentName;
                _augmentPrefab[i].descriptionValue = _augmentRepository._augmentDic[randomAugmentId].augmentDescripction;
                _augmentPrefab[i].iconimage = _augmentRepository._augmentDic[randomAugmentId].augmentIcon;
                _augmentPrefab[i].id = _augmentRepository._augmentDic[randomAugmentId].augmentId;
                beforeAugmentIds[i] = randomAugmentId;
            }

        }
    
        public void SelectAugment(int augmentIndex)
        {
            selectedAugmentId = augmentIndex;
            OnAugmentSelected?.Invoke(selectedAugmentId);
            Debug.Log($"Selected Augment ID: {selectedAugmentId}");
            gameObject.SetActive(false);
            // 이벤트 발생
        }
    }
}
