using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PBG.Runtime.Util
{
    [ExecuteInEditMode]
    public class MovePlayerHelper : MonoBehaviour
    {
        public StartPoint SelectedStartPoint;
        public List<StartPoint> StartPoints;
        public Transform Player;

        public void Move()
        {
            if (Player != null && SelectedStartPoint != null)
                Player.transform.position = SelectedStartPoint.transform.position;
        }
    }
}