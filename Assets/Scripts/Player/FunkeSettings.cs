using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FunkePlayer {
    public class FunkeSettings : MonoBehaviour
    {
        [Header("In-Scene Refrences")]
        public Transform vrHeadTransform;
        public Transform vrRightHandTransform;
        public Transform vrLeftHandTransform;
        [Space]
        [Header("Prefab Refrences")]
        [Tooltip("Use this to define a custom player prefab. If not selected, will default to the Player prefab in Resources/FunkePlayer (Custom prefab MUST be in Resources)")] public GameObject playerPrefab;
        [Space]
        [Header("Server Settings")]
        public int maxPlayersPerLobby;
    }
}
