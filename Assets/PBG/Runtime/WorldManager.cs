using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class WorldManager : MonoBehaviour
{
    public GameObject PlayerTorso;
    public GameObject GameWorld;
    public StartPoint CurStartPoint;
    public PostProcessVolume Volume;

    public List<InvisibleObject> InvisibleObjects;

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

        // Quit Game
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        CheckAndRebirth();
    }

    public void ToggleVolumeColorGrading()
    {
        ColorGrading colorGrading;
        if (Volume.profile.TryGetSettings(out colorGrading))
            colorGrading.enabled.value = !colorGrading.enabled.value;
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
            PlayerTorso.transform.position = CurStartPoint.transform.position;
    }
}