using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    
    [CustomEditor(typeof(TimeMachineControlClip))]
    public class TimeMachineEditor:Editor
    {
        private TimeMachineControlClip timeMachineControlClip;
        private StringBuilder _stringBuilder = new StringBuilder(); 
        private Dictionary<string,TimelineClip> allClips = new Dictionary<string, TimelineClip>();
        private List<TimeMachineControlClip> _clips = new List<TimeMachineControlClip>();
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            timeMachineControlClip = (TimeMachineControlClip)target;
            
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sectionName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("sectionNameToDisplayName"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mute"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onClipStartAction"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("onClipEndAction"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isFinishOnStart"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("isFinishOnEnd"));
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("isSyncClip"));
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("clipIndex"));
            EditorGUI.EndDisabledGroup();
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(serializedObject.targetObject, "Change TimeMachineControlClip");
                serializedObject.ApplyModifiedProperties();
            }

            var isSyncClip = timeMachineControlClip.isSyncClip;
            var clipSelectionList = new List<string>();
            clipSelectionList.Add("None");
            allClips.Clear();
            var playableDirector = timeMachineControlClip.director;
            if (playableDirector != null)
            {
                var tracks = (playableDirector.playableAsset as TimelineAsset).GetOutputTracks().ToList();
                foreach (var track in tracks)
                {
                    if(track.GetType() == typeof(TimeMachineControlTrack)) continue;
                    var clips = track.GetClips().ToList();
                    foreach (var clip in clips)
                    {
                        if (clip.GetType() != typeof(TimeMachineControlClip))
                        {
                            _stringBuilder.Clear();
                            _stringBuilder.Append(tracks.IndexOf(track));
                            _stringBuilder.Append("_");
                            _stringBuilder.Append(clips.IndexOf(clip));
                            _stringBuilder.Append(" (");
                            _stringBuilder.Append(track.GetType());
                            _stringBuilder.Append(")");
                            _stringBuilder.Append(clip.displayName);
                            var selectName = _stringBuilder.ToString();
                            clipSelectionList.Add(selectName);    
                            if(!allClips.ContainsKey(selectName))allClips.Add(selectName, clip);
                        }
                    }
                }

            }

            var syncClip = timeMachineControlClip.syncClip;

            var currentIndex = 0;
            if (syncClip != null)
            {
                if (allClips.ContainsValue(syncClip))
                {
                    var key = allClips.FirstOrDefault(x => x.Value == syncClip).Key;
                    currentIndex = clipSelectionList.IndexOf(key);
                }
            }
            
            EditorGUI.BeginChangeCheck();
            var selectIndex = EditorGUILayout.Popup("Sync Clip",currentIndex,clipSelectionList.ToArray());
            if (EditorGUI.EndChangeCheck())
            {
                
                
                
                Undo.RecordObject(serializedObject.targetObject, "Change TimeMachineControlClip");
                if (selectIndex == 0)
                {
                    timeMachineControlClip.syncClip = null;
                    timeMachineControlClip.isSyncClip = false;
                }
                else
                {
                    timeMachineControlClip.isSyncClip = true;
                    // Debug.Log(selectClip.displayName);
                    timeMachineControlClip.syncClip = allClips[clipSelectionList[selectIndex]];

                }

                serializedObject.ApplyModifiedProperties();
            }
            
           
        }

    }
}