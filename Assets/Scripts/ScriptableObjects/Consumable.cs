using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Name", menuName = "Game Data/Consumable", order = 2)]
public class Consumable : Item {

    [Header("Consumable")]
    public float healingQuantity;
}
