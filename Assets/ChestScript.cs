using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestScript : MonoBehaviour
{
    private CameraScript cameraScript;
    private GameObject player;
    public bool isActive = false;
    private Canvas inventorycanvas;

    void Start()
    {
        player = GameObject.Find("Player");
        cameraScript = GameObject.Find("Main Camera").GetComponent<CameraScript>();
        //inventorycanvas = GameObject.Find("");
    }
}
