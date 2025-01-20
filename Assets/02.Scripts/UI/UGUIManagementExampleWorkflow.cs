using Practices.UGUI_Management.UI;
using System.Collections;
using UnityEngine;

namespace Practices.UGUI_Management.GameElements.Workflows
{
    public class UGUIManagementExampleWorkflow : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(Workflow());
        }

        IEnumerator Workflow()
        {
            UI_UGUIManagementExampleScreen uguiManagementExampleScreen = UI_Manager.instance.Resolve<UI_UGUIManagementExampleScreen>();
            uguiManagementExampleScreen.Show();

            UI_Equipments equipments = UI_Manager.instance.Resolve<UI_Equipments>();
            equipments.Show();

            UI_Inventory inventory = UI_Manager.instance.Resolve<UI_Inventory>();
            inventory.Show();
            yield break;
        }
    }
}