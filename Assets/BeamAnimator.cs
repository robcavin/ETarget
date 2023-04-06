using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAnimator : MonoBehaviour
{
    private GameObject boneOrigin;
    private List<GameObject> bones;
    public Vector3 gazeTarget;

    public GameObject testObject;

    // Start is called before the first frame update
    void Start()
    {
        boneOrigin = GameObject.Find("beam12/Armature/Bone");
        bones = new List<GameObject>(10);
        var currentBone = boneOrigin;
        for (int i = 0; i < 10; i++)
        {
            bones.Add(currentBone.transform.GetChild(0).gameObject);
            currentBone = bones[bones.Count - 1];
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if (testObject)
        //    gazeTarget = testObject.transform.position;

        //gameObject.SetActive(gazeTarget != Vector3.zero);

        List<float> elements = new List<float> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        for (int i = 0; i < elements.Count; i++)
            elements[i] = elements[i] * elements[i];
        float elementsSum = 0;
        for (int i = 0; i < elements.Count; i++)
            elementsSum += elements[i];


        Vector3 target = boneOrigin.transform.InverseTransformPoint(gazeTarget);

        var x_elemSize = target.x / elementsSum;
        var y_elemSize = target.y / 10;
        var z_elemSize = target.z / elementsSum;

        if (gazeTarget == Vector3.zero)
        {
            x_elemSize = 0;
            y_elemSize = 0.5f;
            z_elemSize = 0;
        }

        var lastPostiion = new Vector3(0,0,0);
        List<Vector3> localPositions = new List<Vector3>();
        for (int i = 0; i < elements.Count; i++)
        {
            localPositions.Add(
                lastPostiion + new Vector3(x_elemSize * elements[i], y_elemSize, z_elemSize * elements[i]));
            lastPostiion = localPositions[localPositions.Count - 1];
        }

        for (int i = 0; i < elements.Count; i++)
        {
            bones[i].transform.position = boneOrigin.transform.TransformPoint(localPositions[i]);
        }

        List<Vector3> finalPositions = new List<Vector3>();
        for (int i = 0; i < elements.Count; i++)
        {
            finalPositions.Add(bones[i].transform.position);
        }

        Debug.Log("Here");

       
    }
}
