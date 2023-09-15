using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CBGamejam.Ingame.Objects
{
    public class Grabbable : MonoBehaviour
    {
        private Outline outline;
        private Rigidbody rigidbody;
        public bool isFocused;

        private void Awake()
        {
            if (!TryGetComponent(out outline))
            {
                outline = gameObject.AddComponent<Outline>();
            }
            outline.OutlineWidth = 0f;

            if (!TryGetComponent(out rigidbody))
            {
                rigidbody = gameObject.AddComponent<Rigidbody>();
            }
        }

        public void OnEnterInteraction()
        {
            outline.OutlineWidth = 5f;
            isFocused = true;
        }

        public void OnExitInteraction()
        {
            outline.OutlineWidth = 0f;
            isFocused = false;
        }

        public void OnStartGrab()
        {

        }

        public void OnExitGrab()
        {

        }
    }

}
