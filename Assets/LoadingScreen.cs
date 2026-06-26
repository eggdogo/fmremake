using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class LoadingScreen : MonoBehaviourPunCallbacks
{
    public Transform cam;
    public Transform textPivot;

    void Update(){
        textPivot.position = cam.position;
        if (Mathf.Abs(textPivot.rotation.eulerAngles.y - cam.rotation.eulerAngles.y) > 6f){
            Quaternion targetRotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);

            textPivot.rotation = Quaternion.Slerp(
                textPivot.rotation,
                targetRotation,
                Time.deltaTime
            );
        }
    }

    public override void OnJoinedRoom()
    {
        Destroy(gameObject); //kill yourself
    }

}
