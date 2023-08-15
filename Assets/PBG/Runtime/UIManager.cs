using System.Collections;
using System.Collections.Generic;
using PBG.Runtime;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    public bool IsPause = false;

    public GameObject PlayerCamera;

    public GameObject PauseCanvas;

    // Start is called before the first frame update
    private void Start()
    {
        PauseCanvas.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if ((Gamepad.current != null && Gamepad.current.startButton.wasPressedThisFrame) ||
            Input.GetKeyDown(KeyCode.P))
        {
            IsPause = !IsPause;
            ProcessPause();
        }
    }

    private void ProcessPause()
    {
        PauseCanvas.SetActive(IsPause);
        PlayerCamera.GetComponent<Blur>().enabled = !PlayerCamera.GetComponent<Blur>().enabled;
        Time.timeScale = IsPause ? 0f : 1f;
    }
}