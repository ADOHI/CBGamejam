using CBGamejam.Global.UI;
using CBGamejam.Ingame.Characters;
using Cysharp.Threading.Tasks;
using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CBGamejam.Ingame.Manager
{
    public class IngameManager : Singleton<IngameManager>
    {
        private bool isPlaying;

        [Header("Times")]
        public float maxPlaytime;
        public float remainPlayTime;

        [Header("Characters")]
        public CustomCharacterContoller firstCharacter;
        public CustomCharacterContoller secondCharacter;
        [Header("MainItem")]
        public GameObject crafts;
        public bool isCraftsInLighting;

        [Header("Camera")]
        public Camera ingameCamera;
        public Camera zoomInCamera;
        public float FullCamStartTime = 5f;
        public float finalTimeScale = 0.2f;
        [Header("UIs")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI counterText;
        public GameObject cameraRTImageChunk;
        public GameObject fullCameraCrossHair;
        //public TextMeshProUGUI ;

        [Header("Missons")]
        public List<string> missions;
        public List<TextMeshProUGUI> missionUIs;

        [Header("Config")]
        public AudioClip countDownBgm;
        public AudioClip ingameBgm;
        public AudioClip shutterSfx;
        public Color firstPlayerColor;
        public Color secondPlayerColor;

        private void Awake()
        {
            remainPlayTime = maxPlaytime;
        }

        private void Start()
        {
            StartGameAsync().AttachExternalCancellation(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private void Update()
        {
            if (isPlaying)
            {
                remainPlayTime = Mathf.Clamp(remainPlayTime -= Time.deltaTime, 0f, maxPlaytime);
                timerText.text = TimeSpan.FromSeconds(remainPlayTime).ToString(@"m\:ss");
            }

         }


        private async UniTask StartGameAsync()
        {
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            await GlobalUIManager.Instance.FadeOutAsync(3f);
            SoundManager.PlayMusic(countDownBgm);
            await PlayTimerAsync((int)FullCamStartTime, 2f, "½ÃÀÛ!");

            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            isPlaying = true;

            SoundManager.PlayMusic(ingameBgm, loop:true);

            await UniTask.WaitUntil(() => remainPlayTime < FullCamStartTime + 1.5f);

            ShowFullCameraView();
            PlayTimerAsync((int)FullCamStartTime, 1f, "±èÄ¡!", ignoreTimeScale: false).Forget();

            await UniTask.WaitUntil(() => remainPlayTime == 0f);
            SoundManager.StopAll();
            Scoreing();
            PressShutter();
        }

        private async UniTask PlayTimerAsync(int second, float lastTime = 0f, string lastString = "", bool ignoreTimeScale = true)
        {
            for (int i = second; i > 0; i--)
            {
                counterText.text = i.ToString();
                await UniTask.Delay(1000, ignoreTimeScale);
            }
            counterText.text = lastString;
            await UniTask.Delay((int)(lastTime * 1000f), ignoreTimeScale);
            counterText.text = "";
        }

        private void ShowFullCameraView()
        {
            Time.timeScale = finalTimeScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            timerText.gameObject.SetActive(false);
            //cameraRTImageChunk.SetActive(false);
            cameraRTImageChunk.transform.localScale = new Vector3(4.55f, 4.35f, 0f);
            var outlines = FindObjectsByType<Outline>(FindObjectsSortMode.None);
            foreach (var outline in outlines)
            {
                outline.enabled = false;
            }
            //fullCameraCrossHair.SetActive(true);
            //zoomInCamera.gameObject.SetActive(true);
            //ingameCamera.gameObject.SetActive(false);
        }

        private void PressShutter()
        {
            Time.timeScale = 0f;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            SoundManager.PlayFx(shutterSfx, volume:5f);
            fullCameraCrossHair.SetActive(false);
            GlobalUIManager.Instance.FlashAsync().Forget();
        }


        private void Calculate()
        {

        }

        [Button]
        public (bool isInCamera, float distance) IsInCamera(Camera camera, GameObject gameObject)
        {
            var screenPosition = camera.WorldToScreenPoint(gameObject.transform.position);
            var isInCamera = 
                screenPosition.x > 0f && 
                screenPosition.x < Screen.width &&
                screenPosition.y > 0f &&
                screenPosition.y < Screen.height &&
                screenPosition.z >= 0f;

            Debug.Log(screenPosition);
            return (isInCamera, screenPosition.z);
        }

        public void Scoreing()
        {
            //var isCraftIn = IsInCamera(zoomInCamera, crafts);
            //var isCraftLighting = IsInCamera(zoomInCamera, crafts);

            var isFirstCharacterIn = IsInCamera(zoomInCamera, firstCharacter.gameObject);
            var isSecondCharacterIn = IsInCamera(zoomInCamera, secondCharacter.gameObject);

            var isFirstCharacterDance = isFirstCharacterIn.isInCamera && firstCharacter.isDancing;
            var isSecondCharacterDance = isSecondCharacterIn.isInCamera && secondCharacter.isDancing;

            Debug.LogWarning((isFirstCharacterIn.isInCamera, isSecondCharacterIn.isInCamera, isFirstCharacterDance, isSecondCharacterDance));
        }
    }

}
