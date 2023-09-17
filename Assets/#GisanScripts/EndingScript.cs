using System.Collections;
using System.Collections.Generic;
using ChatGPTWrapper;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EndingScript : MonoBehaviour
{
    
    // Ending UI
   
    public string RequestMessage;
    public ChatGPTConversation[] chatGptConversation;
    
    public Button goToIntroBtn;
    public GameObject heartParticle;
    public GameObject badParticle;
    
    public float score;

    public AudioSource keyBoardAudio;
    
    public GameObject flashImg;
    public int textEndCount = 0;
    
    //---------------------// 디버그용!
    
    
    // Start is called before the first frame update
    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            goToIntroBtn.GetComponent<ButtonScirpt>().changeSize();
            goToIntroBtn.GetComponent<ButtonScirpt>().LoadIntro();
        }

        if (textEndCount == 2)
        {
            keyBoardAudio.Stop();
        }
    }

    public void setScore(int score)
    {
        this.score = score;
    }

    void GenerateGPT()
    {
        if (score >= 4)
        {
            RequestMessage = "고양이들이 인스타 감성적인 사진을 너무 잘찍었는데 이에 따른 반응 댓글을 작성해줘 댓글 하나만";
            
            sendGpt();
            
            
            heartParticle.SetActive(true);
            
        }
        else
        {
            RequestMessage = "고양이들이 인스타 감성적인 사진을 너무 못찍어서 이에 따른 안좋은 반응 댓글을 작성해줘 댓글 하나만";

            sendGpt();
            
            badParticle.SetActive(true);
        }
    }

    
    
    
    
    public void StartEnding()
    {
        gameObject.SetActive(true);
        StartCoroutine(endingCorutine());
    }

    IEnumerator endingCorutine()
    {
        flashImg.SetActive(true);
        
        // 플레시 동작
        while (flashImg.GetComponent<Image>().color.a > 0f)
        {
            flashImg.GetComponent<Image>().color -= new Color(0f, 0f, 0f, 0.05f);
            
            yield return new WaitForSecondsRealtime(0.01f);
        }

        textEndCount = 0;
        
        
        keyBoardAudio.Play();
        
        GenerateGPT();
        
        
    }
    
    IEnumerator sendGpt()
    {
        for (int i = 0; i < 2; i++)
        {
            yield return new WaitForSecondsRealtime(1f);
            chatGptConversation[i].SendToChatGPT(RequestMessage);
        }
    }

    
}
