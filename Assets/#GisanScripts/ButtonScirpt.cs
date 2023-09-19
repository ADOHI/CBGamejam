using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonScirpt : MonoBehaviour
{
    // button Audio
    public AudioSource buttonAudio;
    
    
    public Vector3 originalScale;



    public float increseScaleVal = 1.1f;
    private int delayTime = 1;

    // Start is called before the first frame update
    void Start()
    {
        originalScale = gameObject.transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeSize()
    {
        StartCoroutine(ChangeSize());
    }
    
    private IEnumerator ChangeSize()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        // 버튼 크기를 조금 키웁니다.
        Vector3 targetScale = currentScale * 1.1f; // 10% 크게
        float duration = 0.3f; // 크기 변경 지속 시간

        float elapsed = 0f;
        while (elapsed < duration)
        {
            gameObject.transform.localScale = Vector3.Lerp(currentScale, targetScale, elapsed / duration);
            elapsed += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        // 원래 크기로 돌아옵니다.
        gameObject.transform.localScale = originalScale;
    }
    
    
    public void Focused()
    {
        
        gameObject.transform.localScale = originalScale * increseScaleVal;
    }

    public void exitToFocus()
    {
        gameObject.transform.localScale = originalScale;
    }


    public void LoadStage()
    {
        buttonAudio.Play();
        Debug.Log("load Stage");
       

        StartCoroutine(LoadScene("IngameScene"));
    }

    public void LoadIntro()
    {
        buttonAudio.Play();

        StartCoroutine(LoadScene("IntroScene"));
    }

    public void ExitGame()
    {
        buttonAudio.Play();

        StartCoroutine(exitGame());
    }

    IEnumerator exitGame()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Application.Quit();
    }
    
    private IEnumerator LoadScene(string str)
    {
        yield return new WaitForSeconds(delayTime);
        
        SceneManager.LoadScene(str); 
    }
}
