using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerInventory : MonoBehaviour
{
    private string currentItem;

    public void attachItem (string itemName)
    {
        currentItem = itemName;
    }

    public void useItem()
    {

    }
}
