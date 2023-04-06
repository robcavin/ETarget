using System.Collections;
using System.Collections.Generic;
using Oculus.Interaction.Input;
using UnityEngine;

public class ExperimentManager : MonoBehaviour
{
    public GameObject centerEyeAnchor;
    public GameObject leftTrackedEye;
    public GameObject rightTrackedEye;
    public ExperimentDebug debugConsole;
    public BeamAnimator targetBeam;
    public GameObject gazeDebug;
    public GameObject targetController;
    public OVRHand handController;

    GameObject[] ETTargets;
    GameObject[] ControllerTargets;

    private float lastPinchTime;

    // Start is called before the first frame update
    void Start()
    {
        ETTargets = GameObject.FindGameObjectsWithTag("ETTarget");
        ControllerTargets = GameObject.FindGameObjectsWithTag("ControllerTarget");
        lastPinchTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Handle target size changes
        var thumbstick = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);

        bool updated = false;
        if (thumbstick.x > 0.5 || thumbstick.x < -0.5)
        {
            foreach (var target in ETTargets)
            {
                var currentScale = target.transform.localScale;
                target.transform.localScale = currentScale + thumbstick.x * new Vector3(0.001f, 0.001f, 0.0f);
                target.GetComponent<MeshRenderer>().enabled = true;
            }
            updated = true;
        }
        else
        {
            foreach (var target in ETTargets)
            {
                target.GetComponent<MeshRenderer>().enabled = false;
            }
        }


        if (thumbstick.y > 0.5 || thumbstick.y < -0.5)
        {
            foreach (var target in ControllerTargets)
            {
                var currentScale = target.transform.localScale;
                target.transform.localScale = currentScale + thumbstick.y * new Vector3(0.001f, 0.001f, 0.0f);
                target.GetComponent<MeshRenderer>().enabled = true;
            }
            updated = true;
        }
        else
        {
            foreach (var target in ControllerTargets)
            {
                target.GetComponent<MeshRenderer>().enabled = false;
            }
        }


        if (updated)
        {
            if (ETTargets.Length > 0) {// Update debug
                var depth = centerEyeAnchor.transform.TransformPoint(ETTargets[0].transform.position).z;
                var size = ETTargets[0].transform.localScale;
                var x_angle = Mathf.Atan2(size.x / 2, depth) * 2 * 180 / Mathf.PI;
                var y_angle = Mathf.Atan2(size.y / 2, depth) * 2 * 180 / Mathf.PI;
                debugConsole.ETTargetSize = new Vector2(x_angle, y_angle);
            }
            if (ControllerTargets.Length > 0) {
                // Update debug
                var depth = centerEyeAnchor.transform.TransformPoint(ControllerTargets[0].transform.position).z;
                var size = ControllerTargets[0].transform.localScale;
                var x_angle = Mathf.Atan2(size.x / 2, depth) * 2 * 180 / Mathf.PI;
                var y_angle = Mathf.Atan2(size.y / 2, depth) * 2 * 180 / Mathf.PI;
                debugConsole.ControllerTargetSize = new Vector2(x_angle, y_angle);
            }
        }


        // Find cyclopean gaze dir
        var ipd = (rightTrackedEye.transform.position - leftTrackedEye.transform.position).magnitude;
        var leftVector = leftTrackedEye.transform.localRotation * Vector3.forward;
        var rightVector = rightTrackedEye.transform.localRotation * Vector3.forward;
        var convergenceDistance = ipd / ((leftVector.x / leftVector.z - rightVector.x / rightVector.z) + 1e-9f);
        var convergence = (leftVector / leftVector.z * convergenceDistance + rightVector / rightVector.z * convergenceDistance) / 2;
        var cyclopeanCenter = centerEyeAnchor.transform.position;
        var cyclopeanVector = (centerEyeAnchor.transform.TransformPoint(convergence) - cyclopeanCenter).normalized;

        if (gazeDebug)
            gazeDebug.transform.position = cyclopeanCenter + cyclopeanVector * 10;

        GameObject eyesHit = null;
        GameObject controllerHit = null;

        float closest = float.MaxValue;
        foreach (var hit in Physics.RaycastAll(cyclopeanCenter, cyclopeanVector))
        {
            if ((hit.collider.tag == "ETTarget") && ((hit.collider.transform.position - hit.point).magnitude < closest))
                eyesHit = hit.collider.gameObject;
        }

        foreach (var hit in Physics.RaycastAll(targetController.transform.position, targetController.transform.forward))
        {
            if ((hit.collider.tag == "ControllerTarget") && ((hit.collider.transform.position - hit.point).magnitude < closest))
                controllerHit = hit.collider.gameObject;
        }

        if (handController.IsPointerPoseValid)
        {
            var handPointerPose = handController.PointerPose;
            foreach (var hit in Physics.RaycastAll(handPointerPose.position, handPointerPose.forward))
            {
                if ((hit.collider.tag == "ControllerTarget") && ((hit.collider.transform.position - hit.point).magnitude < closest))
                    controllerHit = hit.collider.gameObject;
            }
        }

        // Set interaction flag if we pressed the button or we pinched
        bool buttonPressed = OVRInput.GetDown(OVRInput.Button.One);
        bool isIndexFingerPinching = handController.GetFingerIsPinching(OVRHand.HandFinger.Index);
        OVRHand.TrackingConfidence confidence = handController.GetFingerConfidence(OVRHand.HandFinger.Index);
        if (isIndexFingerPinching && (confidence == OVRHand.TrackingConfidence.High))
        {
            float now = Time.time;
            if ((lastPinchTime - now) > 0.2)
                buttonPressed = true;
            lastPinchTime = now;
        }

        if (debugConsole.eyesRequired && debugConsole.controllerRequired)
        {
            if (eyesHit && controllerHit && (eyesHit.transform.parent == controllerHit.transform.parent))
            {
                targetBeam.gazeTarget = eyesHit.transform.position;
                if (buttonPressed)
                {
                    var test = eyesHit.transform.parent.GetComponentInChildren<KeyState>();
                    if (test) debugConsole.updateTypedString(test.key);
                }
            }
            else
                targetBeam.gazeTarget = Vector3.zero;
        }
        else if (debugConsole.eyesRequired)
        {
            if (eyesHit)
            {
                if (buttonPressed)
                {
                    var test = eyesHit.transform.parent.GetComponentInChildren<KeyState>();
                    if (test) debugConsole.updateTypedString(test.key);
                }
            }
        }
        else if (debugConsole.controllerRequired)
        {
            if (controllerHit)
            {
                if (buttonPressed)
                {
                    var test = controllerHit.transform.parent.GetComponentInChildren<KeyState>();
                    if (test) debugConsole.updateTypedString(test.key);
                }
            }
        }
    }
}
