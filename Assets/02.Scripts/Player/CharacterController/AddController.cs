using Photon.Pun;
using Practices.PhotonPunClient;
using Practices.PhotonPunClient.Network;
using UnityEngine;

public class AddController : MonoBehaviour, IPunInstantiateMagicCallback
{

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
            {
                { PlayerInGamePlayPropertyKey.IS_CHARACTER_SPAWNED, true }
            });

        Debug.Log("Instantiated");
        ExampleCharacterController exampleCharacterController = GetComponentInChildren<ExampleCharacterController>();
        exampleCharacterController.AddController();

    }
}
