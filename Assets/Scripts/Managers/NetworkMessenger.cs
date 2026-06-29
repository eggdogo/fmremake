using UnityEngine;
using Photon.Pun;

public class NetworkMessenger : MonoBehaviourPunCallbacks, IPunObservable
{
    public GameObject playerPrefab;

    GameObject myPlayer;
    FunkePlayer myPlayerScript;

    [HideInInspector] public Transform head;
    [HideInInspector] public Transform leftHand;
    [HideInInspector] public Transform rightHand;

    [Header("Remote Smoothing")] //i dont understand this fully this part is chatgpt
    public float positionSmoothSpeed = 20f;
    public float rotationSmoothSpeed = 20f;

    Vector3 targetHeadPos;
    Quaternion targetHeadRot;

    Vector3 targetLeftHandPos;
    Quaternion targetLeftHandRot;

    Vector3 targetRightHandPos;
    Quaternion targetRightHandRot;

    bool gotFirstNetworkUpdate;

    void Start()
    {
        myPlayer = Instantiate(playerPrefab);
        myPlayerScript = myPlayer.GetComponent<FunkePlayer>();
        myPlayerScript.isLocalPlayer = photonView.IsMine;
    }

    void LateUpdate()
    {
        if (myPlayerScript == null)
            return;

        if (photonView.IsMine)
        {
            myPlayerScript.head.position = head.position;
            myPlayerScript.head.rotation = head.rotation;

            myPlayerScript.leftHand.position = leftHand.position;
            myPlayerScript.leftHand.rotation = leftHand.rotation;

            myPlayerScript.rightHand.position = rightHand.position;
            myPlayerScript.rightHand.rotation = rightHand.rotation;
        }
        else if (gotFirstNetworkUpdate)
        {
            // Remote players smoothly move toward latest network target.
            float posT = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
            float rotT = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

            myPlayerScript.head.position =
                Vector3.Lerp(myPlayerScript.head.position, targetHeadPos, posT);
            myPlayerScript.head.rotation =
                Quaternion.Slerp(myPlayerScript.head.rotation, targetHeadRot, rotT);

            myPlayerScript.leftHand.position =
                Vector3.Lerp(myPlayerScript.leftHand.position, targetLeftHandPos, posT);
            myPlayerScript.leftHand.rotation =
                Quaternion.Slerp(myPlayerScript.leftHand.rotation, targetLeftHandRot, rotT);

            myPlayerScript.rightHand.position =
                Vector3.Lerp(myPlayerScript.rightHand.position, targetRightHandPos, posT);
            myPlayerScript.rightHand.rotation =
                Quaternion.Slerp(myPlayerScript.rightHand.rotation, targetRightHandRot, rotT);
        }

        // For audio.
        transform.position = myPlayerScript.head.position;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (myPlayerScript == null)
            return;

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
            targetHeadPos = (Vector3)stream.ReceiveNext();
            targetHeadRot = (Quaternion)stream.ReceiveNext();

            targetLeftHandPos = (Vector3)stream.ReceiveNext();
            targetLeftHandRot = (Quaternion)stream.ReceiveNext();

            targetRightHandPos = (Vector3)stream.ReceiveNext();
            targetRightHandRot = (Quaternion)stream.ReceiveNext();

            if (!gotFirstNetworkUpdate)
            {
                gotFirstNetworkUpdate = true;

                myPlayerScript.head.position = targetHeadPos;
                myPlayerScript.head.rotation = targetHeadRot;

                myPlayerScript.leftHand.position = targetLeftHandPos;
                myPlayerScript.leftHand.rotation = targetLeftHandRot;

                myPlayerScript.rightHand.position = targetRightHandPos;
                myPlayerScript.rightHand.rotation = targetRightHandRot;
            }
        }
    }

    void OnDestroy()
    {
        if (myPlayer == null)
            return;

        if (!photonView.IsMine)
        {
            DisconnectEffect effect = myPlayer.AddComponent<DisconnectEffect>();
            effect.StartLaunch();
        }
        else
        {
            Destroy(myPlayer);
        }
    }
}