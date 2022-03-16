using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class spawnPlatforms : MonoBehaviourPun
{

    [Tooltip("The prefab for big platforms")]
    [SerializeField] private GameObject bigPlatform;
    [Tooltip("The prefab for small platforms")]
    [SerializeField] private GameObject smallPlatform;
    [Tooltip("All the spawnpoints for platforms")]
    [SerializeField] private GameObject[] spawnPoints;
    [Tooltip("The amount of platforms are spawned in total")]
    [SerializeField] private float totalPlatforms = 9f;
    [Tooltip("How long it takes between each platform in seconds")]
    [SerializeField] private float timerMax = 1.3f;
    [Tooltip("The platform players spawn upon")]
    [SerializeField] private GameObject playerPlatform;
    private ArrayList spawnedPlatforms;
    private GameObject playerSpawnPlat;
    private float timer;
    private float randomX;
    private float randomY;
    private int lastRandom = 1;
    private int rndSpawn;
    private bool startedSpawning = false;
    private bool goingRight;

    void Start()
    {
        spawnedPlatforms = new ArrayList();

        if (PhotonNetwork.IsMasterClient)
        playerSpawnPlat = PhotonNetwork.Instantiate(playerPlatform.name, new Vector3(0, -1, 0), transform.rotation);
    }

    [PunRPC]
    void SpawnPlatforms(Vector3 platformPos)
    {
        spawnedPlatforms.Add(PhotonNetwork.Instantiate(smallPlatform.name, platformPos, transform.rotation));
        Debug.Log("Total platforms spawned: " + spawnedPlatforms.Count);
    }

    [PunRPC]
    public Vector3 setRandom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Vector3 randomPos;
            if (!startedSpawning)
            {
                rndSpawn = Random.Range(0, spawnPoints.Length);
                startedSpawning = true;
            } else
            {
                int randomPick = Random.Range(0, 2);
                Debug.Log("randomPicked: " + randomPick + " right: " + (rndSpawn+1) + " left: " + (rndSpawn-1) );
                switch (randomPick)
                {
                    case 0:

                        if ((rndSpawn - 1) >= 0)
                        {
                            rndSpawn = rndSpawn - 1;
                            Debug.Log("New left spawn: " + rndSpawn);
                        }
                        else
                            rndSpawn += 1;

                        break;

                    case 1:

                        if ((rndSpawn + 1) <= (spawnPoints.Length - 1))
                        {
                            rndSpawn = rndSpawn + 1;
                            Debug.Log("New right spawn: " + rndSpawn);
                        } else
                            rndSpawn -= 1;

                        break;
                }
            }
            randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);           

            return randomPos;
        }
        return new Vector3(50,0,0);
    }

    void FixedUpdate()
    {
        if (spawnedPlatforms.Count < totalPlatforms && timer < timerMax && PhotonNetwork.IsMasterClient)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (spawnedPlatforms.Count < totalPlatforms && PhotonNetwork.IsMasterClient)
            {
                Debug.Log("SPAWNING: " + timer);
                photonView.RPC("SpawnPlatforms", RpcTarget.All, setRandom() );
                timer = 0;
            }
        }
    }
}
