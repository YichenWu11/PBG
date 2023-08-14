using System;
using UnityEngine;

namespace PBG.Runtime.Util
{
    public class ButtonTrigger : MonoBehaviour
    {
        public Action<bool> OnButtonTriggered;
        public Action<bool> OnButtonTriggeredEnterOnly;
        public Action<bool> OnButtonTriggeredExitOnly;

        public bool IsDown { get; set; } = false;

        [SerializeField] private GameObject m_ButtonBody;
        private Material m_ButtonBodyMat;

        private float m_MaxIntensity = 20f;
        private float m_MinIntensity = 1f;

        private void Start()
        {
            if (m_ButtonBody)
            {
                m_ButtonBodyMat = m_ButtonBody.GetComponent<Renderer>().material;
                m_ButtonBodyMat.SetFloat("_EmissiveIntensity", m_MinIntensity);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            IsDown = true;
            OnButtonTriggered?.Invoke(true);
            OnButtonTriggeredEnterOnly?.Invoke(true);
            m_ButtonBodyMat.SetFloat("_EmissiveIntensity", m_MaxIntensity);
        }

        private void OnTriggerExit(Collider other)
        {
            IsDown = false;
            OnButtonTriggered?.Invoke(false);
            OnButtonTriggeredExitOnly?.Invoke(false);
            m_ButtonBodyMat.SetFloat("_EmissiveIntensity", m_MinIntensity);
        }
    }
}