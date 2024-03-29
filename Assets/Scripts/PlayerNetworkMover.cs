using Photon.Pun;
using UnityEngine;
using UnityEditor;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

[RequireComponent(typeof(FirstPersonController))]

public class PlayerNetworkMover : MonoBehaviourPunCallbacks, IPunObservable {

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private GameObject cameraObject;
    [SerializeField]
    private GameObject gunObject;
    [SerializeField]
    private GameObject playerObject;
    [SerializeField]
    private NameTag nameTag;

    private Vector3 position;
    private Quaternion rotation;
    private bool jump;
    private float smoothing = 10.0f;

    private bool displayMenu = false;

    bool cursorVisible = true;

    /// <summary>
    /// Move game objects to another layer.
    /// </summary>
    void MoveToLayer(GameObject gameObject, int layer) {
        gameObject.layer = layer;
        foreach(Transform child in gameObject.transform) {
            MoveToLayer(child.gameObject, layer);
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake() {
        // FirstPersonController script require cameraObject to be active in its Start function.
        if (photonView.IsMine) {
            cameraObject.SetActive(true);
        }
    }

    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start() {
        if (photonView.IsMine) {
            GetComponent<FirstPersonController>().enabled = true;
            MoveToLayer(gunObject, LayerMask.NameToLayer("Hidden"));
            MoveToLayer(playerObject, LayerMask.NameToLayer("Hidden"));
            // Set other player's nametag target to this player's nametag transform.
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                player.GetComponentInChildren<NameTag>().target = nameTag.transform;
            }
        } else {
            position = transform.position;
            rotation = transform.rotation;
            // Set this player's nametag target to other players's target.
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject player in players) {
                if (player != gameObject) {
                    nameTag.target = player.GetComponentInChildren<NameTag>().target;
                    break;
                }
            }
        }
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    void Update() {
        if (!photonView.IsMine) {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * smoothing);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * smoothing);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            displayMenu = !displayMenu; // Đảo ngược trạng thái của biến displayMenu
        }
    }

    /// <summary>
    /// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
    /// </summary>
    void FixedUpdate() {
        if (photonView.IsMine) {
            animator.SetFloat("Horizontal", CrossPlatformInputManager.GetAxis("Horizontal"));
            animator.SetFloat("Vertical", CrossPlatformInputManager.GetAxis("Vertical"));
            if (CrossPlatformInputManager.GetButtonDown("Jump")) {
                animator.SetTrigger("IsJumping");
            }
            animator.SetBool("Running", Input.GetKey(KeyCode.LeftShift));
        }
    }

    /// <summary>
    /// Used to customize synchronization of variables in a script watched by a photon network view.
    /// </summary>
    /// <param name="stream">The network bit stream.</param>
    /// <param name="info">The network message information.</param>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
        if (stream.IsWriting) {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        } else {
            position = (Vector3)stream.ReceiveNext();
            rotation = (Quaternion)stream.ReceiveNext();
        }
    }


    void OnGUI()
    {
        if (displayMenu)
        {
            // Hiển thị bảng lựa chọn ở đây
            // Ví dụ: 
            GUI.Box(new Rect(Screen.width / 2 - 480, Screen.height / 2 - 300, 960, 600), "Menu");
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 - 50, 300, 100), "Chơi Tiếp"))
            {
                // Xử lý khi người chơi chọn chơi tiếp
                // Đặt biến displayMenu thành false để ẩn bảng lựa chọn
                Input.GetKeyDown(KeyCode.Escape);
                displayMenu = false;
            }
            if (GUI.Button(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 80, 300, 100), "Thoát"))
            {
                // Xử lý khi người chơi chọn thoát
                #if UNITY_EDITOR
                     UnityEditor.EditorApplication.isPlaying = false;
                #else
                     UnityEngine.Application.Quit();
                #endif
            }
        }
    }

}
