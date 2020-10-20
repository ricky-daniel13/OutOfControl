using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class CameraControl : MonoBehaviour
{
    public GameObject player;
    public Posseable poserTarget;
    public PostProcessVolume scaryVol;

    enum mode {ghost, poser, travel};

    mode currMode;
    //3rd Person Camera Settings
    public float distanceFromPlayer, cameraRotationX, cameraRotationZ, rotateAmmount, cameraTime, followSpeed, motionBlurOff, motionBlurOn;
    public Vector3 ghostOffset;
    float currRota, currentLerpTime;
    Quaternion ghostRota, lastRota;
    Vector3 dampVelocity, followVelocity, currPos, toPos;

    //Poss Camera Settings
    Vector3 rotation = new Vector3(0, 0, 0);
    public float viewSpeed, horLimit, verLimit;

    // Start is called before the first frame update
    void Start()
    {
        //Ghost mode initiation
        currMode = mode.ghost;
        currRota = 0;
        ghostRota = Quaternion.identity;
        dampVelocity = Vector3.zero;
        currPos = Vector3.zero;
        toPos = Vector3.zero;

        currentLerpTime = 0;
        lastRota = transform.rotation;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        switch (currMode)
        {
            case mode.ghost:
                {
                    GhostMode();
                    break;
                }
            case mode.poser:
                {
                    PoserMode();
                    break;
                }
            case mode.travel:
                {
                    TravelMode();
                    break;
                }
        }
    }

    void GhostMode()
    {
        //Camera input
        if (Input.GetButtonDown("Camera Left"))
        {
            currRota += rotateAmmount;
            currentLerpTime = 0;
            lastRota = transform.rotation;
        }
        if (Input.GetButtonDown("Camera Right"))
        {
            currRota -= rotateAmmount;
            currentLerpTime = 0;
            lastRota = transform.rotation;
        }
        
        //Rotation Lerp
        currentLerpTime += Time.unscaledDeltaTime;
        if (currentLerpTime > cameraTime)
        {
            currentLerpTime = cameraTime;
        }

        ghostRota = Quaternion.Euler(new Vector3(cameraRotationX, currRota, cameraRotationZ));
        toPos = ghostRota * (Vector3.forward * distanceFromPlayer);
        currPos = Vector3.SmoothDamp(currPos, -toPos, ref dampVelocity, cameraTime);

        transform.rotation = Quaternion.Lerp(lastRota, ghostRota, (currentLerpTime / (cameraTime)));

        transform.position = Vector3.Lerp(transform.position, player.transform.position + ghostOffset + currPos, followSpeed * Time.unscaledDeltaTime);
        //transform.position = player.transform.position + ghostOffset + currPos;


    }

    void PoserMode()
    {
        
        transform.position = poserTarget.eyePosition.transform.position;

        rotation.y += (Input.GetAxis("Horizontal") * viewSpeed);
        rotation.x += -(Input.GetAxis("Vertical") * viewSpeed);
        rotation.x = Mathf.Clamp(rotation.x, -verLimit, verLimit);
        rotation.y = Mathf.Clamp(rotation.y, -horLimit, horLimit);

        transform.rotation = poserTarget.eyePosition.transform.rotation * Quaternion.Euler(rotation);
    }

    void TravelMode()
    {
        transform.position = Vector3.Lerp(transform.position, poserTarget.eyePosition.transform.position, 6 * Time.deltaTime);
        if (Vector3.Distance(transform.position, poserTarget.eyePosition.transform.position) < 0.1)
        {
            currMode = mode.poser;
            scaryVol.profile.GetSetting<MotionBlur>().shutterAngle.value = motionBlurOff;
        }
    }

    public void ToPoserMode(Posseable _target)
    {
        poserTarget = _target;
        currMode = mode.poser;
        scaryVol.weight = 1;
        rotation = Vector3.zero;
    }

    public void ToGhostMode()
    {
        currMode = mode.ghost;
        scaryVol.weight = 0;
    }

    public void ToTravelMode()
    {
        currMode = mode.travel;
        scaryVol.profile.GetSetting<MotionBlur>().shutterAngle.value = motionBlurOn;
    }
}
