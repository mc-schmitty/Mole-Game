using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScaredyGrub : Grub
{
    [SerializeField]
    private float maxSpeed = 4f;
    [SerializeField]
    private float initSpeed = 1f;
    [SerializeField]
    private AnimationCurve animationCurve;
    public bool IsRunning       // Idk why im using a property
    {
        private set;
        get;
    }

    private Rigidbody2D rb;
    private Vector2 scaredDirection;
    private Transform playerPos;
    private Coroutine runningRoutine;
    

    void Start()
    {
        base.Initilize();
        squishPlayer = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        //dirtParticles = GetComponent<ParticleSystem>();
        //animator = GetComponent<Animator>();
        IsRunning = false;
    }

    public override void Reveal()
    {
        base.Reveal();

        if(playerPos == null)
            playerPos = FindObjectOfType<MoleMovement>().transform;     // Get position of player using find please stop hitting me
        scaredDirection = transform.position - playerPos.position;
        scaredDirection.Normalize();

        IsRunning = true;       // Flag that we start running
        runningRoutine = StartCoroutine(RunAway());
    }

    public override void Hide()
    {
        base.Hide();

        IsRunning = false;
        StopCoroutine(runningRoutine);      // So coroutine should stop naturally but hey whatever
    }

    private void OnCollisionEnter2D(Collision2D collision)      // quick fix, will be refined later
    {
        IsRunning = false;
        StopCoroutine(runningRoutine);
        

        Debug.Log(collision.contactCount);
        foreach (var item in collision.contacts)
        {
            Debug.DrawRay(item.point, item.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        }
        scaredDirection = Vector2.Perpendicular(collision.GetContact(0).normal);    // Run perpendicular to normal now
        runningRoutine = StartCoroutine(GetStunned(0.5f));

    }

    IEnumerator RunAway()
    {
        // Stop movement
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;

        // snap rotate away from player
        rb.SetRotation(Vector2.SignedAngle(Vector2.right, scaredDirection));
        // ok for now im not bothering with a curve
        float timeStep = 0;
        while (IsRunning)
        {
            rb.MovePosition(rb.position + scaredDirection * Mathf.Lerp(initSpeed, maxSpeed, timeStep) * Time.fixedDeltaTime);
            timeStep = Mathf.Min(timeStep + Time.fixedDeltaTime, 1);     // temp, takes 1 second to accelerate to max
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator GetStunned(float timeStunned)
    {
        // setup animation stuff here to show we're stunned

        while(timeStunned > 0)
        {
            yield return null;
            timeStunned -= Time.deltaTime;
        }

        // ok stun over, now run again
        IsRunning = true;
        runningRoutine = StartCoroutine(RunAway());
        // Running direction was previously set

    }
}
