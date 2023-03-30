using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TempWinCon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TestVictory());
    }

    IEnumerator TestVictory()
    {
        yield return new WaitForSeconds(0.2f);
        int count = GetComponentsInChildren<Transform>().Length;
        if (count == 1)
        {
            Debug.Log("You Win!");
        }
        else
        {
            StartCoroutine(TestVictory());
            //Debug.Log(count);
        }
            
    }
}
