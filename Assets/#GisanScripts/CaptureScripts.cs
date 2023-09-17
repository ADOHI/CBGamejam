using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaptureScripts : MonoBehaviour
{
    
    // capture UI
    
    public AudioSource cameraAudio;
    public GameObject photoBoarder;
    public GameObject captureImg;
    public GameObject flashImg;
    
    // for ending
    public GameObject endingUI;
    public EndingScript endingScript;

    public int score;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartCapture()
    {
        cameraAudio.Play();

        StartCoroutine(captureUICorutine());
    }
    
    IEnumerator captureUICorutine()
    {
        flashImg.SetActive(true);
        
        while (flashImg.GetComponent<Image>().color.a > 0f)
        {
            flashImg.GetComponent<Image>().color -= new Color(0f, 0f, 0f, 0.05f);
            
            yield return new WaitForSecondsRealtime(0.01f);
        }

        yield return new WaitForSecondsRealtime(1f);
        
        endingUI.SetActive(true);
        endingScript.StartEnding();
        endingScript.score = this.score;
        
        while (photoBoarder.GetComponent<Image>().color.a > 0.5f)
        {
            photoBoarder.GetComponent<Image>().color -= new Color(0f, 0f, 0f, 0.05f);
            captureImg.GetComponent<RawImage>().color -= new Color(0f, 0f, 0f, 0.05f);
            
            yield return new WaitForSecondsRealtime(0.02f);
        }
        
        
    }
}
