using GetyourCrown.Database;
using GetyourCrown.UI;
using Unity.Services.Authentication;
using UnityEngine;

public class GamaManager : MonoBehaviour
{
    private const string IS_LOGIIN = "IsLogIn";

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private async void OnApplicationQuit()
    {
        UI_CharacterSelect _uiCharacterSelect = UI_Manager.instance.Resolve<UI_CharacterSelect>();
        int lastCharacterId = _uiCharacterSelect._selectedCharacterId;
        await DataManager.instance.SaveLastCharacterAsync(lastCharacterId);
        PlayerPrefs.SetInt(IS_LOGIIN, 0);
        PlayerPrefs.Save();
        AuthenticationService.Instance.SignOut();
    }
}
