using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class MainMenu : MonoBehaviourPunCallbacks {

    public InputField userField;
    public Button loginBtn;

    public Dropdown graphicsDropdown;

    public override void OnJoinedRoom() {
        gameObject.SetActive(false);
    }
}
