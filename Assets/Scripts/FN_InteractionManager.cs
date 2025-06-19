using System.Collections.Generic;
using UnityEngine;

public class FN_InteractionManager : MonoBehaviour
{
    public static FN_InteractionManager Instance;

    private List<FN_Interactable> interactables = new();
    private FN_Interactable currentClosest;

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void Register(FN_Interactable interactable)
    {
        if (!interactables.Contains(interactable))
            interactables.Add(interactable);
    }

    public void Unregister(FN_Interactable interactable)
    {
        if (interactables.Contains(interactable))
            interactables.Remove(interactable);
    }

    private void Update()
    {
        if (interactables.Count == 0) return;

        float closestDist = float.MaxValue;
        FN_Interactable closest = null;

        foreach (var interactable in interactables)
        {
            if (interactable == null || !interactable.IsActive()) continue;

            float dist = Vector3.Distance(interactable.transform.position, interactable.tf_pc.position);
            if (dist < interactable.fl_activation_distance && dist < closestDist)
            {
                closestDist = dist;
                closest = interactable;
            }
        }

        // Hide previous widget
        if (currentClosest != null && currentClosest != closest)
            currentClosest.HideWidget();

        currentClosest = closest;

        if (currentClosest != null)
            currentClosest.ShowWidget();
    }
}
