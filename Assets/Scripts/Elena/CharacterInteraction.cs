using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterInteraction : MonoBehaviour
{
    //Module Detection
    private LayerMask moduleLayer; 
    [SerializeField] private float detectionRadius = 200f; // Rayon de détection autour de la caméra
    [SerializeField] private float fieldOfViewAngle = 90f;

    //Module Variable
    private Transform currentModule;

    [SerializeField]private FreeCameraScript freeCamera;
    
    // Start is called before the first frame update
    void Start()
    {
        //Input declaration
        moduleLayer = LayerMask.GetMask("Module");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTarget()
    {
        Debug.Log("Module search");
        currentModule = FindClosestModuleInVision();
        if (currentModule != null)
        {
            freeCamera.ChangeCurrentTarget(currentModule);
            Debug.Log("Module trouvé");
        }
    }

    public Transform FindClosestModuleInVision() 
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, moduleLayer);
        Transform closestModule = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 visionDirection = transform.forward;

        foreach (Collider hit in hits)
        {
            Vector3 directionToTarget = hit.transform.position - transform.position;
            float angleToTarget = Vector3.Angle(visionDirection, directionToTarget);
            float dSqrToTarget = directionToTarget.sqrMagnitude;

            // Vérifie si l'ennemi est dans le champ de vision et est le plus proche jusqu'à présent
            if (angleToTarget < fieldOfViewAngle / 2 && dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;  
                closestModule = hit.transform;
            }
        }
        return closestModule;
    }
}
