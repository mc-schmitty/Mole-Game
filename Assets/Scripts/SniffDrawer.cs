using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniffDrawer : MonoBehaviour
{
    public float sniffStrength;
    public bool gainSniff;     // Whether we should gain sniffStrength;
    [SerializeField]
    private float radius = 1f;
    [SerializeField]
    private int queueSize = 60;
    [SerializeField]
    private float decayLinear = 10f;
    [SerializeField]
    private float decayConstant = 0f;
    [SerializeField]
    private float decayExponential = 0f;
    [SerializeField]
    private float decayExponentialMax = 0f;
    private float decayExpoValue;
    [SerializeField]
    private float sniffMax = 200f;
    [SerializeField]
    private float angleRange = 60;
    [SerializeField]
    private float sniffRange = 300;

    private Queue<Vector3> positionQueue;
    private Dictionary<HiddenObject, SpriteRenderer> spriteDict;

    void Start()
    {
        sniffStrength = 0;
        positionQueue = new Queue<Vector3>();
        decayExpoValue = decayExponential;
        gainSniff = true;
        //StartCoroutine(MovementAverageCoroutine());
    }

    void Update()
    {
        float deltaSniff = 0;
        if (gainSniff && positionQueue.Count > 0) // Test mouse angle difference/delta
        {
            Vector3 convertedTransform = Camera.main.WorldToScreenPoint(transform.position);    // Convert camera to pixel coordinates
            deltaSniff = Vector3.Angle(Input.mousePosition - convertedTransform, positionQueue.Peek() - convertedTransform) * Time.deltaTime - decayConstant;     // Get angle between current mouse position and last mouse position, sub constDecay
            sniffStrength += Mathf.Clamp(deltaSniff, 0, sniffMax);       // apply sniff, (const cant reduce past 0)
        }
        decayExpoValue = Mathf.Clamp(deltaSniff <= 0 ? decayExpoValue * 2 : decayExponential, decayExponential, decayExponentialMax);   // Apply exponential decay, but clamp it to a max
        sniffStrength = Mathf.Clamp(sniffStrength - decayLinear*Time.deltaTime - decayExpoValue*Time.deltaTime, 0, sniffMax);
        
        // Get average direction of mouse 
        if (positionQueue.Count > queueSize)
            positionQueue.Dequeue();    // Keep queue at constant size

        positionQueue.Enqueue(Input.mousePosition);
        Vector3 averageVector = Camera.main.ScreenToWorldPoint(GetAverage());
        Debug.DrawLine(transform.position, averageVector, Color.red);
        //Debug.Log("Average Angle: " + Vector2.SignedAngle(Vector2.up, averageVector - transform.position));

        DrawSniffedObjects(Vector2.SignedAngle(Vector2.up, averageVector - transform.position));      // Now draw the sniffers
    }

    public void UpdateSprites(Dictionary<HiddenObject, SpriteRenderer> newDict)
    {
        spriteDict = newDict;
    }

    // Draw sniffed objects in a ring around gameobject
    private void DrawSniffedObjects(float sniffAngle)
    {
        if (spriteDict == null)
            return;

        Vector3 currentPos = transform.position;
        foreach(var (key, value) in spriteDict)
        {
            if (key == null)
                continue;
            Vector3 vect = key.transform.position - currentPos;
            float angle = Vector2.SignedAngle(Vector2.up, vect);

            // Check if we need to draw object
            // 1. angle within range; 2. within distance range; 
            if(Mathf.Abs(sniffAngle - angle) < angleRange && vect.magnitude < sniffRange)
            {
                Vector3 newV = currentPos + (vect.normalized * radius);
                newV.z = 0;
                value.transform.position = newV;
                
                value.color = new Color(value.color.r, value.color.g, value.color.b, Mathf.Lerp(0, 1, sniffStrength / sniffMax));   // Alpha scales to sniff strength
                // Scale scales to distance (0.5 - 0.01)scale, (1 - 100)
                value.transform.localScale = Vector3.one * Mathf.Lerp(0.01f, 0.40f, Mathf.InverseLerp(50f, 1f, vect.magnitude));
            }
            else
            {
                value.color = new Color(value.color.r, value.color.g, value.color.b, 0);
            }
        }
    }

    private Vector3 GetAverage()
    {
        Vector3 outp = Vector3.zero;
        foreach(Vector3 v in positionQueue)
        {
            outp += v;
        }

        return outp / positionQueue.Count;
    }

    IEnumerator MovementAverageCoroutine()
    {
        float sum = 0;
        Vector3 testo;
        bool p;
        for (int i=0; i < 120; i++)
        {
            p = positionQueue.TryPeek(out testo);
            yield return null;
            if (p)
            {
                Vector3 convertedTransform = Camera.main.WorldToScreenPoint(transform.position);
                sum += Vector3.Angle(Input.mousePosition - convertedTransform, testo - convertedTransform) * Time.deltaTime;
            }

        }
        Debug.Log("Average: " + sum / 120);
        StartCoroutine(MovementAverageCoroutine());
    }
}
