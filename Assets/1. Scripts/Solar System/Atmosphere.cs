using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atmosphere : MonoBehaviour
{
    MaterialPropertyBlock block;

    new Renderer renderer;
    private void Start()
    {
        renderer = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    // Update is called once per frame
    void Update()
    {
        block.SetVector("_LightDir", transform.position);
        renderer.SetPropertyBlock(block);
    }
}
