using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class zzz : MonoBehaviour
{
    public CaptureScripts captureScripts;
    public GameObject captureUI;
    


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) ){
            
            // 이거를 게임 끝나면 추가해야함 에디터에서 연결하고
            
            
            captureUI.SetActive(true);
            captureScripts.score = 5;
            captureScripts.StartCapture();
            
            
        }   
    }
    
    
}
