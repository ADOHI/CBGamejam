using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

namespace CBGamejam.Ingame.Characters.FXs
{
    public class LetterEffectSystem : MonoBehaviour
    {
        private CancellationTokenSource tokenSource;
        public Transform characterTransform;
        public Vector3 offset;
        public Camera ingameCamera;
        public TextMeshProUGUI letterFX;

        public List<string> presets = new List<string>();
        public float defaultInterval = 0.2f;
        public float defaultRemainDuration = 1f;

        private void Awake()
        {
            if (ingameCamera == null)
            {
                ingameCamera = Camera.main;
            }
        }

        private void Start()
        {
            letterFX.text = "";
        }

        private void FixedUpdate()
        {
            letterFX.transform.position = ingameCamera.WorldToScreenPoint(characterTransform.position + offset);
        }

        [Button]
        public void ShowTextFX(int index)
        {
            Debug.Log("Test " + index);
            if (index < 0 || index >= presets.Count)
            {
                return;
            }
            var text = presets[index];
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            tokenSource = new CancellationTokenSource();
            ShowTextFXAsync(text, defaultInterval, defaultRemainDuration).AttachExternalCancellation(tokenSource.Token);
        }


        [Button]
        public void ShowTextFX(string text, float timeInterval, float remainDuration)
        {
            if (tokenSource != null)
            {
                tokenSource.Cancel();
            }
            tokenSource = new CancellationTokenSource();
            ShowTextFXAsync(text, timeInterval, remainDuration).AttachExternalCancellation(tokenSource.Token);
        }


        public async UniTask ShowTextFXAsync(string text, float timeInterval, float remainDuration)
        {
            for (int i = 0; i < text.Length; i++)
            {
                letterFX.text = text.Substring(0, i + 1);
                await UniTask.Delay((int)(timeInterval * 1000f)).AttachExternalCancellation(tokenSource.Token);
            }
            await UniTask.Delay((int)(remainDuration * 1000f)).AttachExternalCancellation(tokenSource.Token);
            letterFX.text = "";
        }

    }

}
