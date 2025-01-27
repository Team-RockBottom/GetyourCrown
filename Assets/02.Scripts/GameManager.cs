using GetyourCrown.Database;
using GetyourCrown.UI;
using Unity.Services.Authentication;
using UnityEngine;

public class GamaManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private async void OnApplicationQuit()
    {
        UI_CharacterSelect _uiCharacterSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
        int lastCharacterId = _uiCharacterSelect._selectedCharacterId;
        await DataManager.instance.SaveLastCharacterAsync(lastCharacterId);
        AuthenticationService.Instance.SignOut();
    }
}
