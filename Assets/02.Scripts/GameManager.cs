using Unity.Services.Authentication;
using UnityEngine;

public class GamaManager : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void OnApplicationQuit()
    {
        AuthenticationService.Instance.SignOut();
    }
}
