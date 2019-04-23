using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Name", menuName = "Game Data/Weapon", order = 3)]
public class Weapon : Item {
    public float damage = 80f;
    public float speed = 1f;
}
