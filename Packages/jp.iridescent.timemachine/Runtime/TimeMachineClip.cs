using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    public class TimeMachineClip: PlayableAsset, ITimelineClipAsset
    {

        [SerializeField] [HideInInspector] public TimeMachineBehaviour timeMachineBehaviour;
        [SerializeField] public string sectionName;
        // [SerializeField] public bool mute;
        [SerializeField] public TimeMachineClipEvent timeMachineClipEvent = TimeMachineClipEvent.THOROUGH;
        [SerializeField] public bool isFinishRole;
        [SerializeField] public int clipIndex;
        private TimeMachineBehaviour behaviour;
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            ScriptPlayable<TimeMachineBehaviour> playable = ScriptPlayable<TimeMachineBehaviour>.Create(graph, timeMachineBehaviour);
            behaviour = playable.GetBehaviour();
            // behaviour.timeMachineClipEvent = timeMachineClipEvent;
            // behaviour.isFinishRole = isFinishRole;
            // behaviour.mute = mute;
            return playable;
        }
    }
}
