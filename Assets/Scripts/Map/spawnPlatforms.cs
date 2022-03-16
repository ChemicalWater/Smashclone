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
    private int lastRandom = 1;
    private int rndSpawn;
    private bool startedSpawning = false;
    private bool goingRight;

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
            Vector3 randomPos;
            if (!startedSpawning)
            {
                rndSpawn = Random.Range(0, spawnPoints.Length);
                startedSpawning = true;
            } else
            {
                int randomPick = Random.Range(0, 2);
                Debug.Log("RandomPicked: " + randomPick + " right: " + (rndSpawn+1) + " left: " + (rndSpawn-1) );

                if (randomPick == 0 && (rndSpawn - 1) != -1 )
                {
                    rndSpawn = rndSpawn - 1;
                    Debug.Log("New left spawn: " + rndSpawn);
                }
                if ( (rndSpawn -1 ) == -1)
                {
                    rndSpawn = rndSpawn + 1;
                    Debug.Log("Cant go left");
                }

                if (randomPick == 1 && (rndSpawn + 1) != (spawnPoints.Length +1))
                {
                    rndSpawn = rndSpawn + 1;
                    Debug.Log("New right spawn: " + rndSpawn);
                }
                if ( (rndSpawn+1) == spawnPoints.Length +1)
                {
                    rndSpawn = rndSpawn - 1;
                    Debug.Log("Cant go right");
                }
            }
            randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);

            // Random spawning attempt 2
            // Vector3 randomPos;
            // if (rndSpawn != lastRandom && !startedSpawning)
            // {
            //     rndSpawn = Random.Range(0, spawnPoints.Length);
            //     lastRandom = rndSpawn;
            //     Debug.Log("Random chosen start: " + rndSpawn);
            //     randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            //     startedSpawning = true;
            // }
            // else
            // {
            //     if (lastRandom <= spawnPoints.Length && goingRight)
            //     {
            //         lastRandom += 1;
            //         Debug.Log("Going right: " + lastRandom);
            //     }
            //     else
            //     {
            //         lastRandom -= 1;
            //         Debug.Log("Going left: " + lastRandom);
            //
            //         randomPos = new Vector3(spawnPoints[lastRandom].transform.position.x, spawnPoints[lastRandom].transform.position.y, 0);
            //     }
            // }

            // Random spawning attempt 1
            //rndSpawn = Random.Range(0, spawnPoints.Length);
            //Vector3 randomPos;

            // if (rndSpawn != lastRandom)
            // {
            //     lastRandom = rndSpawn;
            //     randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            // } else
            // {
            //     if (rndSpawn != spawnPoints.Length)
            //         rndSpawn += 1;
            //     else
            //         rndSpawn = Random.Range(0, spawnPoints.Length);
            //
            //     randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            // }

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
