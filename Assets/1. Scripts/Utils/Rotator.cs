using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
#pragma warning disable CS0649

    [SerializeField] private float speed;
    [SerializeField] private Vector3 axis = Vector3.up;
    [SerializeField] private bool useLocalSpace = true, rotateOnVisibleOnly = true;
    [SerializeField] private Transform[] objects;
    private bool isVisible = true;


    private void Start()
    {
        isVisible = true;
    }

    private void OnBecameVisible()
    {
        isVisible = true;
    }

    private void OnBecameInvisible()
    {
        isVisible = false;
    }

    void Update()
    {
        if (!isVisible && rotateOnVisibleOnly) return;

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].Rotate(axis, Time.deltaTime * speed, useLocalSpace ? Space.Self : Space.World);
        }
    }
}
