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

    private Vector3 scaredDirection;
    private Transform playerPos;
    private Coroutine runningRoutine;
    

    void Start()
    {
        base.Initilize();
        squishPlayer = GetComponent<AudioSource>();
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
        scaredDirection.z = 0;                                          // Get rid of Z
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
    }

    IEnumerator RunAway()
    {
        // snap rotate away from player
        transform.eulerAngles = new Vector3(0, 0, Vector2.SignedAngle(Vector2.right, scaredDirection));
        // ok for now im not bothering with a curve
        float timeStep = 0;
        while (IsRunning)
        {
            transform.position += scaredDirection * Mathf.Lerp(initSpeed, maxSpeed, timeStep) * Time.deltaTime;
            timeStep = Mathf.Min(timeStep + Time.deltaTime, 1);     // temp, takes 1 second to accelerate to max
            yield return null;
        }
    }
}
