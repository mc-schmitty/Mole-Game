using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MovingGrub : Grub
{
    [SerializeField]
    private float speed = 2f;
    [SerializeField]
    private float timeMoving = 3f;      // Time spent moving
    [SerializeField]
    private float timeWaiting = 7f;     // Time spent waiting
    [SerializeField]
    private float randTimeMod = 0.2f;   // Random modification to time waiting/moving
    [SerializeField]
    private float maxWanderRange = 30f;

    [SerializeField]
    private Vector3 homePoint;
    private ParticleSystem dirtParticles;
    private bool isMoving;
    private Coroutine activeCoroutine;
    private Animator animator;       // Animator variable
    private Collider2D wormCollider; // how we collide

    private void OnValidate()
    {
        homePoint = transform.position;
    }

    void Start()
    {
        base.Initilize();
        squishPlayer = GetComponent<AudioSource>();
        dirtParticles = GetComponent<ParticleSystem>();
        animator = GetComponent<Animator>();
        isMoving = false;
        homePoint = transform.position;
        wormCollider = GetComponent<Collider2D>();

        activeCoroutine = StartCoroutine(BehaviorChange(1f));
    }

    private void Update()
    {
        if (isMoving)
        {
            transform.position += transform.right * speed * Time.deltaTime;
        }
    }

    private void StopMoving()
    {
        isMoving = false;
        // more stuff later maybe, like animation changes
        //dirtParticles.Stop();
        animator.SetBool("isMoving", isMoving);
        animator.SetFloat("cycleOffset", Random.Range(0, 3));   // Makes worm stop at random position
    }

    private void StartMoving()
    {
        isMoving = true;
        // Rotate to a random direction
        Vector3 rotEuler = transform.eulerAngles;
        if(Vector3.Distance(transform.position, homePoint) > maxWanderRange)        // Move towards home
        {
            transform.eulerAngles = new Vector3(rotEuler.x, rotEuler.y, Vector2.SignedAngle(Vector2.right, homePoint - transform.position));
        }
        else
        {
            //transform.rotation = Quaternion.Euler(rotEuler.x, rotEuler.y, Random.Range(0, 360));
            transform.eulerAngles = new Vector3(rotEuler.x, rotEuler.y, Random.Range(0, 360));
        }
        //dirtParticles.Play();
        animator.SetBool("isMoving", isMoving);
        
    }

    public override void Squash(Transform playerRef)
    {
        //StopMoving();
        isMoving = false;
        StopCoroutine(activeCoroutine);         // Stop movement behaviour
        animator.SetTrigger("getEaten");
        dirtParticles.Stop();
        wormCollider.enabled = false;

        StartCoroutine(BeingEaten(playerRef, 0.8f));      // Begin getting eaten
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(homePoint, maxWanderRange);
    }

    // Switches between moving and stopping behaviour
    IEnumerator BehaviorChange(float delay)
    {
        while(delay > 0)
        {
            delay -= Time.deltaTime;
            yield return null;
        }

        if (isMoving)
        {
            StopMoving();
            activeCoroutine = StartCoroutine(BehaviorChange(timeWaiting + Random.Range(-randTimeMod * timeWaiting, randTimeMod * timeWaiting)));
        }
        else
        {
            StartMoving();
            activeCoroutine = StartCoroutine(BehaviorChange(timeMoving + Random.Range(-randTimeMod * timeMoving, randTimeMod * timeMoving)));
        }

    }

    // Delay before getting spaghetti'd up
    IEnumerator BeingEaten(Transform playerPos, float delay)
    {
        Vector3 direction = playerPos.position - transform.position;    // Get direction of player
        direction.z = 0;                                                // Have to get rid of the stupid z 
        direction.Normalize();
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.left, direction));
        transform.position = new Vector3(playerPos.position.x, playerPos.position.y, transform.position.z);

        transform.parent = playerPos;

        // delay so animation plays out
        while (delay > 0)
        {
            
            delay -= Time.deltaTime;
            yield return null;
        }

        // Play the particle effects and explode
        base.Squash(playerPos);
    }
}
