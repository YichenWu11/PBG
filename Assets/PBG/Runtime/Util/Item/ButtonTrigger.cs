using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class ButtonTrigger : MonoBehaviour
    {
        public Action<Collider> OnButtonTriggered;

        private void OnTriggerEnter(Collider other)
        {
            OnButtonTriggered?.Invoke(other);
            Debug.Log("Button Enter");
        }
    }
}