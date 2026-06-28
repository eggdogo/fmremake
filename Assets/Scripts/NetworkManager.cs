using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public string prefabPath = "NetworkMessenger";

    NetworkMessenger networkMessenger;

    public Transform head;
    public Transform leftHand;
    public Transform rightHand;


    void Start()
    {
        connectToServers();
    }

    void connectToServers()
    {
        Debug.Log("Connecting to servers...");
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "us";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected! Joining lobby...");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("Joined lobby! Joining room...");
        PhotonNetwork.JoinRandomOrCreateRoom(null, 0, MatchmakingMode.FillRoom, new TypedLobby(SceneManager.GetActiveScene().name, LobbyType.Default));
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("Joined room!");
        GameObject networkMessengerObj = PhotonNetwork.Instantiate(prefabPath, Vector3.zero, Quaternion.identity);
        networkMessenger = networkMessengerObj.GetComponent<NetworkMessenger>();
        networkMessenger.head = head;
        networkMessenger.leftHand = leftHand;
        networkMessenger.rightHand = rightHand;
    }
}
