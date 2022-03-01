using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawn : MonoBehaviour
{
    [Tooltip("All possible spawnpoints for items")]
    [SerializeField] GameObject[] itemSpawnPoints;
    [Tooltip("All possible items that are able to spawn")]
    [SerializeField] GameObject[] items;

    private int randomSpawn;
    private int randomItem;

    void Start()
    {
        //  foreach (GameObject item in items)
        // {
        //    item.SetActive(false);
        //}
        randomSpawn = Random.Range(0, itemSpawnPoints.Length);
        randomItem = Random.Range(0, items.Length);

        MoveItemToSpawnpoint();
    }

    void MoveItemToSpawnpoint()
    {
        this.transform.position = itemSpawnPoints[randomSpawn].transform.position;
        items[randomItem].transform.position = itemSpawnPoints[randomSpawn].transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(items[randomItem].name);
        if (other.tag == "Player")
        {
            this.gameObject.SetActive(false);
        }
    }
}
