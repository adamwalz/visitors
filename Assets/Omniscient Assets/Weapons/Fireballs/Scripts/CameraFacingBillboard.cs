using UnityEngine;
using System.Collections;

public class CameraFacingBillboard : MonoBehaviour
{
    void LateUpdate()
    {
       transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.back,
            Camera.main.transform.rotation * Vector3.up);
    }
}