using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ItemSpawn : MonoBehaviourPun
{
    [Tooltip("All possible spawnpoints for items")]
    [SerializeField] GameObject[] itemSpawnPoints;
    [Tooltip("All possible items that are able to spawn")]
    [SerializeField] GameObject[] items;

    [Tooltip("Amount of time it takes between item spawns in frames")]
    [SerializeField]
    float timerMax = 15;
    private float timer;

    private int randomSpawn;
    private int randomItem;
    private bool pickedUp = true;

    void Start()
    {
        foreach(GameObject item in items)
        {
            item.SetActive(false);
        }
    }

    void Update()
    {
        if (timer < timerMax && PhotonNetwork.IsMasterClient)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (pickedUp && PhotonNetwork.IsMasterClient)
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
            transform.position = itemSpawnPoints[rndSpawn].transform.position;
            items[rndItem].transform.position = itemSpawnPoints[rndSpawn].transform.position;
            items[rndItem].gameObject.SetActive(true);
            pickedUp = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<playerInventory>().Items.Count == 0)
        {
            items[randomItem].gameObject.SetActive(false);
            pickedUp = true;
            other.GetComponent<playerInventory>().attachItem(items[randomItem].name);
        }
    }
}
