using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using Object = System.Object;

namespace Iridescent.TimeMachine
{
    public class TimeMachineControlClip : PlayableAsset,ITimelineClipAsset
    {

        [SerializeField, HideInInspector] public TimeMachineControlBehaviour timeMachineControlBehaviour;
        [SerializeField] public string sectionName;
        [SerializeField] public bool mute;
        [SerializeField] public TimeMachineClipEvent onClipStartAction = TimeMachineClipEvent.THOROUGH;
        [SerializeField] public TimeMachineClipEvent onClipEndAction = TimeMachineClipEvent.THOROUGH;
       
        public bool isFireOnClipStart =false;
        public bool isFireOnClipEnd = false;
        [SerializeField] public bool isFinishOnStart = false;
        [SerializeField] public bool isFinishOnEnd = false;
        [SerializeField, ] public int clipIndex= 0;
        
        
        public bool isSyncClip = false;
        public TimelineAsset syncTimelineAsset;
        public string syncTrackTargetName = null;
        public string syncClipTargetName = null;
        public TimelineClip syncClip = null;
        
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
        private TimeMachineControlBehaviour behaviour;
        public TimeMachineControlMixer mixer;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeMachineControlBehaviour>.Create(graph, timeMachineControlBehaviour);
            behaviour = playable.GetBehaviour();

            
            FindSyncClip();

            // if (sectionName == null || sectionName == "")
            // {
            //     sectionName = "Section_"+clipIndex;
            //     // CheckSameSectionName();
            // }
           
            return playable;
        }

        private void CheckSameSectionName()
        {
            var sameNameCount = 0;
            foreach (var c in mixer.clips)
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
            
            
            sectionName = sameNameCount == 0 ?  sectionName: $"{sectionName} ({sameNameCount})";
        }

        
        [ContextMenu("FindSyncClip")]
        public void FindSyncClip()
        {
            if (syncTimelineAsset != null)
            {
                var track = syncTimelineAsset.GetOutputTracks().ToList().Find(t=> t.name == syncTrackTargetName);
                if (track != null)
                {
                    var clips = track.GetClips().ToList();
                    syncClip = clips.Find(c => c.displayName == syncClipTargetName);        
                }
            
            }
        }
        
        


    }
}