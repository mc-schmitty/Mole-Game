using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Collider2D))]
public class Grub : HiddenObject
{
    [SerializeField]
    private ParticleSystem[] squishParticles; 
    [SerializeField]
    private AudioClip squishNoise;
    [SerializeField]
    private AudioClip revealNoise;
    private AudioSource squishPlayer;

    private void Start()
    {
        base.Initilize();
        //squishParticles = GetComponent<ParticleSystem>();
        squishPlayer = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.attachedRigidbody.CompareTag("Player"))
        {
            Squash();
        }
    }

    public void Squash()
    {
        //squishParticles.Play();
        //AudioSource.PlayClipAtPoint(squishNoise, transform.position);
        foreach(ParticleSystem party in squishParticles)
            Instantiate(party, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    public override void Reveal()
    {
        squishPlayer.PlayOneShot(revealNoise);
        base.Reveal();
    }
}
