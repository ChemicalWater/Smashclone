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
    float maxSpd = 0.5f;
    [SerializeField]
    float jumpHeight = 8f;
    private bool allowJump = true;
    private bool punching;
    private bool facingRight = true;
    private bool usingItem = false;
    public float health = 1f;

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
        // #Critical
        // we flag as don't destroy on load so that instance survives level synchronization, thus giving a seamless experience when levels load.
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animPlayer = GetComponent<Animator>();

        if (PlayerUiPrefab != null)
        {
            GameObject UI = Instantiate(PlayerUiPrefab);
            UI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

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
            this.punching = (bool)stream.ReceiveNext();
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
                punching = true;
            else
                punching = false;

            if (Input.GetKey(KeyCode.F))
                usingItem = true;
            else
                usingItem = false;
        }
    }

    void CalledOnLevelWasLoaded()
    {
        GameObject UI = Instantiate(this.PlayerUiPrefab);
        UI.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
    }

    public void takeHealth(float remHealth)
    {
        if (photonView.IsMine)
        {
            health -= remHealth;
            body.AddForce(new Vector2(health, 0), ForceMode2D.Impulse);
        }
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
    void SetRigidBodyVelocity()
    {
        float speed = Vector2.SqrMagnitude(body.velocity);
        if (speed > maxSpd)

        {
            float brakeSpeed = speed - maxSpd;

            Vector2 normalisedVelocity = body.velocity.normalized;
            Vector2 brakeVelocity = normalisedVelocity * brakeSpeed;

            body.AddForce(-brakeVelocity);
        }
        else
        {
            Vector2 newVelocity = new Vector3(horizontal, 0);
            body.velocity += newVelocity.normalized;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && Input.GetKey(KeyCode.Space))
            other.GetComponent<playerControl>().takeHealth(0.1f);
        if (other.tag == "platforms" && transform.position.y > other.transform.position.y)
        {
            allowJump = true;
        }
    }
}
