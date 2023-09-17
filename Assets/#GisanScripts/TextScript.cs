using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextScript : MonoBehaviour
{
    public string respone;

    public int size;
    // Start is called before the first frame update
    public TMP_Text text;

    public GameObject endUI;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setText(string str)
    {
        respone = str;
        size = respone.Length;
        StartCoroutine(responeWrite());
    }

    IEnumerator responeWrite()
    {
        text.text = "";
        for (int i = 0; i < size; i++)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            text.text += respone[i];
        }

        endUI.GetComponent<EndingScript>().textEndCount++;
    }
}
