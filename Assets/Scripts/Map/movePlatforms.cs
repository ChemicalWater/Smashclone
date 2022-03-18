using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace smashclone
{
    public class movePlatforms : MonoBehaviourPun
    {
        private SpriteRenderer platformSprite;
        [SerializeField]
        private float moveSpeed = 0.008f;
        private float storeSpeed;

        void Start()
        {
            platformSprite = GetComponent<SpriteRenderer>();
            transform.SetParent(GameObject.FindGameObjectWithTag("platforms").transform);
            storeSpeed = moveSpeed;
        }

        void FixedUpdate()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - moveSpeed);
            if (transform.position.y < -2f && PhotonNetwork.IsMasterClient)
                photonView.RPC("NewPosition", RpcTarget.All, gameObject.GetComponentInParent<spawnPlatforms>().setRandom());
        }


        public void ChangeSpeed(float newSpeed)
        {
            moveSpeed = newSpeed;
        }

        [PunRPC]
        public void NewPosition(Vector3 randomPos)
        {
            transform.position = randomPos;
        }
    }
}
