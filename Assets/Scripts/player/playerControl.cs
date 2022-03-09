using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using UnityEngine.UI;


public class playerControl : MonoBehaviourPun, IPunObservable
{
    private Rigidbody2D body;
    private float vertical;
    private float horizontal;
    private Animator animPlayer;
    [SerializeField]
    float maxSpd = 1f;
    [SerializeField]
    float jumpHeight = 4f;
    private bool allowJump = true;
    private bool punching;
    [Tooltip("The amount of force a Player's punch packs")]
    [SerializeField] private float punchPower = 5f;
    public bool facingRight = true;
    private bool usingItem = false;
    public float health = 1f;
    [Tooltip("How much damage does one player hit do?")]
    [SerializeField] private float dmgHit = 0.1f;

    [Tooltip("The Player's UI GameObject Prefab")]
    [SerializeField]
    public GameObject PlayerUiPrefab;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;

    void Awake()
    {
        // #Important
        // used in GameManager.cs: we keep track of the localPlayer instance to prevent instantiation when levels are synchronized
        if (photonView.IsMine)
        {
            playerControl.LocalPlayerInstance = this.gameObject;
        }

        if (!photonView.IsMine)
        {
            gameObject.tag = "Enemy";
        }
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animPlayer = GetComponent<Animator>();

       // if (PlayerUiPrefab != null)
       // {
       //    GameObject UI = Instantiate(PlayerUiPrefab);
       //     UI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
       // }
       // else
       // {
       //     Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
       // }

    }

    void Update()
    {
        if (photonView.IsMine == false && PhotonNetwork.IsConnected == true)
        {
            return;
        }
    }

    #region IPunObservable implementation


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send the others our data
            stream.SendNext(punching);
            stream.SendNext(usingItem);
            stream.SendNext(health);
            stream.SendNext(body.velocity);
        }
        else
        {
            // Network player, receive data
            this.transform.GetChild(0).gameObject.SetActive( (bool)stream.ReceiveNext() );
            this.usingItem = (bool)stream.ReceiveNext();
            this.health = (float)stream.ReceiveNext();
            this.body.velocity = (Vector2)stream.ReceiveNext();
        }
    }

    #endregion

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            vertical = Input.GetAxisRaw("Vertical");
            horizontal = Input.GetAxisRaw("Horizontal");
            SetRigidBodyVelocity();
            TriggerAnimations();

            if (Input.GetKey(KeyCode.W))
            {
                if (allowJump)
                {
                    body.AddForce(new Vector3(0, jumpHeight, 0), ForceMode2D.Impulse);
                    allowJump = false;
                }
            }

            if (Input.GetKey(KeyCode.Space))
            {
                if (!punching)
                {
                    punching = true;
                    this.transform.GetChild(0).gameObject.SetActive(punching);
                }
            }
            else
            {
                punching = false;
                this.transform.GetChild(0).gameObject.SetActive(punching);
            }

            if (Input.GetKey(KeyCode.F) && this.GetComponent<playerInventory>().Items.Count >= 1)
            {
                usingItem = true;
                photonView.RPC("addHealth", transform.GetComponent<PhotonView>().Controller, GetComponent<playerInventory>().useItem("Item_Health"));
            }
            else
                usingItem = false;

            if (this.health <= 0)
                Dead();
        }
    }
    void Dead()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }
    }
    void CalledOnLevelWasLoaded()
    {
        //GameObject UI = Instantiate(this.PlayerUiPrefab);
        //UI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    [PunRPC]
    public void takeHealth(float remHealth)
    {
        Debug.Log("TAKING DAMAGE");
        Debug.Log(remHealth);
            health -= remHealth;
            body.AddForce(new Vector2(((1 - health) * (punchPower)), 0), ForceMode2D.Impulse);
    }

    [PunRPC]
    public void addHealth(float addHealth)
    {
        if (photonView.IsMine)
            health += addHealth;
    }

    void TriggerAnimations()
    {
        if (facingRight && usingItem && allowJump)
        {
            animPlayer.SetBool("use_right", true);
            body.velocity = new Vector3(0, 0);
        }
        else
            animPlayer.SetBool("use_right", false);

        if (!facingRight && usingItem && allowJump)
        {
            animPlayer.SetBool("use_left", true);
            body.velocity = new Vector3(0, 0);
        }
        else
            animPlayer.SetBool("use_left", false);

        if (punching && facingRight)
            animPlayer.SetBool("punch_right", true);
        else
            animPlayer.SetBool("punch_right", false);

        if (punching && !facingRight)
            animPlayer.SetBool("punch_left", true);
        else
            animPlayer.SetBool("punch_left", false);

        if (body.velocity.x > 0)
        {
            facingRight = true;
            animPlayer.SetBool("walk_right", true);
        }
        else
            animPlayer.SetBool("walk_right", false);

        if (body.velocity.x < 0)
        {
            facingRight = false;
            animPlayer.SetBool("walk_left", true);
        }
        else
            animPlayer.SetBool("walk_left", false);

        if (body.velocity.y > 0 && body.velocity.x > 0 || body.velocity.y > 0 && facingRight)
            animPlayer.SetBool("jump_right", true);
        else
            animPlayer.SetBool("jump_right", false);

        if (body.velocity.y > 0 && body.velocity.x < 0 || body.velocity.y > 0 && !facingRight)
            animPlayer.SetBool("jump_left", true);
        else
            animPlayer.SetBool("jump_left", false);

    }

    void Test()
    {
        photonView.RPC("takeHealth", GameObject.FindGameObjectWithTag("Enemy").transform.GetComponent<PhotonView>().Controller, dmgHit);
    }

    void SetRigidBodyVelocity()
    {
        Vector2 xVel = body.velocity;
        xVel.x = horizontal;
        body.velocity = xVel * maxSpd;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Enemy" && other.GetComponent<playerControl>() != null)
        {
            Test();
            Debug.Log("My health: " + GetComponent<playerControl>().health);
            Debug.Log("Enemy health: " + other.GetComponent<playerControl>().health);
        }

        if (other.tag == "Respawn")
            Dead();
    }
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "platforms" && transform.position.y > other.transform.position.y)
        {
            allowJump = true;
        }
    }
}
