using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInventory : MonoBehaviour
{
    public ArrayList Items;

    void Start()
    {
        Items = new ArrayList();
    }

    public void attachItem (string itemName)
    {
        Items.Add(itemName);
        Debug.Log(Items.Count);
    }

    public void useItem(string itemName)
    {
        switch (itemName) {
            case "Item_Health":
               // this.GetComponent<playerControl>().addHealth(0.2f);
                Items.Remove(itemName);
                break;

            default:
                Debug.Log("Couldn't find item with name: " + itemName);
                break;
                }
    }
}
