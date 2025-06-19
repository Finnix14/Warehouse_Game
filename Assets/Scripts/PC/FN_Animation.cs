using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FN_Animation : MonoBehaviour
{
    private FN_SurfaceSoundDetection surfaceDetector;
    private Vector3 lastFootstepPos;
    [SerializeField] private float stepDistance = 2f;
    void Start()
    {
        surfaceDetector = GetComponent<FN_SurfaceSoundDetection>();
    }
    private void Update()
    {
    
        float distance = Vector3.Distance(transform.position, lastFootstepPos);
        if (distance > stepDistance)
        {
            surfaceDetector.DetectSurface();
            lastFootstepPos = transform.position;
        }
    }

}
