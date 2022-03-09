using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInventory : MonoBehaviour
{
    public ArrayList Items;
    [Tooltip("How many items can a player carry?")]
    [SerializeField] private int inventoryCapacity = 1;

    void Start()
    {
        Items = new ArrayList();
    }

    public void attachItem (string itemName)
    {
        if (Items.Count < inventoryCapacity)
        Items.Add(itemName);
    }

    public float useItem(string itemName)
    {
        switch (itemName) {
            case "Item_Health":
               // this.GetComponent<playerControl>().addHealth(0.2f);
                Items.Remove(itemName);
                return 0.2f;

            default:
                Debug.Log("Couldn't find item with name: " + itemName);
                return 0f;
                }
    }
}
