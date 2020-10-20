using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignDialog : MonoBehaviour
{
    // Update is called once per frame
    private void OnEnable()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }
}
