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
        public List<StartPoint> StartPoints;
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

        public void ToggleVolumeVignette()
        {
            Vignette vignette;
            if (Volume.profile.TryGetSettings(out vignette))
                vignette.enabled.value = !vignette.enabled.value;
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

        public void Cheat()
        {
            // 把 StartPoint 设置成下一个, 并移动人物
            var curIdx = 0;
            var nxtIdx = 0;
            for (var i = 0; i < StartPoints.Count; ++i)
                if (CurStartPoint.name == StartPoints[i].name)
                    curIdx = i;
            nxtIdx = (curIdx + 1) % StartPoints.Count;

            CurStartPoint = StartPoints[nxtIdx];
            Rebirth();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}