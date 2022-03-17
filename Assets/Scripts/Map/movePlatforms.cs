using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class movePlatforms : MonoBehaviourPun
{
    private SpriteRenderer platformSprite;
    [SerializeField] private float moveSpeed = 0.006f;

    void Start()
    {
        platformSprite = GetComponent<SpriteRenderer>();
        transform.SetParent(GameObject.FindGameObjectWithTag("platforms").transform);
    }

    void FixedUpdate()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed);
        if (transform.position.y < -2f && PhotonNetwork.IsMasterClient)
            photonView.RPC("newPosition", RpcTarget.All, gameObject.GetComponentInParent<spawnPlatforms>().setRandom() );
    }

    [PunRPC]
    public void newPosition(Vector3 randomPos)
    {
        transform.position = randomPos;
    }
}
