using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour {

    public List<Item> items = new List<Item>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public bool Contains(Item item) {
        return items.Contains(item);
    }
}
