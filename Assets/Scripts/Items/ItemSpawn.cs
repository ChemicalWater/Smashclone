using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class ItemSpawn : MonoBehaviour
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
        if (timer < timerMax)
        {
            timer += Time.deltaTime;
        }
        else
        {
            MoveItemToSpawnpoint();
            timer = 0;
        }
    }

    void MoveItemToSpawnpoint()
    {
        if (pickedUp)
        {
            randomSpawn = Random.Range(0, itemSpawnPoints.Length);
            randomItem = Random.Range(0, items.Length);

            transform.position = itemSpawnPoints[randomSpawn].transform.position;
            items[randomItem].transform.position = itemSpawnPoints[randomSpawn].transform.position;
            items[randomItem].gameObject.SetActive(true);
            pickedUp = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            items[randomItem].gameObject.SetActive(false);
            pickedUp = true;
            other.GetComponent<playerInventory>().attachItem(items[randomItem].name);
        }
    }
}
