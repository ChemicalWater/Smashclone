using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class removePlatform : MonoBehaviourPun
{
    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.SetActive(true);
    }

    [PunRPC]
    void blipPlatform()
    {
        if (PhotonNetwork.IsMasterClient)
            this.gameObject.SetActive(false);
    }
}
