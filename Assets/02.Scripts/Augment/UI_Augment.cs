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
        [SerializeField] Button[] _augmentationButtons;
        private int selectedAugmentId = -1;

        public static event Action<int> OnAugmentSelected; //���� �̺�Ʈ

        private void Awake()
        {
            _augmentRepository.GetComponent<AugmentRepository>();

            //for (int i = 0; i < _augmentationButtons.Length; i++)
            //{
            //    //��ư ���
            //    _augmentationButtons[i].onClick.AddListener(() => SelectAugment(_augmentPrefab[i].id));
            //}
        }


        /// <summary>
        /// ���� �ʱ�ȭ
        /// </summary>
        public void AugmentSlotRefresh()
        {

            int[] beforeAugmentIds = new int[3];
            beforeAugmentIds[0] = 99;
            beforeAugmentIds[1] = 99;
            beforeAugmentIds[2] = 99;

            for (int i = 0; i < _augmentPrefab.Length; i++)
            {
                int randomAugmentId = UnityEngine.Random.Range(1, _augmentRepository._augmentDic.Count + 1);
                Debug.Log(_augmentRepository._augmentDic.Count);
                Debug.Log(randomAugmentId);
                for (int j = 0; j < beforeAugmentIds.Length; j++)
                {
                    if (beforeAugmentIds[j] == randomAugmentId)
                    {
                        return;
                    }
                }

                _augmentPrefab[i].nameValue = _augmentRepository._augmentDic[randomAugmentId].augmentName;
                _augmentPrefab[i].descriptionValue = _augmentRepository._augmentDic[randomAugmentId].augmentDescripction;
                _augmentPrefab[i].iconimage = _augmentRepository._augmentDic[randomAugmentId].augmentIcon;
                _augmentPrefab[i].id = _augmentRepository._augmentDic[randomAugmentId].augmentId;
                _augmentationButtons[i].onClick.AddListener(() => SelectAugment(randomAugmentId));
                beforeAugmentIds[i] = randomAugmentId;
            }

        }
    
        void SelectAugment(int augmentIndex)
        {
            selectedAugmentId = augmentIndex;
            OnAugmentSelected?.Invoke(selectedAugmentId);
            Debug.Log($"Selected Augment ID: {selectedAugmentId}");
            gameObject.SetActive(false);
            // �̺�Ʈ �߻�
        }
    }
}
