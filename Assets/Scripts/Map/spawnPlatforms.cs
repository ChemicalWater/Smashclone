using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class spawnPlatforms : MonoBehaviourPun
{

    [SerializeField] private GameObject bigPlatform;
    [SerializeField] private GameObject smallPlatform;
    [SerializeField] private GameObject[] spawnPoints;
    [SerializeField] private float totalPlatforms = 4f;
    [SerializeField] private float timerMax = 3f;
    private ArrayList spawnedPlatforms;
    private float timer;
    private float randomX;
    private float randomY;

    void Start()
    {
        spawnedPlatforms = new ArrayList();
    }

    [PunRPC]
    void SpawnPlatforms(Vector3 platformPos)
    {
        spawnedPlatforms.Add(PhotonNetwork.Instantiate(smallPlatform.name, platformPos, transform.rotation));
    }
    
    [PunRPC]
    public Vector3 setRandom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            int rndSpawn = Random.Range(0, spawnPoints.Length);
            Vector3 randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);
            return randomPos;
        }
        return new Vector3(50,0,0);
    }


    void Update()
    {
        if (spawnedPlatforms.Count < totalPlatforms && timer < timerMax && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            timer += Time.deltaTime;
        }
        else
        {
            if (spawnedPlatforms.Count < totalPlatforms && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1)
            {
                photonView.RPC("SpawnPlatforms", RpcTarget.All, setRandom());
                timer = 0;
            }
        }
    }
}
