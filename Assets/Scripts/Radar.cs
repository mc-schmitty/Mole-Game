using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Radar : MonoBehaviour
{
    [SerializeField]
    private SniffDrawer sniffer;

    private List<HiddenObject> TrackedObjects;
    private Dictionary<HiddenObject, SpriteRenderer> ObjectData;

    private void Start()
    {
        TrackedObjects = new List<HiddenObject>();
        ObjectData = new Dictionary<HiddenObject, SpriteRenderer>();
        foreach(HiddenObject hidd in FindObjectsOfType<HiddenObject>())
        {
            TrackedObjects.Add(hidd);
            ObjectData.Add(hidd, Instantiate(hidd.scentSprite).GetComponent<SpriteRenderer>());
        }
        Debug.Log("Scanning for: "+TrackedObjects.Count);

        StartCoroutine(DoScan());
    }

    public (int revealed, int hidden) Scan()       // Searches for nearby hidden objects
    {
        Vector2 pos = transform.position;
        int revealed = 0;
        int hidden = 0;

        HiddenObject obj;
        for(int i = 0; i < TrackedObjects.Count; i++)
        {
            obj = TrackedObjects[i];
            if(obj == null)
            {
                TrackedObjects.RemoveAt(i);
                Destroy(ObjectData[obj]);
                ObjectData.Remove(obj);
                i--;
                continue;
            }

            float distance = Vector2.Distance(obj.transform.position, pos);
            if (obj.IsHidden && distance <= obj.revealRange)     // Test for objects in range
            {
                Debug.Log("Revealed " + obj.name);
                obj.Reveal();
                revealed++;
            }
            else if (!obj.IsHidden && distance >= obj.hideRange)
            {
                Debug.Log("Hidden " + obj.name);
                obj.Hide();
                hidden++;
            }
            
        }

        sniffer.UpdateSprites(ObjectData);
        //Debug.Log("List: " + TrackedObjects.Count + ", Dict: " + ObjectData.Count);
        return (revealed, hidden);
    }

    IEnumerator DoScan()
    {
        yield return new WaitForSeconds(0.1f);
        Scan();
        StartCoroutine(DoScan());
    }
}
