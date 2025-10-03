using UnityEngine;

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

    [SerializeField] public string itemName;
    [SerializeField] public int itemWeight;
    [SerializeField] public string itemDescription;
    [SerializeField] public Sprite itemIcon;
    [SerializeField] public int itemID;

    public void Interact(PlayerInventory inventory)
    {
        Debug.Log("Interacted with " + gameObject.name);
        Destroy(gameObject);
    }
}
