using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MoleMovement : MonoBehaviour
{
    // todo: clean up variables

    public float speed = 5;         // Normal speed
    public float digSpeedModifier = -0.2f;  // Speedmod after digging
    public float eatSpeedModifier = -4f;    // Speedmod after eating
    public float rumbly = 3;        // Level of vibration

    [SerializeField]
    private float eatSlowdownDuration = 0.2f;   // How long you are slowed down after eating
    [SerializeField]
    private bool autoMove = false;
    [SerializeField]
    private bool transformMove = true;
    private float transMoveTimer = 0;
    private float rumblyFor = 0;    // How long speed is modified
    private float eatMoveTimer = 0;     // How long speed is modified after eating
    private Animator whateveryouwant;       // Animator variable
    private HoleDig digger;
    [SerializeField]
    private ParticleSystem dirtParticle;
    [SerializeField]
    private SniffDrawer sniffDrawer;
    private Rigidbody2D rb;
    private Collider2D[] contactsList;
    [SerializeField]
    private AudioSource eatAudioSource;
    [SerializeField]
    private AudioSource digAudioSource;
    private float defaultDigVolume;
    [SerializeField]
    private AudioClip[] eatNoise;
    [SerializeField]
    private AudioClip[] digNoise;

    // Start is called before the first frame update
    void Start()
    {
        whateveryouwant = GetComponentInChildren<Animator>();
        digger = GetComponent<HoleDig>();
        rb = GetComponent<Rigidbody2D>();
        contactsList = new Collider2D[20];
        defaultDigVolume = digAudioSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDirection = mousePos - transform.position;
        mouseDirection.z = transform.position.z;

        // Check for physics collision
        if(rb.GetContacts(contactsList) > 0)
        {
            transformMove = false;      // Disable transform movement if we collide for a bit
            transMoveTimer = 0.15f;
        }

        Rotate(mouseDirection);
        Move(mouseDirection, mousePos);

        if (transMoveTimer <= 0)
            transformMove = true;
        transMoveTimer = Mathf.Clamp(transMoveTimer - Time.deltaTime, 0, transMoveTimer);   // Enable transform move after no collisions for a bit
    }

    void Rotate(Vector3 mouseDirection)
    {
        rb.angularVelocity = 0;
        if (transformMove)
            transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.left, mouseDirection));       // Rotates towards the mouse
        rb.SetRotation(Vector2.SignedAngle(Vector2.left, mouseDirection));

        float zCheck = transform.eulerAngles.z;
        if (zCheck > 90 && zCheck < 270)    // mole switches y-scale when rotated to the right
        {
            transform.localScale = new Vector3(1, -1, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, transform.localScale.z);
        }
    }

    void Move(Vector3 mouseDirection, Vector3 mousePos)
    {
        rb.velocity = Vector2.zero;
        if (autoMove || Input.GetMouseButton(0) && mouseDirection.magnitude > 1f)       // make mole move towards mouse
        {
            sniffDrawer.gainSniff = false;
            whateveryouwant.SetBool("moving", true);
            Vector2 mouseDir2d = mouseDirection.normalized;     // rb movement
            float eatMod = Mathf.Lerp(0, eatSpeedModifier, Mathf.InverseLerp(0, eatSlowdownDuration, eatMoveTimer));
            if (transformMove && rb.GetContacts(contactsList) == 0)
                transform.position += new Vector3(mouseDirection.x, mouseDirection.y, 0).normalized * (speed + (rumblyFor > 0 ? digSpeedModifier : 0) + eatMod) * Time.deltaTime;    // non-rb movement
            rb.MovePosition(rb.position + new Vector2(mouseDirection.x, mouseDirection.y).normalized * (speed + (rumblyFor > 0 ? digSpeedModifier : 0) + eatMod) * Time.fixedDeltaTime); //rb movement

            // add rumbling
            if (rumblyFor > 0)
            {
                transform.eulerAngles +=  new Vector3(0, 0, Random.Range(-rumbly, rumbly));
                rumblyFor -= Time.deltaTime;
            }

            if (digger.DigHole(transform.position + mouseDirection.normalized * 0.5f))   // hole digger make hole
            {
                digAudioSource.clip = digNoise[Random.Range(0, digNoise.Length)];      // play random dig noise
                if (eatAudioSource.isPlaying)      // Dampen noise if currently playing eat noise
                {
                    digAudioSource.volume = defaultDigVolume * 0.5f;
                }
                else   // otherwise set to default volume
                {
                    digAudioSource.volume = defaultDigVolume;
                }
                digAudioSource.Play();
                dirtParticle.Play();        // also play dirt digging particles
                rumblyFor = 0.2f;           // and make mole rumbly
            }
        }
        else
        {
            sniffDrawer.gainSniff = true;
            whateveryouwant.SetBool("moving", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)     // Right now only triggers on grubs (eat)
    {
        if (collision.CompareTag("Food"))
        {
            eatAudioSource.clip = eatNoise[Random.Range(0, eatNoise.Length)];      // Play random eating noise
            eatAudioSource.Play();
            StartCoroutine(EatTimer(eatSlowdownDuration));
        }
    }

    // Testing out a better way of doing timers
    IEnumerator EatTimer(float timer)
    {
        if(eatMoveTimer != 0)       // Prevent multiple coroutine calls, or calling in the middle of an action
        {
            yield break;
        }
        // So now i realize that we might want to call a new coroutine and stop the old one, but it is late and i am too tired to do that so above code works for now

        eatMoveTimer = timer;
        while(eatMoveTimer > 0)
        {
            yield return null;
            eatMoveTimer = Mathf.Clamp(eatMoveTimer - Time.deltaTime, 0, eatMoveTimer);
        }
    }
}
