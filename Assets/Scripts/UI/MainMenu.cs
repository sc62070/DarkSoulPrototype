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

    public void ShowControls() {
        UIManager.ShowDialog("Left Click: Attack \nMouse Wheel Click: Heavy Attack \nRight Click: Block \nShift: Evade \nSpace: Action \nG: Target \nZ: Free Mouse ");
    }
}
