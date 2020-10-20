using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GhostMode {normal, poss, clock, transforming, detransform  }

public class GhostController : MonoBehaviour
{

    public float acc, dcc, maxSpeed, rotaSpeed;
    public GameObject model, trail, ghostCage;

    public GhostMode currMode;
    CharacterController controller;
    float speed;
    Vector2 movInput;
    Vector3 movement;
    Vector3 camRelInput;
    Quaternion movRotation, rotateTo;
    Camera mCamera;
    CameraControl cameraControl;

    public LayerMask possesableMask;

    [HideInInspector]
    public Posseable currPosses;
    Vector3 possVelocity;
    Vector3 possScaleVelocity;
    // Start is called before the first frame update
    void Start()
    {
        movement = Vector3.zero;
        movRotation = transform.rotation;
        rotateTo = transform.rotation;
        mCamera = Camera.main;
        controller = GetComponent<CharacterController>();
        currMode = GhostMode.normal;
        cameraControl = Camera.main.gameObject.GetComponent<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        switch(currMode)
        {
            case GhostMode.normal:
                {
                    RotatePlayer();
                    MovePlayer();
                    ApplyPlayer();
                    GhostActions();
                    break;
                }
            case GhostMode.poss:
                {
                    PossesActions();
                    break;
                }
            case GhostMode.transforming:
                {
                    Transforming();
                    break;
                }
            case GhostMode.detransform:
                {
                    Detransforming();
                    break;
                }
        }
        
    }

    void GetInput()
    {
        movInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        camRelInput = mCamera.transform.TransformDirection(new Vector3(movInput.x, 0, movInput.y));
        camRelInput.y = 0f;
        camRelInput.Normalize();

        RaycastHit hit;
        Ray ray = mCamera.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction, Color.blue);

        if (Input.GetButtonDown("Fire1") && (currMode == GhostMode.normal || currMode == GhostMode.poss) && Physics.Raycast(ray, out hit, 15f, possesableMask))
        {
            Debug.Log("Hit!");
            Posseable tempPerson = hit.transform.gameObject.GetComponent<Posseable>();
            if (tempPerson != null)
            {
                PossesGuy(tempPerson);
            }
        }
    }

    void GhostActions()
    {

    }

    void RotatePlayer()
    {
        if (movInput.magnitude > 0.1)
        {
            rotateTo = Quaternion.LookRotation(camRelInput);
        }
        movRotation = Quaternion.Lerp(movRotation, rotateTo, rotaSpeed * Time.deltaTime);
        Debug.DrawRay(transform.position, rotateTo * Vector3.forward, Color.red);
        Debug.DrawRay(transform.position, movRotation * Vector3.forward, Color.blue);
    }

    void MovePlayer()
    {
        if (movInput.magnitude > 0.1)
        {
            speed += acc * Time.deltaTime;
        }
        else if(speed > 0)
        {
            speed -= dcc * Time.deltaTime;
        }
        else
        {
            speed = 0;
        }

        if(speed > maxSpeed * Time.deltaTime)
        {
            speed = maxSpeed * Time.deltaTime;
        }

        movement = movRotation * (Vector3.forward * speed);
    }

    void ApplyPlayer()
    {
        controller.Move(movement);
        model.transform.rotation = movRotation;
    }


    //Posses Mode
    void PossesGuy(Posseable person)
    {
        if (currMode == GhostMode.poss)
        {
            currPosses.UnPosses();
            cameraControl.ToPoserMode(person);
            cameraControl.ToTravelMode();
            Debug.Log("Doing Travel");
        }
        currPosses = person;
        currMode = GhostMode.transforming;
        model.transform.localScale = Vector3.one;
    }

    void Transforming()
    {
        transform.position = Vector3.SmoothDamp(transform.position, currPosses.transform.position, ref possVelocity, 0.2f);
        model.transform.localScale = Vector3.SmoothDamp(model.transform.localScale, Vector3.zero, ref possScaleVelocity, 0.2f);
        
        if (model.transform.localScale.magnitude < 0.1)
        {
            currMode = GhostMode.poss;
            currPosses.Posses();
            cameraControl.ToPoserMode(currPosses);
            model.SetActive(false);
            trail.SetActive(false);
            ghostCage.SetActive(false);
            AudioManager.instance.toPossMusic();
        }
    }

    void PossesActions()
    {
        if (Input.GetButtonDown("Fire2"))
            UnPosses();
    }

    void UnPosses()
    {
        currPosses.UnPosses();
        transform.position = currPosses.transform.position;
        ghostCage.transform.position = currPosses.transform.position;
        currMode = GhostMode.detransform;
        model.SetActive(true);
        trail.SetActive(true);
        ghostCage.SetActive(true);
        model.transform.localScale = Vector3.zero;
        cameraControl.ToGhostMode();
        
    }

    void Detransforming()
    {
        
        model.transform.localScale = Vector3.SmoothDamp(model.transform.localScale, Vector3.one, ref possScaleVelocity, 0.2f);
        if (model.transform.localScale.magnitude > 1.6f)
        {
            currMode = GhostMode.normal;
            model.transform.localScale = Vector3.one;
            AudioManager.instance.toNormMusic();
        }
    }
}
