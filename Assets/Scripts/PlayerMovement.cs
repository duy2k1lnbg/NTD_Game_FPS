using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed;  // Tốc độ di chuyển
    private Rigidbody rb;
    public float SprintModifier;

    public float JumForce;

    public float sensitivityX = 15F;
    public float sensitivityY = 15F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -60F;
    public float maximumY = 60F;

    float rotationY = 0F;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        rb = GetComponent<Rigidbody>();
    }

    //Thực hiện update vị trí của chuột để nhìn xung quanh
    void Update()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");


        // Tính vector di chuyển dựa trên phím nhấn
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical);
        //Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical) * speed * Time.deltaTime;

        // Áp dụng vector di chuyển vào vị trí của nhân vật
        //transform.Translate(movement);
        
    }


    private void FixedUpdate() 
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveHorizontal, 0f, moveVertical) * speed * Time.deltaTime;
        transform.Translate(movement);
        float rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;

        rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }
        
}
