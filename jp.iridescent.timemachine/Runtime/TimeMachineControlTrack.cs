using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    [TrackColor(177f / 255f, 142f / 255f, 255f / 255f)]
    [TrackClipType(typeof(TimeMachineControlClip))]
    [TrackBindingType(typeof(TimeMachineTrackManager))]
    public class TimeMachineControlTrack : TrackAsset
    {

        private ScriptPlayable<TimeMachineControlMixer> mixer;
        // public bool forceClipLayout = false;
        public float clipMargin = 3f;
     
        public TimeMachineControlMixer timeMachineControlMixer
        {
            get { return mixer.GetBehaviour(); }
        }

        
        private Dictionary<string,TimelineClip> allClips = new Dictionary<string, TimelineClip>();
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            mixer = ScriptPlayable<TimeMachineControlMixer>.Create(graph, inputCount);
            mixer.GetBehaviour().clips = GetClips().ToList();
            mixer.GetBehaviour().playableDirector = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().initialized = false;
            mixer.GetBehaviour().timeMachineControlTrack = this;

            var director = go.GetComponent<PlayableDirector>();
            
            
            if (m_Clips != null)
            {
                foreach (var clip in m_Clips)
                {
                    var timeMachineCLip = clip.asset as TimeMachineControlClip;
                    // clip.displayName = timeMachineCLip.sectionName;
                    if(timeMachineCLip == null) continue;
                    timeMachineCLip.clipIndex = m_Clips.IndexOf(clip);
                    timeMachineCLip.mixer = mixer.GetBehaviour();
                    timeMachineCLip.director = director;
                }
            }
            return mixer;
        }

        private int GetSameSectionCount(string sectionName)
        {
            var sameNameCount = 0;
            foreach (var c in m_Clips)
            {
                var asset = c.asset as TimeMachineControlClip;
                if (asset != this)
                {
                    if (asset.sectionName == sectionName)
                    {
                        sameNameCount++;
                    }
                }
            }

            return sameNameCount;
        }
        
        public void Initialize()
        {
            foreach (var clip in m_Clips)
            {
                var timeMachineControlClip = clip.asset as TimeMachineControlClip;
                if (timeMachineControlClip != null)
                {
                    timeMachineControlClip.isFinishOnStart = false;
                    timeMachineControlClip.isFinishOnEnd = false;
                }
                timeMachineControlClip.FindSyncClip();
            }
        }

        public void ForceMoveClip(int i)
        {
            Debug.Log($"move {i}");
            if(mixer.GetBehaviour() == null) return;
            mixer.GetBehaviour().ForceMoveClip(i);
        }
        
        public void ForceMoveClip(string sectionName)
        {
            Debug.Log($"move {sectionName}");
            if(mixer.GetBehaviour() == null) return;

            foreach (var clip in m_Clips)
            {
                var timeMachineControlClip = clip.asset as TimeMachineControlClip;
                if (timeMachineControlClip != null)
                {
                    if (timeMachineControlClip.sectionName == sectionName)
                    {
                        mixer.GetBehaviour().ForceMoveClip(timeMachineControlClip.clipIndex);
                        return;
                    }
                }
            }
        }

        public void AutoLayoutClips(float marginDuration)
        {
            var startDuration = 0d;

            foreach (var clip in m_Clips)
            {
                clip.start = startDuration;
                startDuration += clip.duration + marginDuration;
            }

#if UNITY_EDITOR
            // set dirty
            UnityEditor.EditorUtility.SetDirty(timelineAsset);
            // save assets
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }

        public void FinishRole(int index)
        {
            mixer.GetBehaviour().FinishRole(index);
        }

        
        public void FinishRoleCurrentClip()
        {
            mixer.GetBehaviour().FinishCurrentRole();
        }


        public void InitStatus()
        {
            if (mixer.GetBehaviour() != null) mixer.GetBehaviour().initialized = false;
        }

        public void CreateClipsAccordingTo(string targetTrackName, double clipMargin)
        {
            if(string.IsNullOrEmpty(targetTrackName)) return;

            if (GetClips().Count() > 0)
            {
                Debug.LogError("Delete all clips before create new clips");
            }

            var tracks = this.timelineAsset.GetOutputTracks();
            var targetTrack = tracks.FirstOrDefault(t => t.name == targetTrackName);

            if (targetTrack == null)
            {
                Debug.LogWarning("Target track not found");
                return;
            }

            var targetClips = targetTrack?.GetClips();
            for (int i = 0; i < targetClips.Count(); i++)
            {
                var tClip = targetClips.ElementAt(i);
                var clip = this.CreateClip<TimeMachineControlClip>();
                clip.start = tClip.start;
                var d = tClip.duration - clipMargin;
                clip.duration = d > 0 ? d : 0.1;
                clip.displayName = tClip.displayName;
                var tmClip = (TimeMachineControlClip)clip.asset;
                tmClip.clipIndex = i;
                tmClip.sectionName = clip.displayName;
                tmClip.onClipEndAction = TimeMachineClipEvent.WAIT;
                Debug.Log($"created : {tClip.displayName}");
            }
#if UNITY_EDITOR
            // set dirty
            UnityEditor.EditorUtility.SetDirty(timelineAsset);
            // save assets
            UnityEditor.AssetDatabase.SaveAssets();
#endif
        }
    }
}