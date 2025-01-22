using GetyourCrown.UI.UI_Utilities;
using UnityEngine.UI;

namespace GetyourCrown.UI
{
    public class UI_CreateAccount : UI_Popup
    {
        [Resolve] InputField _id;
        [Resolve] InputField _password;
        [Resolve] Button _create;
        [Resolve] Button _cancle;

        protected override void Start()
        {
            base.Start();
        }

        public void Show()
        {
            base.Show();

            _create.onClick.AddListener(() => 
            {
                if (string.IsNullOrWhiteSpace(_id.text))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("�г����� �Է����ּ���.");
                    return;
                }
                else if (string.IsNullOrWhiteSpace(_password.text))
                {
                    UI_ConfirmWindow uI_ConfirmWindow = UI_Manager.instance.Resolve<UI_ConfirmWindow>();
                    uI_ConfirmWindow.Show("��й�ȣ�� �Է����ּ���.");
                    return;
                }
                /*else if ()
                {
                    // todo => ������ �̹� �ִٸ� ���� ������
                }*/

                Hide();
            });

            _cancle.onClick.AddListener(Hide);
        }
    }
}
