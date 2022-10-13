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
        public UnityEvent onClipStartEvent;
        public UnityEvent onClipEndEvent;
        
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

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeMachineControlBehaviour>.Create(graph, timeMachineControlBehaviour);
            behaviour = playable.GetBehaviour();

            FindSyncClip();
            
            // behaviour.timeMachineClipOnStartEvent = timeMachineClipOnStartEvent;
            // behaviour.timeMachineClipOnEndEvent = timeMachineClipOnEndClipEvent;
            // behaviour.isFinishRole = isFinishRole;
            // behaviour.mute = mute;
            return playable;
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