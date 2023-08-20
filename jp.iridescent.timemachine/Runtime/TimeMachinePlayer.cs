using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;


namespace Iridescent.TimeMachine
{
    [RequireComponent(typeof(PlayableDirector))]
    public class TimeMachinePlayer : MonoBehaviour
    {
        // Start is called before the first frame update
        public TimeMachineTrackManager timeMachineTrackManager;
        public List<PlayableDirector> playableDirectors = new List<PlayableDirector>();
        public bool exportTimelineAsset = false;
        public string exportPath = "Assets/TimeMachineTimeline.asset";
        
        public TimelineAsset timelineAsset;
        [FormerlySerializedAs("playableDirector")]
        public PlayableDirector timeMachinePlayableDirector;

        void Start()
        {

        }

        [ContextMenu("Create TimelineAsset")]
        public void CreateTimelineAsset()
        {
            if (timeMachinePlayableDirector == null)
            {
                timeMachinePlayableDirector = GetComponent<PlayableDirector>();
                if (timeMachinePlayableDirector == null)
                    timeMachinePlayableDirector = gameObject.AddComponent<PlayableDirector>();
            }

            ReleaseTimelineAsset();
            timelineAsset = ScriptableObject.CreateInstance<TimelineAsset>();
            timelineAsset.name = "TimeMachineTimeline";
            timelineAsset.editorSettings.fps = 60;
            // save asset and export timelineAsset

#if UNITY_EDITOR
            if (exportTimelineAsset)
            {
                ExportTimelineAsset();
                // LoadTimelineAsset();

            }
#endif


            CreateTrackAndClips();




        }
#if UNITY_EDITOR
        public void ExportTimelineAsset()
        {

            if (timelineAsset == null) return;
            UnityEditor.AssetDatabase.CreateAsset(timelineAsset, exportPath);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }
#endif
        
#if UNITY_EDITOR
        public void LoadTimelineAsset()
        {
            ReleaseTimelineAsset();
            timelineAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<TimelineAsset>(exportPath);
            if (timelineAsset == null)
            {
                Debug.LogError("TimelineAsset is not found");
                return;
            }

            CreateTrackAndClips();
        }
#endif
        public void CreateTrackAndClips()
        {
            double startTime = 0f;
            var controlTrack = timelineAsset.CreateTrack<ControlTrack>(null, "ControlTrack");
            var timeMachineTrack = timelineAsset.CreateTrack<TimeMachineControlTrack>(null, "TimeMachineTrack");
            // set binding
            if (timeMachineTrackManager == null) return;
            timeMachinePlayableDirector.SetGenericBinding(timeMachineTrack, timeMachineTrackManager);
            // make ControlTrack and add it to control clip
            foreach (var playableDirector in playableDirectors)
            {
                if (playableDirector == null) continue;
                var controlClip = controlTrack.CreateDefaultClip();
                controlClip.displayName = playableDirector.playableAsset.name;
                controlClip.duration = playableDirector.duration;
                controlClip.start = startTime;
                controlClip.timeScale = 1;
                controlClip.blendInDuration = 0;
                controlClip.blendOutDuration = 0;
                controlClip.easeInDuration = 0;
                controlClip.easeOutDuration = 0;
                var controlPlayableAsset = controlClip.asset as ControlPlayableAsset;
                controlPlayableAsset.sourceGameObject = new ExposedReference<GameObject>()
                {
                    defaultValue = playableDirector.gameObject,
                    exposedName = playableDirector.gameObject.name
                };

                var timeMachineClip = timeMachineTrack.CreateDefaultClip();
                timeMachineClip.displayName = playableDirector.playableAsset.name;
                timeMachineClip.duration = playableDirector.duration;
                timeMachineClip.start = startTime;

                startTime += playableDirector.duration;
            }

            timeMachinePlayableDirector.playableAsset = timelineAsset;

            //set dirty
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(timelineAsset);
            UnityEditor.EditorUtility.SetDirty(timeMachinePlayableDirector);
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
#endif

        }



        public void ReleaseTimelineAsset()
        {
            if (timelineAsset != null)
            {
                DestroyImmediate(timelineAsset);
            }

            timelineAsset = null;
        }

        private void OnDestroy()
        {
            ReleaseTimelineAsset();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}