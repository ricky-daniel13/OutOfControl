using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimelineManager : MonoBehaviour
{
    public static TimelineManager instance;
    public float gameTime, seconds, minutes, hours;
    public bool timeStop = false;

    public event GameTimelineEvent StoppedTime, RecalculatedTime, StartedTime;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance != null)
            Destroy(this.gameObject);
        else
            instance = this;
    }

    void Start()
    {
        gameTime = 0;
    }

    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 100, 90), "MenuDebug");
        GUI.Label(new Rect(12, 24, 100, 50), "Tiempo: " + hours + ":" + minutes + ":" + seconds);
        if (timeStop)
            GUI.Label(new Rect(12, 44, 100, 50), "Time stop!");


    }

    void CalcTime()
    {
        hours = ((int)((gameTime / 60) / 60));
        if (Mathf.Floor((hours / 24))>0)
        {
            hours = (hours - (24 * Mathf.Floor((hours / 24))));
        }

        minutes = Mathf.Floor(gameTime / 60);
        if (Mathf.Floor((minutes / 60)) > 0)
        {
            minutes = (minutes - (60 * Mathf.Floor((minutes / 60))));
        }

        seconds = Mathf.Floor(gameTime);
        if (Mathf.Floor((seconds / 60)) > 0)
        {
            seconds = (seconds - (60 * Mathf.Floor((seconds / 60))));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale > 0)
        {
            gameTime += Time.deltaTime;
        }
        CalcTime();
    }

    public void StopTime()
    {
        Time.timeScale = 0;
        StoppedTime();
        timeStop = true;
    }

    public void ReStartTime()
    {
        Time.timeScale = 1;
        timeStop = false;
        StartedTime();
    }

    public void ReCalculate()
    {
        RecalculatedTime();
    }

    public void ChangeTime(float absoluteTime)
    {
        gameTime = absoluteTime;
        if (gameTime < 0)
            gameTime = 0;
    }

    public delegate void GameTimelineEvent();

}
