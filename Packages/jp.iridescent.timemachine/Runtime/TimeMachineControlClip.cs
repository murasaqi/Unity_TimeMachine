using System;
using System.Collections.Generic;
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
        [SerializeField] public bool sectionNameToDisplayName = false;
        [SerializeField] public bool mute;
        [SerializeField] public TimeMachineClipEvent onClipStartAction = TimeMachineClipEvent.THOROUGH;
        [SerializeField] public TimeMachineClipEvent onClipEndAction = TimeMachineClipEvent.THOROUGH;
       
        internal bool isFireOnClipStart =false;
        internal bool isFireOnClipEnd = false;
        [SerializeField] public bool isFinishOnStart = false;
        [SerializeField] public bool isFinishOnEnd = false;
        [SerializeField] public int clipIndex= 0;
        
        
        public bool isSyncClip = false;
        // public Dictionary<string, TimelineClip> allClipDict = new Dictionary<string, TimelineClip>();
        public TimelineClip syncClip = null;
        
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
        private TimeMachineControlBehaviour behaviour;
        public TimeMachineControlMixer mixer;
        public PlayableDirector director;
        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeMachineControlBehaviour>.Create(graph, timeMachineControlBehaviour);
            behaviour = playable.GetBehaviour();
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
            // if (syncTimelineAsset != null)
            // {
            //     var track = syncTimelineAsset.GetOutputTracks().ToList().Find(t=> t.name == syncTrackTargetName);
            //     if (track != null)
            //     {
            //         var clips = track.GetClips().ToList();
            //         syncClip = clips.Find(c => c.displayName == syncClipTargetName);        
            //     }
            //
            // }
        }
        
        


    }
}