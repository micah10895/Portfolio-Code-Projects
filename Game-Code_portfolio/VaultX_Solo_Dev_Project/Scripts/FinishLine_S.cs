using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System;

public class FinishLine_S : MonoBehaviour
{
    //public TextMeshProUGUI displayTime;
    public Canvas winMenu;
    public Text displayVar;

    int minutes;
    float seconds;

    bool start;

    private void Start()
    {
        //winMenu.enabled = false;
        minutes = 0;
        seconds = 0;
        start = false;
        _start();
    }

    private void Update()
    {
        if (start)
        {
            seconds += Time.deltaTime;
            if (seconds >= 60.0f)
            {
                seconds = 0;
                minutes += 1;
            }
        }
        //displayVar.text = minutes.ToString() + ":" + seconds.ToString("00.00");
    }

    private void _start()
    {
        start = true;
    }

    private void _stop()
    {
        start = false;
    }

    private void _reset()
    {
        start = false;
        minutes = 0;
        seconds = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            _stop();
            winMenu.enabled = true;
            Time.timeScale = 0f;
            //displayTime.text = "Time : " + displayVar.text;
            //SceneManager.LoadScene(0);
        }
    }
}
