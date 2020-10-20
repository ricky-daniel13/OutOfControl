using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PossCharacter : Posseable
{
    int lastnode=-1;
    Vector3 lastPos;
    Quaternion lastRota;
    public CharacterController control;
    public Animator anim;
    bool showMessage;

    public GameObject dialogGlobe;
    public Text dialogText;

    private void Start()
    {
    }

    public override void NormalUpdate()
    {
        base.NormalUpdate();
        showMessage = false;
    }

    private void LateUpdate()
    {
        dialogGlobe.SetActive(showMessage);
    }

    public override void DoEvent(TimeEvent ev, float currTimer, int node)
    {
        float currentLerpTime = currTimer - ev.start;
        if (currentLerpTime > ev.duration)
        {
            currentLerpTime = ev.duration;
        }

        float perc = currentLerpTime / ev.duration;

        bool isNewEvent = node != lastnode;
        lastnode = node;

        switch (ev.eventType)
        {
            case EventTypes.walkTo:
                {
                    if (isNewEvent)
                    {
                        Debug.Log("Doing walk", this.gameObject);
                        lastPos = transform.position;
                    }
                    float speed = (lastPos - ev.target.position).magnitude / ev.duration;
                    Vector3 direction = (ev.target.position - lastPos).normalized;
                    control.Move(direction * speed * Time.deltaTime);

                    break;
                }
            case EventTypes.lookTo:
                {
                    if (isNewEvent)
                    {
                        Debug.Log("Doing rotation", this.gameObject);
                        lastRota = transform.rotation;
                    }

                    /*Vector3 normTarget = ev.target.position;
                    normTarget.y = transform.position.y;
                    Quaternion toRota = Quaternion.LookRotation((normTarget - transform.position).normalized);

                    transform.rotation = Quaternion.Lerp(lastRota, toRota, perc);*/
                    transform.rotation = Quaternion.Lerp(lastRota, Quaternion.Euler(ev.rotation), perc);
                    break;
                }
            case EventTypes.message:
                {
                    if (isNewEvent)
                    {
                        dialogText.text = ev.message;
                    }
                    showMessage = true;
                    break;
                }
            case EventTypes.animate:
                {
                    if (isNewEvent)
                    {
                        anim.SetTrigger("anim" + ev.animation);
                    }
                    break;
                }
        }
    }

    public override void Scrub(TimeEvent ev, float currTimer, int node, Vector3 origin)
    {
        float currentLerpTime = currTimer - ev.start;
        if (currentLerpTime > ev.duration)
        {
            currentLerpTime = ev.duration;
        }

        float perc = currentLerpTime / ev.duration;

        lastnode = node;

        switch (ev.eventType)
        {
            case EventTypes.walkTo:
                {
                    lastPos = origin;
                    control.enabled = false;
                    if (ev.target != null)
                        transform.position = Vector3.Lerp(lastPos, ev.target.position, perc);
                    else
                        transform.position = lastPos;
                    control.enabled = true;
                    break;
                }
            case EventTypes.lookTo:
                {
                    lastRota = Quaternion.Euler(origin);
                    transform.rotation = Quaternion.Lerp(lastRota, Quaternion.Euler(ev.rotation), perc);
                    break;
                }
            case EventTypes.message:
                {
                    dialogText.text = ev.message;
                    break;
                }
            case EventTypes.animate:
                {
                    if (ev.animation == -1)
                    {
                        if (anim != null)
                            anim.Rebind();
                    }
                    else
                    {
                        anim.PlayInFixedTime("Base Layer.anim" + ev.animation, 0, currTimer - ev.start);
                        anim.Update(Time.unscaledDeltaTime);
                        Debug.Log("Hola");
                    }
                        
                    break;
                }
        }
    }
}
