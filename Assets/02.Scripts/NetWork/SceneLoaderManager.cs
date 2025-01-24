using GetyourCrown.Network;
using GetyourCrown.UI;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderManager : MonoBehaviour
{
    public static SceneLoaderManager Instance;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
        if(scene.name.Equals("MainMenuScene"))
        {
            UI_MainMenu uI_MainMenu = UI_Manager.instance.Resolve<UI_MainMenu>();
            uI_MainMenu.Hide();

            UI_Room uI_Room = UI_Manager.instance.Resolve<UI_Room>();
            uI_Room.Show();

            PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            {PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, false },
            {PlayerInGamePlayPropertyKey.IS_AUGMENT_SELECTED, false },
            {PlayerInRoomProperty.IS_READY, false }
        });
        }
    }
}
