using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using ScriptableObjectArchitecture.Variables;


/// <summary>
/// This class drives the position and displacement of the main camera, as well as the displacement of the floor objects
/// </summary>
public class CameraController : MonoBehaviour
{

    private float moveForwardSpeed;
    private float angle;
    private float lastAngle;
    private float distance;

    [Space]
    public bool randomFlip = true;
    public bool lerpTransition = true;
    public float lerpTime = 1;

    [Space]
    public int maxGroundObjectInstance = 3;

    [Space]
    public GameObject MocapObjects;

    public GameObject[] groundPrefab;
    public Transform background;

    public Transform[] followAngle;

    public float distanceToSpawnNewGroundObject = 33;
    public float offsetDistance = 3;

    public List<GameObject> groundObjectsList = new List<GameObject>();

    public FloatVariable treadmillSpeed;
    public FloatVariable treadmillAngle;



    void Start()
    {
        // create the ground object sequence
        for (int i = 0; i < maxGroundObjectInstance; i++)
        {
            if (i >= groundObjectsList.Count)
                AddGroundObject(i);
        }

        background.Translate(background.forward * distanceToSpawnNewGroundObject, Space.Self);

        distance = 0.1f;
    }




    // Update is called once per frame
    void Update()
    {
        //Values from the float variables that drives speed and angle
        moveForwardSpeed = treadmillSpeed.Value;
        angle = treadmillAngle.Value;

        distance += moveForwardSpeed * Time.deltaTime;

        if (angle != lastAngle)
        {
            // compute the new position taking account for angle
            transform.position = transform.position - TrigoVector(groundObjectsList[0].transform.eulerAngles.x, distance) + TrigoVector(-angle, distance);
           
            //child objects are moved accordingly
            if (MocapObjects)
            {
                foreach (Transform child in MocapObjects.transform)
                {
                    child.position = child.position - TrigoVector(groundObjectsList[0].transform.eulerAngles.x, 4.5f) + TrigoVector(-angle, 4.5f);
                }
            }
           
            
            //next ground objects are moved
            groundObjectsList[0].transform.eulerAngles = new Vector3(-angle, groundObjectsList[0].transform.eulerAngles.y, groundObjectsList[0].transform.eulerAngles.z);
            groundObjectsList[1].transform.eulerAngles = new Vector3(-angle, groundObjectsList[1].transform.eulerAngles.y, groundObjectsList[1].transform.eulerAngles.z);

            groundObjectsList[1].transform.position = groundObjectsList[0].transform.position + TrigoVector(-angle, distanceToSpawnNewGroundObject);
            background.transform.position = groundObjectsList[0].transform.position + TrigoVector(-angle, 2 * distanceToSpawnNewGroundObject);

            lastAngle = angle;
        }

        //camera is moving forward at the given speed
        transform.Translate(TrigoVector(groundObjectsList[0].transform.eulerAngles.x).normalized * moveForwardSpeed * Time.deltaTime);

        foreach (Transform t in followAngle)
            if (groundObjectsList[0].transform.eulerAngles.x > 0 && Mathf.Abs(t.eulerAngles.x - groundObjectsList[0].transform.eulerAngles.x) > 0.1f)
            {
                t.rotation = Quaternion.Lerp(t.rotation, groundObjectsList[0].transform.rotation, Time.deltaTime * 3);
            }


        background.eulerAngles = new Vector3(-angle, background.eulerAngles.y, background.eulerAngles.z);
        // if camera reaches end of the groundObject block, the move ground object sequence is triggered
        if (transform.position.z >= groundObjectsList[0].transform.position.z + TrigoVector(groundObjectsList[0].transform.eulerAngles.x).z + offsetDistance)
        {
            MoveGroundObject();

            distance = 0.1f;
        }

    }


    /// <summary> Moving object sequence
    /// The current block is removed and added back at the end of the list, and moved to end position
    /// </summary>
    void MoveGroundObject()
    {
        GameObject go = groundObjectsList[0];

        MoveBlock(go.transform, groundObjectsList[groundObjectsList.Count - 1].transform.position + TrigoVector(groundObjectsList[groundObjectsList.Count - 1].transform.eulerAngles.x), new Vector3(go.transform.eulerAngles.x, go.transform.eulerAngles.y, go.transform.eulerAngles.z), offsetDistance);

        groundObjectsList.Remove(go);
        groundObjectsList.Add(go);
    }

    /// <summary> move block action
    /// </summary>
    void MoveBlock(Transform tr, Vector3 pos, Vector3 euler, float _offsetDistance)
    {

        Vector3 startCamPos = transform.position;

        background.SetParent(null);

        Vector3 startAngle = tr.eulerAngles;

        Vector3 startpos = pos + TrigoVector(euler.x) + Vector3.up * -maxGroundObjectInstance * 1f;
        Vector3 backStartPos = background.position;
        tr.position = startpos;
        tr.eulerAngles = euler;
        background.eulerAngles = euler;



        tr.position = Vector3.Lerp(startpos, pos, 1);

        background.position = Vector3.Lerp(backStartPos, pos + TrigoVector(euler.x), 1);
    }


    public Vector3 TrigoVector(float angle, float distance = 0)
    {
        if (distance == 0)
            return new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * -angle), Mathf.Cos(Mathf.Deg2Rad * -angle)) * distanceToSpawnNewGroundObject;
        else
            return new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * -angle), Mathf.Cos(Mathf.Deg2Rad * -angle)) * distance;

    }

    /// <summary> instanciate ground objects</summary>
    /// <param><c>index</c> number of ground objects to instanciate</param>
    void AddGroundObject(int index)
    {


        int randomIndex = Random.Range(0, groundPrefab.Length - 1);
        GameObject go = Instantiate<GameObject>(groundPrefab[randomIndex]);
        go.name = index.ToString();
        go.transform.position += Vector3.forward * distanceToSpawnNewGroundObject * (float)index;

        if (randomFlip && Random.value > 0.5f)
            go.transform.localScale = new Vector3(-1, 1, 1);
        else if (randomFlip)
            go.transform.localScale = new Vector3(1, 1, 1);

        groundObjectsList.Remove(go);
        groundObjectsList.Add(go);
    }
}
