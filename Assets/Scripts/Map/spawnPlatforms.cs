using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class spawnPlatforms : MonoBehaviourPun
{

    [SerializeField] private GameObject bigPlatform;
    [SerializeField] private GameObject smallPlatform;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private float totalPlatforms = 12f;
    [SerializeField] private float timerMax = 2f;
    private ArrayList spawnedPlatforms;
    private float timer;
    private float randomX;
    private float randomY;
    private int lastRandom;
    private int rndSpawn;

    void Start()
    {
        spawnedPlatforms = new ArrayList();
    }

    [PunRPC]
    void SpawnPlatforms(Vector3 platformPos)
    {
        Debug.Log("Total platforms spawned: " + spawnedPlatforms.Count);
        spawnedPlatforms.Add(PhotonNetwork.Instantiate(smallPlatform.name, platformPos, transform.rotation));
    }
    
    [PunRPC]
    public Vector3 setRandom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            rndSpawn = Random.Range(0, spawnPoints.Length);
            Vector3 randomPos;

            if (rndSpawn != lastRandom)
            {
                lastRandom = rndSpawn;
                randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            } else
            {
                if (rndSpawn != spawnPoints.Length)
                    rndSpawn += 1;
                else
                    rndSpawn = Random.Range(0, spawnPoints.Length);

                randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            }

            return randomPos;
        }
        return new Vector3(50,0,0);
    }


    void Update()
    {
        if (spawnedPlatforms.Count < totalPlatforms && timer < timerMax && PhotonNetwork.IsMasterClient)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (spawnedPlatforms.Count < totalPlatforms && PhotonNetwork.IsMasterClient)
            {
                photonView.RPC("SpawnPlatforms", RpcTarget.All, setRandom());
                timer = 0;
            }
        }
    }
}
