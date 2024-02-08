using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class StopWatch : MonoBehaviour
{
    public Text displayVar;

    int minutes;
    float seconds;

    bool start;

    public GameObject finishLine;


    // Start is called before the first frame update
    void Start()
    {
        minutes = 0;
        seconds = 0;
        start = false;
        _start();
    }

    // Update is called once per frame
    void Update()
    {
        RunTimer();
    }

    public void RunTimer()
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
        displayVar.text = minutes.ToString() + ":" + seconds.ToString("00.00");
    }

    public void _start()
    {
        start = true;
    }

    public void _stop()
    {
        start = false;
    }

    public void _reset()
    {
        start = false;
        minutes = 0;
        seconds = 0;
    }
}
