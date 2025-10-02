using UnityEngine;
using UnityEngine.VFX;

public class Interactable : MonoBehaviour
{
    public enum InteractableType
    {
        Weapon,
        Amunition,
        Magazine,
        Armor,
        Key,
        QuestItem,
        Miscellaneous,
        Book,
        Food,
    }
    [Header("Interactable Settings")]

    [SerializeField] private InteractableType interactableType;
    public void Interact()
    {
        Debug.Log("Interacted with " + gameObject.name);
        Destroy(gameObject);
    }
}
