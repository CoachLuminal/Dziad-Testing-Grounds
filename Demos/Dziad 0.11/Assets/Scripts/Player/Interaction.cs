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
    [SerializeField] private float rayCastLenght = 5f;

    [Header("Camera")]
    [SerializeField] private Camera playerCamera;

    [Header("MaskLayer")]
    [SerializeField] private LayerMask Layer;



    void Start()
    {
        playerCamera = Camera.main;
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

        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * rayCastLenght, Color.red, rayCastLenght);

        RaycastHit hit;
        

        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, rayCastLenght, Layer))
            {
                Debug.Log("Hit " + hit.transform.name);
                
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                PlayerInventory playerInventory = GetComponent<PlayerInventory>();
            if (interactable != null)
                {
                    interactable.Interact(null);
                    playerInventory.InventoryAddItem(interactable);

            }
            }

        

    }

}
