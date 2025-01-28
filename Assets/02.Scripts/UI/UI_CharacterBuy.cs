using GetyourCrown.Database;
using GetyourCrown.UI.UI_Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_CharacterBuy : UI_Popup
    {
        [Resolve] Image _characterImage;
        [Resolve] TMP_Text _price;
        [Resolve] Button _confirm;
        [Resolve] Button _cancle;


        protected override void Start()
        {
            base.Start();

            _confirm.onClick.AddListener(async () =>
            {
                int price = int.Parse(_price.text);

                if (DataManager.instance.Coins >= price)
                {
                    UI_CharacterSelect _uiCharacterSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
                    int characterId = _uiCharacterSelect._lockedSelectCharacterId;
                    bool characterBuySucces = await DataManager.instance.UnLockCharacterAsync(characterId);

                    if (characterBuySucces)
                    {
                        await DataManager.instance.UpdatePlayerCoinsAsync(-price);
                        UI_ConfirmWindow _uiConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                        _uiConfirmWindow.Show("캐릭터가 구매되었습니다.");
                        _uiCharacterSelect.UpdateCharacterSlot();
                        Hide();
                    }
                }
                else
                {
                    UI_ConfirmWindow _uiConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    _uiConfirmWindow.Show("코인이 부족합니다.");
                }
            });

            _cancle.onClick.AddListener(() =>
            {
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();
        }

        public void CharacterInfo(Sprite sprite) //, int price)
        {
            _characterImage.sprite = sprite;
            //_price.text = price.ToString();
        }
    }
}
