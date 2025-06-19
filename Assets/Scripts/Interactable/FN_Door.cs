using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FN_Door_Interaction : MonoBehaviour
{
    [Header("Door Variables")]
    private bool bl_isOpen = false;
    public float fl_rotationSpeed = 2f;
    public Transform tr_door;
    private bool bl_isRotating = false;
    private bool bl_hasPlayedSound = false;

    [Header("Key Checks")]
    [SerializeField] private bool bl_requiresKey = false;
    [SerializeField] private bool bl_hasKey;
    public void SetKeyState(bool hasIt) => bl_hasKey = hasIt;

    //target rotations
    private Quaternion qt_doorClosedRotation;
    private Quaternion qt_doorOpenRotation;

    [Header("Required Item")]
    [SerializeField] private string st_requiredItem = "Key Card"; // shown in Inspector


    private void Start()
    {
        qt_doorClosedRotation = tr_door.rotation; // initial rotation
        qt_doorOpenRotation = Quaternion.Euler(0f, 90f, 0f) * qt_doorClosedRotation; // open rotation
    }
    public void ToggleDoor()
    {
        if (!bl_isRotating)
        {
            Quaternion targetRotation = bl_isOpen ? qt_doorClosedRotation : qt_doorOpenRotation; //if is open, do close, if not, do open
            StartCoroutine(RotateDoor(targetRotation));
        }
    }

    public void TryToggleDoor()
    {

        if (bl_requiresKey)
        {
            FN_InventoryManager inventory = FindObjectOfType<FN_InventoryManager>();
            if (inventory == null || !inventory.HasItem(st_requiredItem))
            {
                Debug.LogWarning($"[FN_Door] Access denied: Requires '{st_requiredItem}'.");
                FN_Sound_Manager.PlaySound(SoundType.Locked, 1f);
                return; 
            }
        }

        ToggleDoor();
    }

    public void OpenDoor()
    {
        if (!bl_isOpen && !bl_isRotating)
            StartCoroutine(RotateDoor(qt_doorOpenRotation));
    }

    public void CloseDoor()
    {
        if (bl_isOpen && !bl_isRotating)
            StartCoroutine(RotateDoor(qt_doorClosedRotation));
    }

    private IEnumerator RotateDoor(Quaternion targetRotation)
    {
        bl_isRotating = true;

        // play sound once when door starts moving
        if (!bl_hasPlayedSound)
        {
            FN_Sound_Manager.PlaySound(SoundType.Door, 0.3f);
            bl_hasPlayedSound = true;
        }

        while (Quaternion.Angle(tr_door.rotation, targetRotation) > 0.1f)
        {
            tr_door.rotation = Quaternion.Slerp(
                tr_door.rotation,
                targetRotation,
                fl_rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        tr_door.rotation = targetRotation;
        bl_hasPlayedSound = false; // reset sound flag
        bl_isOpen = (targetRotation == qt_doorOpenRotation);
        bl_isRotating = false;
    }

 
}
