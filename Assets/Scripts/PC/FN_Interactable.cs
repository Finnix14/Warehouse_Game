using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
public class FN_Interactable : MonoBehaviour
{

    [Header("Modular Interaction Events")]
    public UnityEvent onInteractEvent;

    public float fl_activation_distance = 5f;
    public float fl_resetTimer = 3f;
    private float fl_timerElapsed = 0f;

    public Transform tf_pc;

    public bool bl_isNear, bl_carrying;
    public bool bl_isAbleToCarry;

    [Header("Interaction State")]
    [SerializeField] private bool bl_hasInteracted;

    public GameObject go_Interact_Widget;
    private FN_Drag_n_Drop dragHandler;

    [Header("UI Prompt")]
    public string st_interactionPrompt = "Interact"; // Default prompt
    private TextMeshProUGUI txt_prompt;

    [Tooltip("Allow this object to be interacted with more than once.")]
    public bool bl_isReusable = false;


    void Start()
    {
        dragHandler = GetComponent<FN_Drag_n_Drop>();
        FN_InteractionManager.Instance?.Register(this);
        tf_pc = GameObject.Find("Collision & Physics")?.transform;
        txt_prompt = go_Interact_Widget.GetComponentInChildren<TextMeshProUGUI>();
        go_Interact_Widget.SetActive(false); // safety init
    }
    private void OnDestroy()
    {
        FN_InteractionManager.Instance?.Unregister(this);
    }

    void Update()
    {
        if (tf_pc == null) return;

        // time based reset after interaction
        if (bl_hasInteracted && bl_isReusable)
        {
            fl_timerElapsed += Time.deltaTime;
            if (fl_timerElapsed >= fl_resetTimer)
                ResetInteraction();
        }

        // distance only used by the manager
        bl_isNear = Vector3.Distance(transform.position, tf_pc.position) < fl_activation_distance;


    }

    private void HandleInteraction()
    {
        if (Vector3.Distance(transform.position, tf_pc.position) < fl_activation_distance)
        {
            bl_isNear = true;
            if (Input.GetKeyDown(KeyCode.E) && (!bl_hasInteracted || bl_isReusable))
            {
                bl_hasInteracted = true;
                fl_timerElapsed = 0f; // start reset timer
                onInteractEvent?.Invoke();
                FN_Sound_Manager.PlaySound(SoundType.Interact, 1f);
            }
        }
        else
            bl_isNear = false;
    }


    private void ResetInteraction()
    {
        if (!bl_isReusable) return;

        bl_hasInteracted = false;
        fl_timerElapsed = 0f;
    }


    //called by FN_InteractionManager if this is the closest object
    public void ShowWidget()
    {
        if (dragHandler != null && dragHandler.IsHeld()) return;

        if (!bl_hasInteracted || bl_isReusable)
        {
            go_Interact_Widget.SetActive(true);

            if (txt_prompt != null)
                txt_prompt.text = st_interactionPrompt;
        }

        if (Input.GetKeyDown(KeyCode.E) && (!bl_hasInteracted || bl_isReusable))
        {
            bl_hasInteracted = true;
            go_Interact_Widget.SetActive(false);
            fl_timerElapsed = 0f;
            onInteractEvent?.Invoke();
            FN_Sound_Manager.PlaySound(SoundType.Interact, 1f);
        }
    }

    public void HideWidget()
    {
        go_Interact_Widget.SetActive(false);
    }
    public bool IsActive()
    {
        if (dragHandler != null && dragHandler.IsHeld()) return false;

        // Reusable objects should always be active if the player is near
        return !bl_isAbleToCarry && bl_isNear && (!bl_hasInteracted || bl_isReusable);
    }

    public bool Interact(bool Interacted)
    {
        return bl_hasInteracted = true;
    }

    public string GetInteractionPrompt()
    {
        return st_interactionPrompt;
    }
}
