using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

namespace PBG.Runtime
{
    public class WorldManager : MonoBehaviour
    {
        public GameObject PlayerTorso;
        public GameObject GameWorld;
        public StartPoint CurStartPoint;
        public ThirdPersonCamera ThirdPersonCamera;
        public PostProcessVolume Volume;

        public List<InvisibleObject> InvisibleObjects;
        public UIManager uiManager;

        public float FallRoundDis = 10f;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
                Cursor.visible = !Cursor.visible;

            Cursor.lockState = uiManager.IsPause ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = uiManager.IsPause;

            CheckAndRebirth();
        }

        public void ToggleVolumeColorGrading()
        {
            ColorGrading colorGrading;
            if (Volume.profile.TryGetSettings(out colorGrading))
                colorGrading.enabled.value = !colorGrading.enabled.value;
        }

        public void ToggleVolumeDOF()
        {
            DepthOfField depthOfField;
            if (Volume.profile.TryGetSettings(out depthOfField))
                depthOfField.enabled.value = !depthOfField.enabled.value;
        }

        public void ToggleInvisibleObjectsVis()
        {
            foreach (var invisible in InvisibleObjects)
                invisible.Visibility = !invisible.Visibility;
        }

        private void CheckAndRebirth()
        {
            var playerY = PlayerTorso.transform.position.y;
            var worldY = GameWorld.transform.position.y;
            if (worldY - playerY > FallRoundDis)
                Rebirth();
        }

        public void Rebirth()
        {
            ThirdPersonCamera.Camera.transform.position = CurStartPoint.transform.position;
            PlayerTorso.transform.position = CurStartPoint.transform.position;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}