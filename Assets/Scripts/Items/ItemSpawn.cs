using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace smashclone
{
    public class ItemSpawn : MonoBehaviourPun
    {
        [Tooltip("All possible spawnpoints for items")]
        [SerializeField]
        GameObject[] itemSpawnPoints;
        [Tooltip("All possible items that are able to spawn")]
        [SerializeField]
        GameObject[] items;
        [Tooltip("Amount of time it takes between item spawns in frames")]
        [SerializeField]
        float timerMax = 30f;
        private float timer;

        private int randomSpawn;
        private int randomItem;
        private bool spawned = false;

        void Start()
        {
            foreach (GameObject item in items)
            {
                item.SetActive(false);
            }
            items[0].transform.parent.GetComponent<Collider2D>().enabled = false;
        }

        void Update()
        {
            if (timer < timerMax && PhotonNetwork.IsMasterClient && !spawned && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (!spawned && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
                {
                    SetRandom();
                    photonView.RPC("MoveItemToSpawnpoint", RpcTarget.All, randomSpawn, randomItem);
                    timer = 0;
                }
            }
        }

        void SetRandom()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                randomSpawn = Random.Range(0, itemSpawnPoints.Length);
                randomItem = Random.Range(0, items.Length);
            }
        }

        [PunRPC]
        void MoveItemToSpawnpoint(int rndSpawn, int rndItem)
        {
            randomItem = rndItem;
            randomSpawn = rndSpawn;
            transform.position = itemSpawnPoints[randomSpawn].transform.position;
            items[randomItem].transform.position = itemSpawnPoints[randomSpawn].transform.position;
            items[randomItem].transform.parent.GetComponent<Collider2D>().enabled = true;
            items[randomItem].gameObject.SetActive(true);
            spawned = true;
        }

        [PunRPC]
        void PickUp()
        {
            items[randomItem].transform.parent.GetComponent<Collider2D>().enabled = false;
            items[randomItem].gameObject.SetActive(false);
            spawned = false;
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player" && other.GetComponent<playerInventory>() != null)
            {
                if (spawned)
                {
                    other.GetComponent<playerInventory>().attachItem(items[randomItem].name);
                    photonView.RPC("PickUp", RpcTarget.All);
                }
            }
        }
    }
}
