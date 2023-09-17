using Cysharp.Threading.Tasks;
using DG.Tweening;
using Pixelplacement;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CBGamejam.Global.UI
{
    public class GlobalUIManager : Singleton<GlobalUIManager>
    {
        public Image whiteImage;
        
        public void FadeIn(float time)
        {
            whiteImage.DOFade(1f, time).From(0f);
        }

        public void FadeOut(float time)
        {
            whiteImage.DOFade(0f, time).From(1f);
        }

        public async UniTask FadeInAsync(float time)
        {
            await whiteImage.DOFade(1f, time).From(0f).SetUpdate(true);
        }

        public async UniTask FadeOutAsync(float time)
        {
            await whiteImage.DOFade(0f, time).From(1f).SetUpdate(true);
        }


        [Button]
        public async UniTask FlashAsync()
        {
            await whiteImage.DOFade(1f, 0.2f).From(0f).SetUpdate(true);
            await UniTask.Delay(1000, true);
            await whiteImage.DOFade(0f, 3f).From(1f).SetUpdate(true);
        }

        public async UniTask FlashAsync(Action action)
        {
            await whiteImage.DOFade(1f, 0.2f).From(0f).SetUpdate(true);
            action.Invoke();
            await UniTask.Delay(1000, true);
            await whiteImage.DOFade(0f, 3f).From(1f).SetUpdate(true);
        }
    }

}
