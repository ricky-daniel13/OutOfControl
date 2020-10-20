using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossesClock : Posseable
{
    public override void PossesedUpdate()
    {
        base.PossesedUpdate();


        if (TimelineManager.instance.timeStop)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
                TimelineManager.instance.ChangeTime(TimelineManager.instance.gameTime - 5);
            if (Input.GetKeyDown(KeyCode.RightArrow))
                TimelineManager.instance.ChangeTime(TimelineManager.instance.gameTime + 5);
            if (Input.GetKeyDown(KeyCode.R))
                TimelineManager.instance.ReCalculate();
        }
    }
    public override void Posses()
    {
        base.Posses();
        TimelineManager.instance.StopTime();
    }

    public override void UnPosses()
    {
        base.UnPosses();
        TimelineManager.instance.ReStartTime();
    }
}
