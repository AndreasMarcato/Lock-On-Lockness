using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAtPlayer : MonoBehaviour
{
    Transform _camera;
    private void Awake()
    {
        _camera = Camera.main.transform;
    }
    private void Update()
    {
        transform.rotation = _camera.rotation;
    }
}
