using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerModel : MonoBehaviour
{
    [Header("Movement")]
    public float speed;

    Vector3 moveDir;
    public float life;
    private Rigidbody _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Cursor.visible = false;
    }

    public void Move(float horizontalInput, float verticalInput)
    {
        moveDir = transform.forward * verticalInput + transform.right * horizontalInput;

        moveDir *= speed;
        moveDir.y = _rb.velocity.y;
        _rb.velocity = moveDir;
    }

    public void RIP()
    {
        //SceneManager.LoadScene("LoseScreen");
    }

    //void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("FinishLine"))
    //    {
    //        SceneManager.LoadScene("WinScreen");
    //    }
    //}
}
