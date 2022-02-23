using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class playerControl : MonoBehaviour
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

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animPlayer = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        SetRigidBodyVelocity();
        TriggerAnimations();

        if (Input.GetKey(KeyCode.Space))
        {
            if (allowJump || body.velocity.y == 0)
            {
                body.AddForce(new Vector3(0, jumpHeight, 0), ForceMode2D.Impulse);
                Debug.Log(PhotonNetwork.NickName);
                allowJump = false;
            }
        }
    }

    void TriggerAnimations()
    {
        if (body.velocity.x > 0)
            animPlayer.SetBool("walk_right", true);
        else
            animPlayer.SetBool("walk_right", false);

        if (body.velocity.x < 0)
            animPlayer.SetBool("walk_left", true);
        else
            animPlayer.SetBool("walk_left", false);

        if (body.velocity.y > 0 && body.velocity.x > 0)
            animPlayer.SetBool("jump_right", true);
        else
            animPlayer.SetBool("jump_right", false);

        if (body.velocity.y > 0 && body.velocity.x < 0)
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
            body.velocity += newVelocity;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "platforms" && transform.position.y > other.transform.position.y)
        {
            allowJump = true;
        }
    }
}
