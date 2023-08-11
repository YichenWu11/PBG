using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject PlayerTorso;
    public GameObject GameWorld;
    public StartPoint CurStartPoint;

    public float FallRoundDis = 10f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
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

    private void CheckAndRebirth()
    {
        var playerY = PlayerTorso.transform.position.y;
        var worldY = GameWorld.transform.position.y;
        if (worldY - playerY > FallRoundDis)
            PlayerTorso.transform.position = CurStartPoint.transform.position;
    }
}