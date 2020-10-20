using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Posseable : MonoBehaviour
{
    public Transform eyePosition;
    bool possesed=false;
    public Vector3 startPos;
    public Quaternion startRota;

    private void Awake()
    {
        startPos = transform.position;
        startRota = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (possesed)
            PossesedUpdate();

        NormalUpdate();

    }

    public virtual void Posses()
    {
        possesed = true;
    }

    public virtual void UnPosses()
    {
        possesed = false;
    }


    public virtual void PossesedUpdate()
    {

    }

    public virtual void NormalUpdate()
    {

    }

    public virtual void DoEvent(TimeEvent ev, float currTimer, int node)
    {

    }

    public virtual void Scrub(TimeEvent ev, float currTimer, int node, Vector3 origin)
    {

    }
}
