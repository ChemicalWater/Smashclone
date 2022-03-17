using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class mapChoice : MonoBehaviourPun
{
    [SerializeField] private Sprite[] backgrounds;
    [SerializeField] private Sprite[] movingBigPlatforms;
    [SerializeField] private Sprite[] movingSmallPlatforms;
    [SerializeField] private GameObject bigPlatforms;
    [SerializeField] private GameObject smallPlatforms;
    [SerializeField] private GameObject background;

    private SpriteRenderer bgSprite;
    private SpriteRenderer smallSprite;
    private SpriteRenderer bigSprite;


    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("changeMap", RpcTarget.AllBuffered, 0);
        }
        bgSprite = background.GetComponent<SpriteRenderer>();
        //bigSprite = bigPlatforms.GetComponent<SpriteRenderer>();
        //smallSprite = smallPlatforms.GetComponent<SpriteRenderer>();
    }

    [PunRPC]
    void changeMap(int map)
    {
        bgSprite.sprite = backgrounds[map];
       // bigSprite.sprite = movingBigPlatforms[map];
       // smallSprite.sprite = movingSmallPlatforms[map];
    }
}
