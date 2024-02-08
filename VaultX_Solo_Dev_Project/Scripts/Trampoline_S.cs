using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trampoline_S : MonoBehaviour
{
    [SerializeField] private float jumpHeight = 10f;
    private float trampolineForce;

    private void OnCollisionEnter(Collision collision)
    {
        trampolineForce = Mathf.Sqrt(jumpHeight * -2f * (Physics.gravity.y));
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

            if(rb != null )
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(new Vector3(0f, trampolineForce, 0f), ForceMode.Impulse);
            }
        }
    }


}
