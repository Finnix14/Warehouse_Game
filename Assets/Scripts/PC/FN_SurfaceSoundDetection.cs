using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class FN_SurfaceSoundDetection : MonoBehaviour
{
    [SerializeField] private Transform tr_groundCheck;
    [SerializeField] private float fl_rayDistance = 1.5f;

    public static event Action<SurfaceType> OnFootstep; // event for the footsteps


    public void DetectSurface()
    {
        RaycastHit hit;
        if (Physics.Raycast(tr_groundCheck.position, Vector3.down, out hit, fl_rayDistance))
        {
            SurfaceType surfaceType;

            switch (hit.collider.tag) // switch statement that assigns surface types to strings
            {
                case "Grass":
                    surfaceType = SurfaceType.Grass;
                    break;
                case "Wood":
                    surfaceType = SurfaceType.Wood;
                    break;
                case "Tile":
                    surfaceType = SurfaceType.Tile;
                    break;
                case "Carpet":
                    surfaceType = SurfaceType.Carpet;
                    break;
                default:
                    surfaceType = SurfaceType.Grass; // default type for surfaces
                    break;
            }

            OnFootstep?.Invoke(surfaceType); // trigger footstep event with detected surface
        }
    }
}