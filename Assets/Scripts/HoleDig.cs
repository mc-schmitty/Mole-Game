using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleDig : MonoBehaviour
{
    public GameObject holePrefab;
    public float randomOffset = 2f;
    public float maxDigHeight = 18;

    
    private Dictionary<Vector2, Transform> HoleSet;
    private Vector2[] vList;

    // Start is called before the first frame update
    void Start()
    {
        HoleSet = new Dictionary<Vector2, Transform>();
        GenerateSquareList();
    }

    public bool DigHole(Vector3 location)
    {
        if (location.y >= maxDigHeight)
            return false;

        Vector2 modLoc = new Vector2(Mathf.Round(location.x), Mathf.Round(location.y));
        if (HoleSet.ContainsKey(modLoc))    // Don't stack holes
            return false;
        
        if(IsCrowded(modLoc, 4))        // If lots of holes already exist, don't fill it in and add a fake hole to simplify later checks
        {
            //HoleSet.Add(modLoc, null);
            return false;
        }

        var tobj = GameObject.Instantiate(holePrefab, location + new Vector3(Random.Range(-randomOffset, randomOffset), Random.Range(-randomOffset, randomOffset), 0), Quaternion.identity);
        tobj.name = modLoc.ToString();
        HoleSet.Add(modLoc, tobj.transform);
        return true;
    }

    private bool IsCrowded(Vector2 holeLoc, int maxSurrounding)
    {
        int neighbors = 0;

        for(int i = 0; i < 8; i++)
        {
            if(HoleSet.ContainsKey(holeLoc + vList[i]))
                neighbors++;
        }

        return neighbors > maxSurrounding;
    }

    private void GenerateSquareList()
    {
        // Screw it we're just testing so ill hardcode the check
        vList = new Vector2[8];
        vList[0] = Vector2.up;
        vList[1] = Vector2.left;
        vList[2] = Vector2.down;
        vList[3] = Vector2.right;
        vList[4] = Vector2.up + Vector2.left;
        vList[5] = Vector2.up + Vector2.right;
        vList[6] = Vector2.down + Vector2.left;
        vList[7] = Vector2.down + Vector2.right;
    }
}
