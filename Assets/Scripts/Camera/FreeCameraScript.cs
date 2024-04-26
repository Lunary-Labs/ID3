using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeCameraScript : MonoBehaviour
{
    [SerializeField]private CinemachineFreeLook freeLookCamera;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ChangeCurrentTarget(Transform target){
        freeLookCamera.LookAt = target;
    }
}
