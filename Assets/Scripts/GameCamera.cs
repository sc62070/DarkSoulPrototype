using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unimotion;

public class GameCamera : MonoBehaviour {

    [Header("Settings")]
    public CameraSocket socket;

    public virtual void Awake() {
        DontDestroyOnLoad(this);
    }

    public virtual void Update() {
        if (Input.GetKeyDown(KeyCode.Z)) {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = !(Cursor.lockState == CursorLockMode.Locked);
        }
    }

    public void LateUpdate() {

        // Jerky camera movement is because of this
        /// Fix later...

        if(socket != null) {
            /*transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;*/
        }

    }

    public void OnSocketPositionChanged(CameraSocket socket) {
        if(socket == this.socket) {
            transform.position = socket.transform.position;
            transform.rotation = socket.transform.rotation;
        }
    }

    public static RaycastHit RaycastPastItself(Collider col, Vector3 startPos, Vector3 direction, float lenght, LayerMask mask, QueryTriggerInteraction queryTriggerInteraction) {
        RaycastHit[] rayHits = Physics.RaycastAll(startPos, direction, lenght, mask, queryTriggerInteraction);
        foreach (RaycastHit hit in rayHits) {
            if (hit.collider != col) {
                return hit;
            }
        }
        return new RaycastHit();
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
    }
}
