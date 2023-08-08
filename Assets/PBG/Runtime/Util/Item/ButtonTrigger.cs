using System;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class ButtonTrigger : MonoBehaviour
    {
        public Action<bool> OnButtonTriggered;
        public Action<bool> OnButtonTriggeredEnterOnly;
        public Action<bool> OnButtonTriggeredExitOnly;

        private void OnTriggerEnter(Collider other)
        {
            OnButtonTriggered?.Invoke(true);
            OnButtonTriggeredEnterOnly?.Invoke(true);
        }

        private void OnTriggerExit(Collider other)
        {
            OnButtonTriggered?.Invoke(false);
            OnButtonTriggeredExitOnly?.Invoke(false);
        }
    }
}