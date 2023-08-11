using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecycleObject : MonoBehaviour
{
    private Vector3 m_RestartPoint;
    private Rigidbody m_Rb;

    // Start is called before the first frame update
    private void Start()
    {
        m_RestartPoint = new Vector3(
            transform.position.x,
            transform.position.y + 1.5f,
            transform.position.z);
        m_Rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var curY = transform.position.y;
        if (m_RestartPoint.y - curY > 10f)
        {
            m_Rb.velocity = Vector3.zero;
            transform.position = m_RestartPoint;
        }
    }
}