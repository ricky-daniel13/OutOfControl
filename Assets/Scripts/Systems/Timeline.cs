using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EventTypes {walkTo, lookTo, animate, message };

[System.Serializable]
public class TimeEvent
{
    public EventTypes eventType = EventTypes.message;
    public float start;
    public Transform target;
    public Vector3 rotation;
    public AudioClip sound;
    public float duration;
    public int animation;
    public string message;
}

public class Timeline : MonoBehaviour
{
    int currNode=0;
    float currTimer, currSeconds;
    public List<TimeEvent> events;
    bool playCharacter = true;

    [HideInInspector]
    public Posseable character;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Posseable>();
        TimelineManager.instance.StartedTime += OnStartedTime;
        TimelineManager.instance.StoppedTime += OnStoppedTime;
        TimelineManager.instance.RecalculatedTime += OnRecalculate;
    }

    // Update is called once per frame
    void Update()
    {
        if (playCharacter)
        {
            currTimer = TimelineManager.instance.gameTime;
            if (currNode == -1)
                currNode = 0;

            if (events[currNode].duration + events[currNode].start < currTimer)
            {
                currNode += 1;
            }
            if (currNode == events.Count)
            {
                playCharacter = false;
                return;
            }

            if (events[currNode].start < currTimer)
            {
                character.DoEvent(events[currNode], currTimer, currNode);
            }
            
        }
    }

    void OnStoppedTime()
    {
        playCharacter = false;
    }

    void OnStartedTime()
    {
        playCharacter = true;
    }

    void OnRecalculate()
    {
        float newTime = TimelineManager.instance.gameTime;
        int currEvent = BinarySearch(newTime);

        if (events[currEvent].start > newTime)
            currEvent = -1;

        int lastMove = GetLastEvent(currEvent, EventTypes.walkTo);
        int preLastMove = GetLastEvent(lastMove-1, EventTypes.walkTo);

        TimeEvent startEv;

        if (lastMove == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.walkTo;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.target = null;
        }
        else
        {
            startEv = (events[lastMove]);
        }

        Vector3 lastPos = character.startPos;

        if (preLastMove != -1)
            lastPos = events[preLastMove].target.position;

        character.Scrub(startEv, newTime, currEvent, lastPos);

        int lastRot = GetLastEvent(currEvent, EventTypes.lookTo);
        int preLastRot = GetLastEvent(lastRot-1, EventTypes.lookTo);

        lastPos = character.startRota.eulerAngles;
        if (lastRot == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.lookTo;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.rotation = lastPos;
        }
        else
        {
            startEv = (events[lastRot]);
        }

        if (preLastRot != -1)
            lastPos = events[preLastRot].rotation;

        character.Scrub(startEv, newTime, currEvent, lastPos);

        int lastAnim = GetLastEvent(currEvent, EventTypes.animate);
        if (lastAnim != -1)
            character.Scrub(events[lastAnim], newTime, currEvent, lastPos);

        int lastMesssage = GetLastEvent(currEvent, EventTypes.message);

        if (lastMesssage == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.animate;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.animation = -1;
        }
        else
            startEv = events[lastMesssage];

        
            character.Scrub(startEv, newTime, currEvent, lastPos);

        currNode = currEvent;
    }

    public void EditorScrub(float newTime)
    {
        int currEvent = BinarySearch(newTime);

        if (events[currEvent].start > newTime)
            currEvent = -1;

        int lastMove = GetLastEvent(currEvent, EventTypes.walkTo);
        int preLastMove = GetLastEvent(lastMove - 1, EventTypes.walkTo);

        TimeEvent startEv;

        if (lastMove == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.walkTo;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.target = null;
        }
        else
        {
            startEv = (events[lastMove]);
        }

        Vector3 lastPos = character.startPos;

        if (preLastMove != -1)
            lastPos = events[preLastMove].target.position;

        character.Scrub(startEv, newTime, currEvent, lastPos);

        int lastRot = GetLastEvent(currEvent, EventTypes.lookTo);
        int preLastRot = GetLastEvent(lastRot - 1, EventTypes.lookTo);

        lastPos = character.startRota.eulerAngles;
        if (lastRot == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.lookTo;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.rotation = lastPos;
        }
        else
        {
            startEv = (events[lastRot]);
        }

        if (preLastRot != -1)
            lastPos = events[preLastRot].rotation;

        character.Scrub(startEv, newTime, currEvent, lastPos);

        int lastAnim = GetLastEvent(currEvent, EventTypes.animate);
        if (lastAnim != -1)
            character.Scrub(events[lastAnim], newTime, currEvent, lastPos);

        int lastMesssage = GetLastEvent(currEvent, EventTypes.message);

        if (lastMesssage == -1)
        {
            startEv = new TimeEvent();
            startEv.eventType = EventTypes.animate;
            startEv.duration = Mathf.Infinity;
            startEv.start = 0;
            startEv.animation = -1;
        }
        else
            startEv = events[lastMesssage];


        character.Scrub(startEv, newTime, currEvent, lastPos);
    }

    public Vector3 GetPositionAtPoint(float newTime)
    {
        int currEvent = BinarySearch(newTime);

        if (events[currEvent].start > newTime)
            currEvent = -1;

        int lastMove = GetLastEvent(currEvent, EventTypes.walkTo);
        int preLastMove = GetLastEvent(lastMove - 1, EventTypes.walkTo);

        if (lastMove == -1)
        {
            return transform.position;
        }
        else
        {
            return events[lastMove].target.position;
        }
    }

    public int BinarySearch(float currTime)
    {
        int minNum = 0;
        int maxNum = events.Count - 1;
        int mid = 0;

        while (minNum <= maxNum)
        {
            mid = (minNum + maxNum) / 2;
            //Debug.Log("Current Event #" + mid + ": " + events[mid].eventType + " Time " + events[mid].start + " ~ " + (events[mid].start + events[mid].duration) + " targetTime " + currTime);
            if (currTime > events[mid].start && currTime < (events[mid].start + events[mid].duration))
            {
                //Debug.Log("Found acceptable event");
                return mid;
            }
            else if (currTime < events[mid].start)
            {
                //Debug.Log("Going back");
                maxNum = mid - 1;
            }
            else
            {
                //Debug.Log("Going forward");
                minNum = mid + 1;
            }
        }

        return mid;
    }

    public int GetLastEvent(int from, EventTypes eventType)
    {
        if (from < 0 || from >= events.Count)
            return -1;

        while (from > 0 )
        {
            if (events[from].eventType == eventType)
                return from;
            else
                from--;
        }
        return -1;
    }

    public TimeEvent getCurrentEvent()
    {
        return null;
    }
}
