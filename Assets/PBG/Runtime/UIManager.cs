using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PBG.Runtime
{
    public class UIManager : MonoBehaviour
    {
        public bool IsPause = false;

        public GameObject PlayerCamera;

        public GameObject PauseCanvas;

        public EventSystem eventSystem;

        public GameObject FirstButton;

        public GameObject FadeCanvas;
        public Text FadeText;

        public float FadeInDuration = 1f;
        public float DisplayDuration = 2.5f;

        private void Start()
        {
            PauseCanvas.SetActive(false);
            FadeCanvas.SetActive(false);
        }

        private void Update()
        {
            if ((Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) ||
                Input.GetKeyDown(KeyCode.Escape))
                TogglePause();
        }

        public void TogglePause()
        {
            IsPause = !IsPause;
            ProcessPause();
        }

        private void ProcessPause()
        {
            PauseCanvas.SetActive(IsPause);
            PlayerCamera.GetComponent<Blur>().enabled = !PlayerCamera.GetComponent<Blur>().enabled;
            Time.timeScale = IsPause ? 0f : 1f;
            if (IsPause)
                eventSystem.SetSelectedGameObject(FirstButton);
        }

        public void StartFadeTextInOut(string content)
        {
            FadeText.text = content;
            StartCoroutine(FadeTextInOut());
        }

        private IEnumerator FadeTextInOut()
        {
            FadeCanvas.SetActive(true);
            var canvasGroup = FadeCanvas.GetComponent<CanvasGroup>();

            var fadeInStartTime = Time.time;
            while (Time.time < fadeInStartTime + FadeInDuration)
            {
                var progress = (Time.time - fadeInStartTime) / FadeInDuration;
                canvasGroup.alpha = Mathf.Lerp(0, 1, progress);
                yield return null;
            }

            canvasGroup.alpha = 1;

            yield return new WaitForSeconds(DisplayDuration);

            var fadeOutStartTime = Time.time;
            while (Time.time < fadeOutStartTime + FadeInDuration)
            {
                var progress = (Time.time - fadeOutStartTime) / FadeInDuration;
                canvasGroup.alpha = Mathf.Lerp(1, 0, progress);
                yield return null;
            }

            canvasGroup.alpha = 0;
            FadeCanvas.SetActive(true);
        }
    }
}