using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]private Transform FreeCamera; 
    [SerializeField]private Transform TargetCamera; 
    [SerializeField]private InputActionAsset actionAsset;
    private InputAction targetAction;
    // Start is called before the first frame update
    private CamStates camState = CamStates.Free;
    public enum CamStates{
        Free, 
        Target
    }

    void Start()
    {
        targetAction = actionAsset.FindAction("Target");
        targetAction.canceled += context => OnTargetCanceled();
        targetAction.Enable();
    }
    // Update is called once per frame
    void Update()
    {
        switch(camState){
            case CamStates.Free:
                FreeCamera.gameObject.SetActive(true);
                TargetCamera.gameObject.SetActive(false);
                break;
            case CamStates.Target:
                FreeCamera.gameObject.SetActive(false);
                TargetCamera.gameObject.SetActive(true);
                break;
        }
    }

    void OnTarget(){
        Debug.Log("Targeting");
        camState = CamStates.Target;
    }

    void OnTargetCanceled(){
        camState = CamStates.Free;
    }
}
