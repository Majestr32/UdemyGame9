using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    private UI ui;

    public AudioSource jetpackAudio;
    public AudioSource footstepsAudio;
    public AudioClip audioCoinPickup;

    public float jetpackForce = 75.0f;
    public float forwardMoveSpeed = 3.0f;

    private Rigidbody2D rb;
    private Animator anim;
    public Transform groundCheckTransform;

    public float checkRadius = 0.1f;
    private bool isGrounded;

    public LayerMask groundCheckLayerMask;

    private uint coins = 0;

    public ParticleSystem jetpack;
    public ParallaxScroll parallax;

    private bool isDead;
    void Awake()
    {
        //Making mouse initial position relative to screen size
        Vector3 cameraCenter = Camera.main.transform.position;
        float cameraHeight = 2f * Camera.main.orthographicSize;
        float cameraWidth = cameraHeight * Camera.main.aspect;
        transform.position = new Vector3(cameraCenter.x - (cameraWidth / 2) * 0.8f, 0f, 0f);

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        ui = FindObjectOfType<UI>();
    }
    void FixedUpdate()
    {
        bool jetpackActive = (Input.touchCount > 0) && !isDead;
        if(jetpackActive)
        {
            rb.AddForce(new Vector2(0f,jetpackForce));
        }
        if(!isDead)
        {
            Vector2 newVelocity = rb.velocity;
            newVelocity.x = forwardMoveSpeed;
            rb.velocity = newVelocity;
        }
        parallax.offset = transform.position.x;
        UpdateGroundedStatus();
        AdjustJetpack(jetpackActive);
    }
    void UpdateGroundedStatus()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheckTransform.position,checkRadius,groundCheckLayerMask);
        anim.SetBool("isGrounded",isGrounded);
    }
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheckTransform.position,checkRadius);
    }
    void AdjustJetpack(bool jetpackActive)
    {
        var jetpackEmission = jetpack.emission;
        footstepsAudio.enabled = !isDead && isGrounded;
        jetpackAudio.enabled = !isDead && !isGrounded;
        jetpackEmission.enabled = !isGrounded;
        if(jetpackActive)
        {
            jetpackEmission.rateOverTime = 300f;
            jetpackAudio.volume = 1f;
        }
        else 
        {
            jetpackEmission.rateOverTime = 75f;
            jetpackAudio.volume = 0.5f;
        }
    }
    void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.CompareTag("Coins"))
        {
            CollectCoins(collider);
        }
        else
        {
            CollideLazer(collider);
        }
    }
    public void CollectCoins(Collider2D collider)
    {
        AudioSource.PlayClipAtPoint(audioCoinPickup, transform.position);
        ui.Redraw(++coins);
        Destroy(collider.gameObject);
    }
    public void CollideLazer(Collider2D collider)
    {
        if (!isDead)
        {
            AudioSource laserZap = collider.gameObject.GetComponent<AudioSource>();
            laserZap.Play();
            isDead = true;
            anim.SetBool("isDead", true);
            anim.SetTrigger("Die");
            ui.ShowRestart();
        }
    }
}
