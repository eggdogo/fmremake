using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using FunkePlayer.Player;

namespace FunkePlayer {
    public class FunkeManager : MonoBehaviourPunCallbacks
    {
        FunkeSettings fs;
        void Start()
        {
            fs = this.gameObject.GetComponentInChildren<FunkeSettings>();
            if (fs == null)
            {
                Debug.LogError("No FunkeSettings script in a child. Add one.");
                return;
            }
            if (fs.playerPrefab != null)
            {
                GameObject prefabCheck = Resources.Load<GameObject>(fs.playerPrefab.name);
                if (prefabCheck == null)
                {
                    Debug.LogError($"You have a custom prefab set but we can't find it in Resources! Make sure it is there, or if " +
                    $"you are using the built in prefab, leave this field blank!");
                    return;
                }
            }

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
            SpawnPlayer();
        }

        public void SpawnPlayer()
        {
            GameObject playerObject = PhotonNetwork.Instantiate("FunkePlayer/Player", Vector3.zero, Quaternion.identity);
            FunkePlayerSetup playerSetup = playerObject.GetComponent<FunkePlayerSetup>();
            playerSetup.enabled = true;

            playerSetup.headOrigin = fs.vrHeadTransform;
            playerSetup.lOrigin = fs.vrLeftHandTransform;
            playerSetup.rOrigin = fs.vrRightHandTransform;
        }
    }
}
