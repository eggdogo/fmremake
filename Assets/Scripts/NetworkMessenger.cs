using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class NetworkMessenger : MonoBehaviourPunCallbacks, IPunObservable
{

    public GameObject playerPrefab;

    GameObject myPlayer;
    FunkePlayer myPlayerScript;

    [HideInInspector]
    public Transform head;
    [HideInInspector]
    public Transform leftHand;
    [HideInInspector]
    public Transform rightHand;

    // Start is called before the first frame update
    void Start()
    {
        myPlayer = Instantiate(playerPrefab);
        myPlayerScript = myPlayer.GetComponent<FunkePlayer>();
        myPlayerScript.isLocalPlayer = photonView.IsMine;
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            myPlayerScript.head.position = head.position;
            myPlayerScript.head.rotation = head.rotation;

            myPlayerScript.leftHand.position = leftHand.position;
            myPlayerScript.leftHand.rotation = leftHand.rotation;

            myPlayerScript.rightHand.position = rightHand.position;
            myPlayerScript.rightHand.rotation = rightHand.rotation;
        }
        transform.position = myPlayerScript.head.position; //for audio
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(myPlayerScript.head.position);
            stream.SendNext(myPlayerScript.head.rotation);

            stream.SendNext(myPlayerScript.leftHand.position);
            stream.SendNext(myPlayerScript.leftHand.rotation);

            stream.SendNext(myPlayerScript.rightHand.position);
            stream.SendNext(myPlayerScript.rightHand.rotation);
        }
        else
        {
            myPlayerScript.head.position = (Vector3)stream.ReceiveNext();
            myPlayerScript.head.rotation = (Quaternion)stream.ReceiveNext();

            myPlayerScript.leftHand.position = (Vector3)stream.ReceiveNext();
            myPlayerScript.leftHand.rotation = (Quaternion)stream.ReceiveNext();

            myPlayerScript.rightHand.position = (Vector3)stream.ReceiveNext();
            myPlayerScript.rightHand.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
