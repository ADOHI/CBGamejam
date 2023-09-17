using Cysharp.Threading.Tasks;
using Pixelplacement;
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
        public float maxPlaytime;
        public float remainPlayTime;

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

        [Header("Config")]
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
                remainPlayTime = Mathf.Clamp(remainPlayTime -= Time.unscaledDeltaTime, 0f, maxPlaytime);
                timerText.text = TimeSpan.FromSeconds(remainPlayTime).ToString(@"m\:ss");
            }
         }


        private async UniTask StartGameAsync()
        {
            isPlaying = true;

            await UniTask.WaitUntil(() => remainPlayTime < FullCamStartTime);

            PlayTimerAsync((int)FullCamStartTime).Forget();
            ShowFullCameraView();

            await UniTask.WaitUntil(() => remainPlayTime == 0f);
            PressShutter();
        }

        private async UniTask PlayTimerAsync(int second)
        {
            for (int i = second; i > 0; i--)
            {
                counterText.text = i.ToString();
                await UniTask.Delay(1000, true);
            }
        }

        private void ShowFullCameraView()
        {
            Time.timeScale = finalTimeScale;
            timerText.gameObject.SetActive(false);
            //cameraRTImageChunk.SetActive(false);
            cameraRTImageChunk.transform.localScale = new Vector3(4.55f, 4.35f, 0f);
            //fullCameraCrossHair.SetActive(true);
            //zoomInCamera.gameObject.SetActive(true);
            //ingameCamera.gameObject.SetActive(false);
        }

        private void PressShutter()
        {
            Time.timeScale = 0f;
        }
    }

}
