using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Rendering;

public class Interaction : MonoBehaviour
{
    [Header("Floaty")]
    [SerializeField] private float interactionRange = 5f;

    [Header("RayCast")]
    [SerializeField] private float rayCastLenght = 1f;

    private Camera playerCamera;

    

    void Start()
    {
        playerCamera = GetComponentInChildren<Camera>();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            InteractCasting();
        }

    }

    void InteractCasting()
    {

        RaycastHit hit;
        
        DebugInteractCasting(default);

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, rayCastLenght))
            {
                Debug.Log("Hit " + hit.transform.name);
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }

    }

    void DebugInteractCasting(RaycastHit hit)
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward, Color.blueViolet, rayCastLenght);
    }
}
