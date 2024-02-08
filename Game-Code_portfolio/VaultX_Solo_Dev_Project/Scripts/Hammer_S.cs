using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hammer_S : MonoBehaviour
{

    [Header("REF")]
    [SerializeField] private Transform hammerBase;

    [Header("SETTINGS")]
    [SerializeField] private float rotTime = 0f;
    [SerializeField] private float rotAngle = 40.0f;
    [SerializeField] private float rotSpeed = 2.0f;
    [SerializeField] private float timeMultiplier = 10.0f;
    [SerializeField] private float desiredRot = 0f;
    [SerializeField] private float damping = 0f;
    [SerializeField] private float startSwingDelay = 0;


    [Header("Player")]
    [SerializeField] private Rigidbody rb;
    private bool isSwinging;




    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SwingHammer());
    }

    // Update is called once per frame
    void Update()
    {
        if (isSwinging)
        {
            rotTime += Time.deltaTime * timeMultiplier;
            hammerBase.rotation = Quaternion.Euler(rotAngle * Mathf.Sin(rotTime), 0f, 0f);
        }
    }

    private IEnumerator SwingHammer()
    {
        yield return new WaitForSeconds(startSwingDelay);
        isSwinging = true;
    }
}
