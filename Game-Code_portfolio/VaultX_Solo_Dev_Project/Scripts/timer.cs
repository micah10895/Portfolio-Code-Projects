using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    bool stopWatchActive = false;
    float currentTime;
    public TextMeshProUGUI currentTimeText;
    FinishLine_S finish;


    // Start is called before the first frame update
    void Start()
    {
        finish = GameObject.Find("FinishLine").GetComponent<FinishLine_S>();
        currentTime = 0;
        StartStopWatch();
    }

    // Update is called once per frame
    void Update()
    {
        if (stopWatchActive)
        {
            currentTime = currentTime + Time.deltaTime;

        }
        TimeSpan time = TimeSpan.FromSeconds( currentTime );
        //currentTimeText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        currentTimeText.text = time.ToString(@"mm\:ss\:fff");
        
    }

    public void StartStopWatch()
    {
        stopWatchActive = true;
    }
    public void StopStopWatch()
    {
        stopWatchActive = false;
    }
}
