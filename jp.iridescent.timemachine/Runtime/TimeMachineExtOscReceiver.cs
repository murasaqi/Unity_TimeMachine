// using System;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using Iridescent.TimeMachine;
// using UnityEditor;
// #if USE_EXTOSC
// using extOSC;
//
//
//
//
// // Custom Editor TimeMachineOscReciver
// #if UNITY_EDITOR
// [CustomEditor(typeof(TimeMachineExtOscReceiver))]
// [CanEditMultipleObjects]
// public class TimeMachineExtOscReceiverEditor: Editor
// {
//     public override void OnInspectorGUI()
//     {
//         TimeMachineExtOscReceiver timeMachineExtOscReceiver = (TimeMachineExtOscReceiver)target;
//         
//         // change check
//         EditorGUI.BeginChangeCheck();
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("oscReceiver"));
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineTrackManager"));
//
//         var isEnable = timeMachineExtOscReceiver.oscReceiver != null &&
//                        timeMachineExtOscReceiver.timeMachineTrackManager != null;
//         
//         EditorGUI.BeginDisabledGroup(!isEnable);
//         EditorGUILayout.Space();
//         
//         EditorGUILayout.BeginHorizontal();
//         {
//             EditorGUILayout.Space();
//             if (GUILayout.Button("Initialize OSC Events", GUILayout.MaxWidth(200),GUILayout.Height(28)))
//             {
//                 timeMachineExtOscReceiver.Init();
//             }
//             EditorGUILayout.Space();
//         }
//     
//         EditorGUILayout.EndHorizontal(); 
//         EditorGUILayout.Space();
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("oscAddressPrefix"));
//         EditorGUILayout.PropertyField(serializedObject.FindProperty("timeMachineOscEvents"));
//        
//         
//         EditorGUI.EndDisabledGroup();
//
//         if (EditorGUI.EndChangeCheck())
//         {
//             
//             // Record undo
//             Undo.RecordObject(target, "TimeMachineExtOscReceiver");
//             serializedObject.ApplyModifiedProperties();
//         }
//     }
// }
// #endif
//
// public class TimeMachineExtOscReceiver : MonoBehaviour
// {
//     public string oscAddressPrefix = "/TimeMachine/MoveTo";
//     public TimeMachineTrackManager timeMachineTrackManager;
//     public List<TimeMachineOscEvent> timeMachineOscEvents = new List<TimeMachineOscEvent>();
//     public OSCReceiver oscReceiver;
//     private void Start()
//     {
//         Bind();
//     }
//
//     private void Update()
//     {
//         
//     }
//
//     [ContextMenu("Init")]
//     public void Init()
//     {
//         if(oscReceiver == null) return;
//         if(timeMachineTrackManager== null) return;
//         
//         timeMachineOscEvents.Clear();
//        
//
//         var clips = timeMachineTrackManager.clips;
//         
//         if(clips == null) return;
//         foreach (var clip in clips)
//         {
//             var timeMachineControlClip = clip.asset as TimeMachineControlClip;
//             timeMachineOscEvents.Add(new TimeMachineOscEvent()
//             {
//                 oscAddress = oscAddressPrefix+"/C"+timeMachineControlClip.clipIndex,
//                 clipIndex = timeMachineControlClip.clipIndex,
//                 clipName = clip.displayName,
//                 // oscValue = "C"+timeMachineControlClip.clipIndex.ToString()
//             });
//         }
//         timeMachineOscEvents.Add(new TimeMachineOscEvent()
//         {
//             oscAddress = oscAddressPrefix+"/Finish",
//             clipIndex = -1,
//             clipName = "Finish Current Role",
//             // oscValue = "Finish"
//         });
//         
//         
//         timeMachineOscEvents.Sort((a, b) => a.clipIndex.CompareTo(b.clipIndex));
//     }
//
//     [ContextMenu("Bind")]
//     public void Bind()
//     {
//
//
//         foreach (var timeMachineOscEvent in timeMachineOscEvents)
//         {
//             oscReceiver.Bind(timeMachineOscEvent.oscAddress, (message) =>
//             {
//                 // var value = message.FindValues(OSCValueType.String).First().Value as string;
//                 var index = timeMachineOscEvent.clipIndex;
//                 if (index == -1)
//                 {
//                     timeMachineTrackManager.ResetAndReplay();
//                 }
//                 else
//                 {
//                     timeMachineTrackManager.MoveClip(index);    
//                 }
//             });
//         }
//
//         // oscReceiver.Bind(oscAddress, (message) =>
//         // {
//         //     
//         //     var value = message.FindValues(OSCValueType.String).First().Value as string;
//         //     
//         //     if (value != null)
//         //     {
//         //         if (value == "Finish")
//         //         {
//         //             timeMachineTrackManager.FinishCurrentClip();
//         //         }
//         //         else
//         //         {
//         //             foreach (var timeMachineOscEvent in timeMachineOscEvents)
//         //             {
//         //                 if (value == timeMachineOscEvent.oscValue)
//         //                 {
//         //                     var index = timeMachineOscEvent.clipIndex;
//         //                     timeMachineTrackManager.MoveClip(index);     
//         //                 }
//         //            
//         //             }   
//         //         }
//         //          
//         //     }
//         //     
//         // });
//         
//     }
//
// }
//
// #endif
