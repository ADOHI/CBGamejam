using CBGamejam.Ingame.Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBGamejam.Ingame.Objects
{
    public class Interactable : MonoBehaviour
    {
        protected Outline outline;
        [HideInInspector] public bool[] isFocuseds;

        protected virtual void Awake()
        {
            if (!TryGetComponent(out outline))
            {
                outline = gameObject.AddComponent<Outline>();
            }
            outline.OutlineWidth = 0f;
            isFocuseds = new bool[2];
        }

        private void Update()
        {
            if (isFocuseds[0] || isFocuseds[1])
            {
                outline.OutlineWidth = 5f;
            }
            else
            {
                outline.OutlineWidth = 0f;
            }

            var color = Color.black;

            if (isFocuseds[0] && isFocuseds[1])
            {
                color = (IngameManager.Instance.firstPlayerColor + IngameManager.Instance.secondPlayerColor) * 0.5f;
            }
            else if (isFocuseds[0])
            {
                color = IngameManager.Instance.firstPlayerColor;
            }
            else if (isFocuseds[1])
            {
                color = IngameManager.Instance.secondPlayerColor;
            }

            outline.OutlineColor = color;
        }

        public void OnEnterFocus(int playerIndex)
        {
            isFocuseds[playerIndex] = true;
        }

        public void OnExitFocus(int playerIndex)
        {
            isFocuseds[playerIndex] = false;
        }

    }

}
