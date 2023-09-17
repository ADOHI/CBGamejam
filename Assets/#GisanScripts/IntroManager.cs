using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = System.Numerics.Vector2;

public class IntroManager : MonoBehaviour
{
    public Button startBtn;
    public Button exitBtn;
    public Camera SelectCam;

    public int focusButton = 0;

    public float camZPos = -50f;

    public AudioSource backGroundAudio;
    public AudioSource buttonFocusAudio;

    // Start is called before the first frame update
    void Start()
    {
        startBtn = GameObject.Find("StartButton").GetComponent<Button>();
        exitBtn = GameObject.Find("ExitButton").GetComponent<Button>();
        SelectCam = GameObject.Find("SelectCamera").GetComponent<Camera>();
        backGroundAudio.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            buttonFocusAudio.Play();
            focusButton += 1;
            focusButton %= 2;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (focusButton == 0)
            {
                startBtn.GetComponent<ButtonScirpt>().changeSize();
                startBtn.GetComponent<ButtonScirpt>().LoadStage();
            }
            else
            {
                
                exitBtn.GetComponent<ButtonScirpt>().changeSize();
                exitBtn.GetComponent<ButtonScirpt>().ExitGame();
            }
        }
        
        setButtonFocus();
        
        SetCamPosToFocusPos();
        
        
    }

    void setButtonFocus()
    {
        if (focusButton == 0)
        {
            startBtn.GetComponent<ButtonScirpt>().Focused();
            exitBtn.GetComponent<ButtonScirpt>().exitToFocus();
        }
        else
        {
            exitBtn.GetComponent<ButtonScirpt>().Focused();
            startBtn.GetComponent<ButtonScirpt>().exitToFocus();
        }
    }
    
    

    void SetCamPosToFocusPos()
    {

        Vector3 camPos;
        
        if (focusButton == 0)
        {
            camPos = startBtn.transform.position;
        }
        else
        {
            camPos = exitBtn.transform.position;
        }
        camPos.z = camZPos;

        SelectCam.transform.position = camPos;
    }
    
    

}
