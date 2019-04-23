using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Name", menuName = "Game Data/Item", order = 1)]
public class Item : ScriptableObject {

    [Header("Basic")]
    public string name;

    [Header("Graphics")]
    public Sprite thumbnail;
    public GameObject prefab;
}
