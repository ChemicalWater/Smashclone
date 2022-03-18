using System;
using System.Collections;


using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;


using Photon.Pun;
using Photon.Realtime;


namespace smashclone
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        [Tooltip("All possible prefabs for the player to have")]
        public GameObject[] playerPrefabList;

        [Tooltip("The prefab to use for representing the player")]
        public GameObject playerPrefab;
        [Tooltip("The prefab to use spawning items")]
        public GameObject itemSpawner;
        private GameObject itemSpawnClone;
        [Tooltip("The prefab to use for spawning platforms")]
        public GameObject platformSpawner;
        [Tooltip("The platform players spawn upon")]
        [SerializeField] public GameObject playerPlatform;

        [SerializeField] private bool ItemSpawning;

        void Start()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
            }
            else
            {
                if (playerControl.LocalPlayerInstance == null)
                {
                    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);
                    // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                    PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 0f, 0f), Quaternion.identity, 0);
                    if (PhotonNetwork.IsMasterClient)
                    {
                        if (ItemSpawning)
                        {
                            spawnItems();
                        }
                    }
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
        }
        #region Private Methods

        void LoadArena()
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
            }

            Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);
            PhotonNetwork.LoadLevel("scene_doodle_map");
            this.playerPrefab.GetComponent<playerControl>().health = 1f;
        }

        public void spawnItems()
        {
                itemSpawnClone = PhotonNetwork.Instantiate(this.itemSpawner.name, new Vector3(-0.1138714f, -0.7103133f, 0f), Quaternion.identity, 0);
                ItemSpawning = false;
        }

        #endregion

        #region Photon Callbacks


        public override void OnPlayerEnteredRoom(Player other)
        {
            Debug.LogFormat("OnPlayerEnteredRoom() {0}", other.NickName); // not seen if you're the player connecting


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerEnteredRoom
                LoadArena();
            }
        }


        public override void OnPlayerLeftRoom(Player other)
        {
            Debug.LogFormat("OnPlayerLeftRoom() {0}", other.NickName); // seen when other disconnects


            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogFormat("OnPlayerLeftRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom


                LoadArena();
            }
        }


        #endregion

        #region Photon Callbacks


        /// <summary>
        /// Called when the local player left the room. We need to load the launcher scene.
        /// </summary>
        public override void OnLeftRoom()
        {
            SceneManager.LoadScene("main_Home");
        }


        #endregion


        #region Public Methods


        public void LeaveRoom()
        {
            PhotonNetwork.LeaveRoom();
        }


        #endregion
    }
}