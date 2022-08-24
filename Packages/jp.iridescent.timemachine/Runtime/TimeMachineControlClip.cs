using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    public class TimeMachineControlClip : PlayableAsset,ITimelineClipAsset
    {

        [SerializeField, HideInInspector] public TimeMachineControlBehaviour timeMachineControlBehaviour;
        [SerializeField] public string sectionName;
        [SerializeField] public bool mute;
        [SerializeField] public TimeMachineClipEvent timeMachineClipEvent = TimeMachineClipEvent.THOROUGH;
        [SerializeField] public bool isFinishRole = false;
        [SerializeField, ] public int clipIndex= 0;
        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }
        private TimeMachineControlBehaviour behaviour;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<TimeMachineControlBehaviour>.Create(graph, timeMachineControlBehaviour);
            behaviour = playable.GetBehaviour();
            behaviour.timeMachineClipEvent = timeMachineClipEvent;
            // behaviour.isFinishRole = isFinishRole;
            // behaviour.mute = mute;
            return playable;
        }
        


    }
}