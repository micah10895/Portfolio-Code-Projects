using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    [SerializeField] public GameObject respawnPoint;
    [SerializeField] public GameObject player;
    [SerializeField] Rigidbody rb;
    private Transform startTransform;
    //[SerializeField] public CharacterController characterController;
    // Start is called before the first frame update
    void Start()
    {
        //startTransform.rotation = player.transform.rotation;
    }
    private void Awake()
    {
        //startTransform.rotation = player.transform.rotation;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            player.transform.position = respawnPoint.transform.position;
            if (rb != null)
                rb.velocity = new Vector3(0,0,0);
            //player.transform.rotation = startTransform.transform.rotation;
        }
        Debug.Log("hit");
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "Player")
    //    {
    //        player.transform.position = respawnPoint.transform.position;
    //    }
    //    //characterController.transform.position = respawnPoint.transform.position;
    //    Debug.Log("hit");

    //}

    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    if (hit.gameObject.tag == "Player")
    //    {
    //        player.transform.position = respawnPoint.transform.position;
    //    }
    //    Debug.Log("hit");
    //}
}
