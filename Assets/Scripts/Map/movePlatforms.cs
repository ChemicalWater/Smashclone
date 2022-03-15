using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class movePlatforms : MonoBehaviour
{
    private SpriteRenderer platformSprite;
    [SerializeField] private float moveSpeed = 0.001f;

    void Start()
    {
        platformSprite = GetComponent<SpriteRenderer>();
        transform.SetParent(GameObject.FindGameObjectWithTag("test").transform);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed);
        if (transform.position.y < -3f && PhotonNetwork.IsMasterClient)
            transform.position = gameObject.GetComponentInParent<spawnPlatforms>().setRandom();
    }
}
