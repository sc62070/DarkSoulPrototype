using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviourPunCallbacks
{

    static NetworkManager instance;

    public GameObject playerPrefab;

    public bool offlineModeInEditor = true;

    void Awake() {

        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void ConnectToMaster(string username) {
        FindObjectOfType<SystemText>().ShowText("Connecting to master...");

        if (Application.isEditor && offlineModeInEditor) {
            PhotonNetwork.PhotonServerSettings.StartInOfflineMode = true;
        } else {
            PhotonNetwork.PhotonServerSettings.StartInOfflineMode = false;
        }

        PhotonNetwork.NickName = username;
        PhotonNetwork.GameVersion = "v1";

        PhotonNetwork.ConnectUsingSettings();
        //PhotonNetwork.ConnectToRegion("usw");
    }
}
