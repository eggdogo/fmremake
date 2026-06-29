using UnityEngine;
using Photon.Pun;

public class FunkePlayer : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;

    [HideInInspector] public Transform physicalHead;
    [HideInInspector] public Transform physicalLeftHand;
    [HideInInspector] public Transform physicalRightHand;

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

    void LateUpdate()
    {
        if (photonView.IsMine)
        {
            head.position = physicalHead.position;
            head.rotation = physicalHead.rotation;

            leftHand.position = physicalLeftHand.position;
            leftHand.rotation = physicalLeftHand.rotation;

            rightHand.position = physicalRightHand.position;
            rightHand.rotation = physicalRightHand.rotation;
        }
        else if (gotFirstNetworkUpdate)
        {
            // Remote players smoothly move toward latest network target.
            float posT = 1f - Mathf.Exp(-positionSmoothSpeed * Time.deltaTime);
            float rotT = 1f - Mathf.Exp(-rotationSmoothSpeed * Time.deltaTime);

            head.position =
                Vector3.Lerp(head.position, targetHeadPos, posT);
            head.rotation =
                Quaternion.Slerp(head.rotation, targetHeadRot, rotT);

            leftHand.position =
                Vector3.Lerp(leftHand.position, targetLeftHandPos, posT);
            leftHand.rotation =
                Quaternion.Slerp(leftHand.rotation, targetLeftHandRot, rotT);

            rightHand.position =
                Vector3.Lerp(rightHand.position, targetRightHandPos, posT);
            rightHand.rotation =
                Quaternion.Slerp(rightHand.rotation, targetRightHandRot, rotT);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(physicalHead.position);
            stream.SendNext(physicalHead.rotation);

            stream.SendNext(physicalLeftHand.position);
            stream.SendNext(physicalLeftHand.rotation);

            stream.SendNext(physicalRightHand.position);
            stream.SendNext(physicalRightHand.rotation);
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

                head.position = targetHeadPos;
                head.rotation = targetHeadRot;

                leftHand.position = targetLeftHandPos;
                leftHand.rotation = targetLeftHandRot;

                rightHand.position = targetRightHandPos;
                rightHand.rotation = targetRightHandRot;
            }
        }
    }
}