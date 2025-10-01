using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            // 方式1：始终正面朝向摄像机
            transform.forward = mainCamera.transform.forward;

            // 方式2（更稳妥）：完全朝向摄像机
            // transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward,
            //                  mainCamera.transform.rotation * Vector3.up);
        }
    }
}

