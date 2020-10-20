using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Timeline))]
public class Timeline_Editor : Editor
{
    Timeline editTarget;
    bool scrubEnable = false;
    bool scrub = false;
    float scrubPosition=0;
    float lastScrub = -1;
    List<bool> foldEvents;
    bool reStockEvents;
    private Vector2 templatesScroll = Vector2.zero;

    public void OnEnable()
    {
        editTarget = (Timeline)target;
        SceneView.duringSceneGui += TimelineUpdate;
    }
    private void OnDisable()
    {
        if (scrub)
        {
            scrub = false;
            editTarget.transform.position = editTarget.character.startPos;
            editTarget.transform.rotation = editTarget.character.startRota;
        }
    }
    // Start is called before the first frame update
    public override void OnInspectorGUI()
    {
        if (!scrub)
            Timeline();
        Scrubber();
    }

    void Scrubber()
    {
        scrubEnable = (EditorGUILayout.Toggle("Scrubb", scrubEnable));
        if (scrubEnable)
        {
            if (!scrub)
            {
                scrub = true;
                editTarget.character = editTarget.GetComponent<Posseable>();
                editTarget.character.startPos = editTarget.transform.position;
                editTarget.character.startRota = editTarget.transform.rotation;
            }

            float maxTime = editTarget.events[editTarget.events.Count - 1].start + editTarget.events[editTarget.events.Count - 1].duration;

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.FloatField(scrubPosition);
            EditorGUI.EndDisabledGroup();
            scrubPosition = EditorGUILayout.Slider(scrubPosition, 0, maxTime);
            if (lastScrub != scrubPosition)
                editTarget.EditorScrub(scrubPosition);
            lastScrub = scrubPosition;
            GUILayout.EndHorizontal();
        }
        else
        {
            if (scrub)
            {
                scrub = false;
                editTarget.transform.position = editTarget.character.startPos;
                editTarget.transform.rotation = editTarget.character.startRota;
            }
        }
    }

    Vector3 CalculateLookTo(float time, Vector3 to)
    {
        Vector3 from = editTarget.GetPositionAtPoint(time);
        to.y = from.y;
        return Quaternion.LookRotation((to - from).normalized).eulerAngles;
    }

    float CalculateDuration(float time, Vector3 to, float speed)
    {
        Vector3 from = editTarget.GetPositionAtPoint(time);
        return (from - to).magnitude / speed;
    }


    void Timeline()
    {
        if (foldEvents == null || foldEvents.Count != editTarget.events.Count)
        {
            foldEvents = new List<bool>();
            reStockEvents = true;
        }
        else
            reStockEvents = false;

        GUILayout.Label("Templates", EditorStyles.boldLabel);
        templatesScroll = EditorGUILayout.BeginScrollView(templatesScroll, GUILayout.Height(200));


        for (int i=0; i < editTarget.events.Count; i++)
        {
            if (reStockEvents == true)
                foldEvents.Add(false);
            
            foldEvents[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldEvents[i], "Event #"+i);
            if (foldEvents[i])
            {
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical();
                editTarget.events[i].eventType = (EventTypes)EditorGUILayout.EnumPopup("Event Type", editTarget.events[i].eventType);
                editTarget.events[i].start = EditorGUILayout.FloatField("Time", editTarget.events[i].start);
                switch (editTarget.events[i].eventType)
                {
                    case EventTypes.walkTo:
                        {
                            editTarget.events[i].target = EditorGUILayout.ObjectField("Walk To", editTarget.events[i].target, typeof(Transform), true) as Transform;
                            editTarget.events[i].duration = EditorGUILayout.FloatField("Duration", editTarget.events[i].duration);
                            if (GUILayout.Button("Calculate duration as Speed"))
                                editTarget.events[i].duration = CalculateDuration(editTarget.events[i].start, editTarget.events[i].target.position, editTarget.events[i].duration);
                            break;
                        }
                    case EventTypes.message:
                        {
                            editTarget.events[i].message = EditorGUILayout.TextField("Message", editTarget.events[i].message);
                            editTarget.events[i].duration = EditorGUILayout.FloatField("Duration", editTarget.events[i].duration);
                            break;
                        }
                    case EventTypes.animate:
                        {
                            editTarget.events[i].animation = EditorGUILayout.IntField("Animation", editTarget.events[i].animation);
                            editTarget.events[i].duration = 1;
                            break;
                        }
                    case EventTypes.lookTo:
                        {
                            editTarget.events[i].rotation = EditorGUILayout.Vector3Field("Rotation", editTarget.events[i].rotation);
                            editTarget.events[i].duration = EditorGUILayout.FloatField("Duration", editTarget.events[i].duration);
                            editTarget.events[i].target = EditorGUILayout.ObjectField("LookTo Object", editTarget.events[i].target, typeof(Transform), true) as Transform;
                            if (GUILayout.Button("Calculate LookTo"))
                                editTarget.events[i].rotation = CalculateLookTo(editTarget.events[i].start, editTarget.events[i].target.position);


                            break;
                        }
                }
                GUILayout.EndVertical();
                if (GUILayout.Button("+"))
                {
                    TimeEvent addition = new TimeEvent();
                    addition.start = editTarget.events[i].start + editTarget.events[i].duration;
                    editTarget.events.Insert(i+1, addition);
                    foldEvents.Insert(i+1,true);
                }
                if (GUILayout.Button("-"))
                    editTarget.events.RemoveAt(i);
                GUILayout.EndHorizontal();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            
        }
        EditorGUILayout.EndScrollView();
    }

    void TimelineUpdate(SceneView sceneview)
    {
        Event e = Event.current;
    }
}
