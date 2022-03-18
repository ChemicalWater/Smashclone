using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace smashclone
{
    public class spawnPlatforms : MonoBehaviourPun
    {

        [Tooltip("The prefab for big platforms")]
        [SerializeField]
        private GameObject bigPlatform;
        [Tooltip("The prefab for small platforms")]
        [SerializeField]
        private GameObject smallPlatform;
        [Tooltip("All the spawnpoints for platforms")]
        [SerializeField]
        private GameObject[] spawnPoints;
        [Tooltip("The amount of platforms are spawned in total")]
        [SerializeField]
        private float totalPlatforms = 9f;
        [Tooltip("How long it takes between each platform in seconds")]
        [SerializeField]
        private float timerMax = 1.3f;
        [Tooltip("How long it takes for the player platform to dissapear")]
        [SerializeField]
        private float dissapearTime = 8f;

        //  private float platformTime;
        //  private SpriteRenderer[] playerSpawnPlatform;
        private List<GameObject> spawnedPlatforms;

        private float timer;
        private float randomX;
        private float randomY;
        private bool removePlat = false;
        private int rndSpawn;
        private bool startedSpawning = false;

        void Start()
        {
            spawnedPlatforms = new List<GameObject>();
        }

        [PunRPC]
        void SpawnPlatforms(Vector3 platformPos)
        {
            if (PhotonNetwork.IsMasterClient)
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
                }
                else
                {
                    int randomPick = Random.Range(0, 2);
                    switch (randomPick)
                    {
                        case 0:

                            if ((rndSpawn - 1) >= 0)
                            {
                                rndSpawn = rndSpawn - 1;
                            }
                            else
                                rndSpawn += 1;

                            break;

                        case 1:

                            if ((rndSpawn + 1) <= (spawnPoints.Length - 1))
                            {
                                rndSpawn = rndSpawn + 1;
                            }
                            else
                                rndSpawn -= 1;

                            break;
                    }
                }
                randomPos = new Vector3(spawnPoints[rndSpawn].transform.position.x, spawnPoints[rndSpawn].transform.position.y, 0);

                return randomPos;
            }
            return new Vector3(50, 0, 0);
        }

        [PunRPC]
        void blipPlatform()
        {
            if (GameObject.FindGameObjectWithTag("spawnplatform") != null)
                GameObject.FindGameObjectWithTag("spawnplatform").SetActive(false);
        }

        void SpeedUp(float speed)
        {
            foreach (GameObject platform in spawnedPlatforms)
            {
                platform.GetComponent<movePlatforms>().ChangeSpeed(speed);
            }
        }

        void FixedUpdate()
        {
            if (spawnedPlatforms.Count < totalPlatforms && timer < timerMax && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1) // PhotonNetwork.CurrentRoom.PlayerCount > 1 to make it wait for 2 players
            {
                timer += Time.deltaTime;
            }
            else
            {
                if (spawnedPlatforms.Count < totalPlatforms && PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom.PlayerCount > 1) // PhotonNetwork.CurrentRoom.PlayerCount > 1 to make it wait for 2 players
                {
                    photonView.RPC("SpawnPlatforms", RpcTarget.All, setRandom());
                    timer = 0;
                }
            }
            if (spawnedPlatforms.Count == 7 && !removePlat)
            {
                photonView.RPC("blipPlatform", RpcTarget.All);
                removePlat = true;
            }

           // if (GameObject.FindGameObjectWithTag("Player").transform.position.y < 0.3)
           //     SpeedUp(0.008f);
           // else if (GameObject.FindGameObjectWithTag("Player").transform.position.y < 0.6)
           //     SpeedUp(0.015f);
           // else
           //     SpeedUp(0.03f);
        }
    }
}
