using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlopBoundary : MonoBehaviour
{
    private float beamOffset;

    public Vector3 BeamPosition { get { return transform.position + (transform.up * beamOffset); } }
    public Vector3 Position { get { return transform.position; } }

    private void Awake()
    {
        beamOffset = GetComponent<Renderer>().bounds.max.y;
        beamOffset = beamOffset - (beamOffset * 0.1f);
    }
}
