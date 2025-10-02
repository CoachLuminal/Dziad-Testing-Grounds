using UnityEngine;
using UnityEngine.Video;

public class Interactions : MonoBehaviour
{
    [Header("Strefa interakcji")]
    [SerializeField] private SphereCollider interactionSphere;
    [SerializeField] private float interactionRadius = 2f;

    [Header("Interakcja")]
    [SerializeField] private bool canInteract = false;
    [SerializeField] private GameObject interactableObject;

    private void Start()
    {
        if (interactionSphere == null)
        {
            // Pr�buj znale�� sfer� jako child obiektu
            interactionSphere = transform.Find("InteractionSphereRadius").GetComponent<SphereCollider>();
            if (interactionSphere == null)
            {
                Debug.LogError("Brak SphereCollider jako strefy interakcji!");
                return;
            }
        }

        interactionSphere.isTrigger = true;
        interactionSphere.radius = interactionRadius;
    }

    private void Update()
    {
        if (canInteract && Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    private void Interact()
    {
        if (interactableObject == null) return;

        Debug.Log("Nacisn��em E, pr�buj� u�y� obiektu: " + interactableObject.name);

        // Znajd� dziecko (lub kilka dzieci) i w��cz je
        Transform childToActivate = interactableObject.transform.Find("SM_BM86Monitor");
        if (childToActivate != null)
        {
            childToActivate.gameObject.SetActive(true);
            Debug.Log("W��czy�em dziecko: " + childToActivate.name);
        }
        else
        {
            Debug.LogWarning("Nie znalaz�em dziecka o nazwie 'SM_BM86Monitor'");
        }

        AudioSource audio = interactableObject.GetComponent<AudioSource>();
        if (audio != null)
        {
            Debug.Log("Znalaz�em AudioSource, odpalam d�wi�k.");
            audio.Play();
        }

        VideoPlayer video = interactableObject.GetComponentInChildren<VideoPlayer>();
        if (video != null)
        {
            Debug.Log("Znalaz�em VideoPlayer, odpalam video.");
            video.Play();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Interactable"))
        {
            interactableObject = other.gameObject;
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Interactable") && other.gameObject == interactableObject)
        {
            interactableObject = null;
            canInteract = false;
        }
    }

    // Publiczny dost�p do ustawiania promienia z innych skrypt�w
    public void SetInteractionRadius(float radius)
    {
        interactionRadius = radius;
        if (interactionSphere != null)
        {
            interactionSphere.radius = interactionRadius;
        }
    }

    public float GetInteractionRadius()
    {
        return interactionRadius;
    }
}
