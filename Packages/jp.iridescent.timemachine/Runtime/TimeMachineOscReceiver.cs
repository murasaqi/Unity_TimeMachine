using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Iridescent.TimeMachine;
using UnityEditor;
using UnityEngine.Serialization;
#if USE_UOSC
using uOSC;
#endif

#if USE_EXTOSC
using extOSC;
#endif

#if UNITY_EDITOR
[CustomEditor(typeof(TimeMachineOscReceiver))]
[CanEditMultipleObjects]
public class TimeMachineOscReceiverEditor: Editor
{
    public override void OnInspectorGUI()
    {
        TimeMachineOscReceiver timeMachineOscReceiver = (TimeMachineOscReceiver)target;
        
        // change check
        EditorGUI.BeginChangeCheck();
#if USE_UOSC
        EditorGUILayout.PropertyField(serializedObject.FindProperty("uOscServer"));
#endif

#if USE_EXTOSC
        EditorGUILayout.PropertyField(serializedObject.FindProperty("extOscReceiver"));
#endif
        
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineTrackManager"));

        var isEnable = timeMachineOscReceiver.uOscServer != null &&
                       timeMachineOscReceiver.timeMachineTrackManager != null;
        
        EditorGUI.BeginDisabledGroup(!isEnable);
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.Space();
            if (GUILayout.Button("Initialize OSC Events", GUILayout.MaxWidth(200),GUILayout.Height(28)))
            {
                timeMachineOscReceiver.Init();
            }
            EditorGUILayout.Space();
        }
    
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("moveSectionAddressPrefix"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineOscMoveSectionEvents"));
       
        EditorGUILayout.PropertyField(serializedObject.FindProperty("playerEventAddressPrefix"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineOscPlayerEvents"));
        
        EditorGUI.EndDisabledGroup();

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(target, "TimeMachineExtOscReceiver");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif


public class TimeMachineOscReceiver : MonoBehaviour
{
#if USE_UOSC
    public uOscServer uOscServer;
#endif
#if USE_EXTOSC
    public OSCReceiver extOscReceiver;
#endif
    public TimeMachineTrackManager timeMachineTrackManager;
    
    [Header("---  Move Section Event  ---")]
    public string moveSectionAddressPrefix = "/TimeMachine/MoveTo";
    public List<TimeMachineOscMoveScetionEvent> timeMachineOscMoveSectionEvents = new List<TimeMachineOscMoveScetionEvent>();
    
    [Header("---  TimeMachine Player Event  ---")]
    public string playerEventAddressPrefix = "/TimeMachine/Player";
    public List<TimeMachineOscPlayerOscEvent> timeMachineOscPlayerEvents = new List<TimeMachineOscPlayerOscEvent>();


    private void Start()
    {
        Bind();
    }

    [ContextMenu("Init")]
    public void Init()
    {
        if(uOscServer == null) return;
        if(timeMachineTrackManager == null) return;
        
        timeMachineOscMoveSectionEvents.Clear();
        timeMachineOscPlayerEvents.Clear();

        var clips = timeMachineTrackManager.clips;
        
        if(clips == null) return;
        foreach (var clip in clips)
        {
            var timeMachineControlClip = clip.asset as TimeMachineControlClip;
            timeMachineOscMoveSectionEvents.Add(new TimeMachineOscMoveScetionEvent()
            {
                oscAddress = moveSectionAddressPrefix+"/"+timeMachineControlClip.sectionName,
                clipIndex = timeMachineControlClip.clipIndex,
                sectionName = clip.displayName,
            });
        }
        timeMachineOscPlayerEvents.Add(new TimeMachineOscPlayerOscEvent()
        {
            oscAddress = playerEventAddressPrefix+"/FinishCurrentRole",
            playerEvent = TimeMachinePlayerEventType.FinishCurrentRole,
        });
        timeMachineOscPlayerEvents.Add(new TimeMachineOscPlayerOscEvent()
        {
            oscAddress = playerEventAddressPrefix+"/ResetAndReplay",
            playerEvent = TimeMachinePlayerEventType.ResetAndReplay,
        });
        timeMachineOscPlayerEvents.Add(new TimeMachineOscPlayerOscEvent()
        {
            oscAddress = playerEventAddressPrefix+"/Stop",
            playerEvent = TimeMachinePlayerEventType.Stop,
        });
        
        
        timeMachineOscMoveSectionEvents.Sort((a, b) => a.clipIndex.CompareTo(b.clipIndex));
    }

    public void Bind()
    {
#if USE_UOSC
        BinduOscServer();
#endif

#if USE_EXTOSC
        BindExtOscServer();
#endif
    }
    
    #if USE_EXTOSC
    public void BindExtOscServer()
    {
        if(extOscReceiver == null) return;
        foreach (var timeMachineOscEvent in timeMachineOscMoveSectionEvents)
        {
            extOscReceiver.Bind(timeMachineOscEvent.oscAddress, (message) =>
            {
                var sectionName = timeMachineOscEvent.sectionName;
                timeMachineTrackManager.MoveClip(sectionName);    
                
            });
        }
        
        foreach (var timeMachineOscEvent in timeMachineOscPlayerEvents)
        {
            extOscReceiver.Bind(timeMachineOscEvent.oscAddress, (message) =>
            {
                switch (timeMachineOscEvent.playerEvent)
                {
                    case TimeMachinePlayerEventType.FinishCurrentRole:
                        timeMachineTrackManager.FinishRole();
                        break;
                    case TimeMachinePlayerEventType.ResetAndReplay:
                        timeMachineTrackManager.ResetAndReplay();
                        break;
                    case TimeMachinePlayerEventType.Stop:
                        timeMachineTrackManager.Stop();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
            });
        }
    }
    #endif
    
#if USE_UOSC
    
    public void BinduOscServer()
    {
        if(uOscServer == null) return;
        uOscServer.onDataReceived.AddListener(OnMoveSectionReceived);
        uOscServer.onDataReceived.AddListener(OnPlayerControlReceived);  
    }
    
#endif

    public void OnMoveSectionReceived(Message message)
    {
        foreach (var timeMachineOscEvent in timeMachineOscMoveSectionEvents.Where(timeMachineOscEvent => string.Equals(message.address, timeMachineOscEvent.oscAddress)))
        {
            var sectionName = timeMachineOscEvent.sectionName;
            timeMachineTrackManager.MoveClip(sectionName);
        }
    }
    
    public void OnPlayerControlReceived(Message message)
    {
        foreach (var timeMachineOscEvent in timeMachineOscPlayerEvents.Where(timeMachineOscEvent => string.Equals(message.address, timeMachineOscEvent.oscAddress)))
        {
            switch (timeMachineOscEvent.playerEvent)
            {
                case TimeMachinePlayerEventType.FinishCurrentRole:
                    timeMachineTrackManager.FinishRole();
                    break;
                case TimeMachinePlayerEventType.ResetAndReplay:
                    timeMachineTrackManager.ResetAndReplay();
                    break;
                case TimeMachinePlayerEventType.Stop:
                    timeMachineTrackManager.Stop();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

}


