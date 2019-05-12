using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR 
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraRig))]
public class CameraRigEditor : Editor {

    CameraRig cameraRig;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        cameraRig = (CameraRig)target;
        EditorGUILayout.LabelField("Camera Helper");
        if (GUILayout.Button("Save camera's position and Euler now."))
        {
            Camera cam = Camera.main;

            if (cam)
            {
                Transform camT = cam.transform;
                Vector3 camPos = camT.localPosition;
                Vector3 camRight = camPos;
                Vector3 camLeft = camPos;
                camLeft.x = -camPos.x;
                cameraRig.cameraSettings.camPositionOffsetRight = camRight;
                cameraRig.cameraSettings.camPositionOffsetLeft = camLeft;
                cameraRig.cameraSettings.camEulerOffset = camT.eulerAngles;
            }
        }
    }
}
#endif
