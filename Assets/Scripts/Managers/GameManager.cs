using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Unimotion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Animations;

public class GameManager : MonoBehaviourPunCallbacks {

    static GameManager instance;

    public NetworkManager networkManager;
    public GameObject playerPrefab;
    public static Unimotion.Player localPlayer;

    [Header("Texts")]
    public SystemText systemText;

    public GameAnimations animations;

    private static string targetScene = "world";
    private static string targetMarkerId = "1";

    void Awake() {

        if (instance != null && instance != this) {
            Destroy(this);
        } else {
            instance = this;
        }

        DontDestroyOnLoad(this);
        DontDestroyOnLoad(Camera.main);

        // Mark all the things on the current scene as DontDestroyOnLoad
        foreach (GameObject mb in SceneManager.GetSceneByName("main").GetRootGameObjects()) {
            DontDestroyOnLoad(mb);
        }
    }

    void Start() {
        //networkManager.ConnectToMaster();
    }

    public static void GoToScene(string sceneName, string markerId) {

        targetScene = sceneName;
        targetMarkerId = markerId;

        if (PhotonNetwork.OfflineMode == false) {
            if (PhotonNetwork.CurrentRoom != null) {
                PhotonNetwork.LeaveRoom();
            }
        } else {
            PhotonNetwork.Destroy(localPlayer.photonView);
            FindObjectOfType<GameManager>().OnJoinedRoom();
        }

    }

    public override void OnConnectedToMaster() {
        //GameManager.GoToScene("apartment_01", "1");

        if (PhotonNetwork.OfflineMode == true) {
            systemText.ShowText("Creating offline room...");
            PhotonNetwork.JoinOrCreateRoom("offline", new RoomOptions() { MaxPlayers = 20 }, TypedLobby.Default);
        }

        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby() {
        Debug.Log("Creating or joining to room " + targetScene + "...");
        systemText.ShowText("Joining " + targetScene + "...");
        PhotonNetwork.JoinOrCreateRoom(targetScene, new RoomOptions() { MaxPlayers = 20 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom() {
        Debug.Log("Joined room");
        systemText.ShowText("Joined " + targetScene + "...");
        localPlayer = PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<Unimotion.Player>();

        /*GameCamera camera = Camera.main.GetComponent<GameCamera>();
        camera.socket = localPlayer.cameraSocket;*/

        UnityEngine.Events.UnityAction<Scene, LoadSceneMode> tmpDelegate = null;
        tmpDelegate = delegate (Scene scene, LoadSceneMode mode) {

            localPlayer.transform.position = Vector3.zero;
            localPlayer.transform.rotation = Quaternion.identity;

            FindObjectOfType<UIManager>().character = localPlayer.GetComponent<Character>();
            Unimotion.Player.main = localPlayer;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            SceneManager.sceneLoaded -= tmpDelegate;
        };

        SceneManager.sceneLoaded += tmpDelegate;
        SceneManager.LoadScene(targetScene);
    }

    public void Quit() {
        Application.Quit(0);
    }

}

[System.Serializable]
public class GameAnimations {
    public RuntimeAnimatorController normalAnimator;
}
