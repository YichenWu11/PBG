using UnityEditor;
using UnityEngine;
using PBG.Runtime.Util;

[CustomEditor(typeof(MovePlayerHelper))]
public class MovePlayerHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var movePlayerHelper = (MovePlayerHelper)target;

        if (GUILayout.Button("Move"))
            movePlayerHelper.Move();
    }
}