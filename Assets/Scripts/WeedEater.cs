using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeedEater : MonoBehaviour
{
    private MoleMovement mm;

    // Script exists to isolate the mouth collider from other mole colliders (so only mouth colliding triggers slowdown)
    void Start()
    {
        mm = GetComponentInParent<MoleMovement>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Food"))   // slowdown after eating food
        {
            mm.EatFoodSlowdown();
        }
    }
}
