using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoleMovement : MonoBehaviour
{
    public float speed = 5;         // Normal speed
    public float modSpeed = -0.2f;  // Speed after digging
    public float rumbly = 3;        // Level of vibration

    private float rumblyFor = 0;    // How long speed is modified
    private Animator whateveryouwant;       // Animator variable
    private HoleDig digger;
    [SerializeField]
    private ParticleSystem dirtParticle;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        whateveryouwant = GetComponentInChildren<Animator>();
        digger = GetComponent<HoleDig>();
        //rb = GetComponent<Rigidbody2D>();
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
        //rb.SetRotation(Vector2.SignedAngle(Vector2.left, mouseDirection));

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
        //rb.velocity = Vector2.zero;
        if (Input.GetMouseButton(0) && mouseDirection.magnitude > 1f)       // make mole move towards mouse
        {
            whateveryouwant.SetBool("moving", true);
            //Vector2 mouseDir2d = mouseDirection.normalized;
            //transform.position = Vector3.MoveTowards(transform.position, mousePos, speed * Time.deltaTime);
            transform.position += new Vector3(mouseDirection.x, mouseDirection.y, 0).normalized * (speed * Time.deltaTime);
            //rb.AddForce(Vector2.left * (speed + rumblyFor>0?modSpeed:0), ForceMode2D.Impulse);
            //rb.MovePosition(Vector3.MoveTowards(transform.position, mousePos, speed * Time.deltaTime));
            //rb.MovePosition(rb.position + mouseDir2d * (speed * Time.fixedDeltaTime));

            // add rumbling
            if (rumblyFor > 0)
            {
                transform.eulerAngles += new Vector3(0, 0, Random.Range(-rumbly, rumbly));
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
