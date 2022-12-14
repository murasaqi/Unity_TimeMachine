using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iridescent.TimeMachine;
using UnityEditor;
#if USE_UOSC
using uOSC;


#if UNITY_EDITOR
[CustomEditor(typeof(TimeMachineUOscReceiver))]
[CanEditMultipleObjects]
public class TimeMachineUOscReceiverEditor: Editor
{
    public override void OnInspectorGUI()
    {
        TimeMachineUOscReceiver timeMachineExtOscReceiver = (TimeMachineUOscReceiver)target;
        
        // change check
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oscServer"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineTrackManager"));

        var isEnable = timeMachineExtOscReceiver.oscServer != null &&
                       timeMachineExtOscReceiver.timeMachineTrackManager != null;
        
        EditorGUI.BeginDisabledGroup(!isEnable);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Initialize OSC Events", GUILayout.MaxWidth(200),GUILayout.Height(28)))
            {
                timeMachineExtOscReceiver.Init();
            }
            EditorGUILayout.Space();
        }
    
        EditorGUILayout.EndHorizontal(); 
        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("oscAddressPrefix"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineOscEvents"));
       
        
        EditorGUI.EndDisabledGroup();

        if (EditorGUI.EndChangeCheck())
        {
            
            // Record undo
            Undo.RecordObject(target, "TimeMachineExtOscReceiver");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif


public class TimeMachineUOscReceiver : MonoBehaviour
{
    public string oscAddressPrefix = "/TimeMachine/MoveTo";
    public TimeMachineTrackManager timeMachineTrackManager;
    public List<TimeMachineOscEvent> timeMachineOscEvents = new List<TimeMachineOscEvent>();
    public uOscServer oscServer;

    private void Start()
    {
        Bind();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if(oscServer == null) return;
        if(timeMachineTrackManager == null) return;
        
        timeMachineOscEvents.Clear();
       

        var clips = timeMachineTrackManager.clips;
        
        if(clips == null) return;
        foreach (var clip in clips)
        {
            var timeMachineControlClip = clip.asset as TimeMachineControlClip;
            timeMachineOscEvents.Add(new TimeMachineOscEvent()
            {
                oscAddress = oscAddressPrefix+"/C"+timeMachineControlClip.clipIndex,
                clipIndex = timeMachineControlClip.clipIndex,
                clipName = clip.displayName,
            });
        }
        timeMachineOscEvents.Add(new TimeMachineOscEvent()
        {
            oscAddress = oscAddressPrefix+"/Finish",
            clipIndex = -1,
            clipName = "Finish Current Role"
        });
        
        
        timeMachineOscEvents.Sort((a, b) => a.clipIndex.CompareTo(b.clipIndex));
    }

    public void Bind()
    {
        oscServer.onDataReceived.AddListener(OnDataReceived);
    }

    public void OnDataReceived(Message message)
    {
        foreach (var timeMachineOscEvent in timeMachineOscEvents.Where(timeMachineOscEvent => string.Equals(message.address, timeMachineOscEvent.oscAddress)))
        {
            var index = timeMachineOscEvent.clipIndex;
            if (index == -1)
            {
                timeMachineTrackManager.FinishCurrentClip();
            }
            else
            {
                timeMachineTrackManager.MoveClip(index);    
            }
        }
    }

}

#endif
