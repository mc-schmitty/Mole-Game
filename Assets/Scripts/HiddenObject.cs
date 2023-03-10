using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenObject : MonoBehaviour
{
    public float revealRange = 50f;
    public float hideRange = 100f;
    public GameObject scentSprite;
    public bool IsHidden
    {
        get;
        private set;
    }
    private float zLevel;

    void Start()
    {
        Initilize();
    }

    protected virtual void Initilize()
    {
        zLevel = transform.position.z;
        IsHidden = true;
    }

    public virtual void Hide()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, zLevel);
        IsHidden = true;
    }

    public virtual void Reveal()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
        IsHidden = false;
    }
}
