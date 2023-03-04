using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class MoleMovement : MonoBehaviour
{
    public float speed = 5;         // Normal speed
    public float modSpeed = -0.2f;  // Speed after digging
    public float rumbly = 3;        // Level of vibration

    [SerializeField]
    private bool autoMove = false;
    private float rumblyFor = 0;    // How long speed is modified
    private Animator whateveryouwant;       // Animator variable
    private HoleDig digger;
    [SerializeField]
    private ParticleSystem dirtParticle;
    private Rigidbody2D rb;
    private Collider2D[] contactsList;

    // Start is called before the first frame update
    void Start()
    {
        whateveryouwant = GetComponentInChildren<Animator>();
        digger = GetComponent<HoleDig>();
        rb = GetComponent<Rigidbody2D>();
        contactsList = new Collider2D[20];
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 mouseDirection = mousePos - transform.position;
        mouseDirection.z = transform.position.z;

        Rotate(mouseDirection);
        Move(mouseDirection, mousePos);
    }

    void Rotate(Vector3 mouseDirection)
    {
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
            whateveryouwant.SetBool("moving", true);
            Vector2 mouseDir2d = mouseDirection.normalized;     // rb movement
            if(rb.GetContacts(contactsList) == 0)
                transform.position += new Vector3(mouseDirection.x, mouseDirection.y, 0).normalized * (speed + (rumblyFor>0?modSpeed:0)) * Time.deltaTime;    // non-rb movement
            rb.MovePosition(rb.position + new Vector2(mouseDirection.x, mouseDirection.y).normalized * (speed + (rumblyFor > 0 ? modSpeed : 0)) * Time.fixedDeltaTime); //rb movement

            // add rumbling
            if (rumblyFor > 0)
            {
                transform.eulerAngles +=  new Vector3(0, 0, Random.Range(-rumbly, rumbly));
                rumblyFor -= Time.deltaTime;
            }

            if (digger.DigHole(transform.position + mouseDirection.normalized * 0.5f))   // hole digger make hole
            {
                dirtParticle.Play();
                rumblyFor = 0.2f;
            }
        }
        else
        {
            whateveryouwant.SetBool("moving", false);
        }
    }
}
