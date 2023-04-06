using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ETTest : MonoBehaviour
{
    public GameObject leftTrackedEye;
    public GameObject rightTrackedEye;
    public GameObject centerTrackedEye;
    public BeamAnimator targetBeam;
    public GameObject gazeDebug;
    public GameObject targetController;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        var ipd = (rightTrackedEye.transform.position - leftTrackedEye.transform.position).magnitude;
        var leftVector = leftTrackedEye.transform.localRotation * Vector3.forward;
        var rightVector = rightTrackedEye.transform.localRotation * Vector3.forward;
        var convergenceDistance = ipd / (leftVector.x / leftVector.z - rightVector.x / rightVector.z);
        var convergence = (leftVector / leftVector.z * convergenceDistance + rightVector / rightVector.z * convergenceDistance) / 2;
        //var cyclopeanCenter = (rightTrackedEye.transform.position + leftTrackedEye.transform.position) / 2;
        var cyclopeanCenter = centerTrackedEye.transform.position;
        var cyclopeanVector = (centerTrackedEye.transform.TransformPoint(convergence) - cyclopeanCenter).normalized;

        gazeDebug.transform.position = cyclopeanCenter + cyclopeanVector * 10;

        bool eyesHit = false;
        bool controllerHit = false;

        RaycastHit info;
        var r = gameObject.transform.parent.GetComponent<Renderer>();
        r.material.color = Color.gray;
        if (Physics.Raycast(cyclopeanCenter, cyclopeanVector, out info))
        {
            info.collider.gameObject.GetComponent<Renderer>().material.color = Color.yellow;
            //if (info.collider == gameObject.GetComponent<Collider>())
            eyesHit = true;
        }

        if (Physics.Raycast(targetController.transform.position, targetController.transform.forward, out info))
        {
            //if (info.collider == gameObject.GetComponent<Collider>())
            controllerHit = true;
        }

        if (eyesHit && !controllerHit)
            gameObject.transform.parent.GetComponent<Renderer>().material.color = Color.blue;
        if (!eyesHit && controllerHit)
            gameObject.transform.parent.GetComponent<Renderer>().material.color = Color.red;
        if (eyesHit && controllerHit)
            gameObject.transform.parent.GetComponent<Renderer>().material.color = Color.green;

        if (eyesHit && controllerHit)
            targetBeam.gazeTarget = gameObject.transform.position;
        else
            targetBeam.gazeTarget = Vector3.zero;
    }
}
