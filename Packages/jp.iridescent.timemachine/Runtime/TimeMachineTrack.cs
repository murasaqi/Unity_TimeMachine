using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    [TrackColor(255f / 255f, 255f / 255f, 94f / 255f)]
    [TrackClipType(typeof(TimeMachineClip))]
    [TrackBindingType(typeof(TimeMachineDirector))]
    public class TimeMachineTrack: TrackAsset
    {
        public bool muteClipRole;

        private ScriptPlayable<TimeMachineMixer> mixer;
        private TimeMachineDirector timeMachineDirector;

        public TimeMachineMixer timeMachineMixer => mixer.GetBehaviour();

        public TimeMachineDirector GetTimeMachineDirector()
        {
            return timeMachineDirector;
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            PlayableDirector playableDirector = go.GetComponent<PlayableDirector>();
            timeMachineDirector = playableDirector.GetGenericBinding(this) as TimeMachineDirector;
            mixer = ScriptPlayable<TimeMachineMixer>.Create(graph, inputCount);
            mixer.GetBehaviour().clips = GetClips().ToList();
            mixer.GetBehaviour().playableDirector = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().initialized = false;
            mixer.GetBehaviour().timeMachineTrack = this;
            return mixer;
        }

        public void Initialize()
        {
            foreach (TimelineClip clip in m_Clips)
            {
                TimeMachineClip timeMachineControlClip = clip.asset as TimeMachineClip;
                if (timeMachineControlClip != null) timeMachineControlClip.isFinishRole = false;
            }
        }

        public void ForceMoveClip(int i)
        {
            Debug.Log($"move {i}");
            if (mixer.GetBehaviour() == null) return;
            mixer.GetBehaviour().ForceMoveSection(i);
        }

        public void FinishRole(int index)
        {
            mixer.GetBehaviour().FinishRole(index);
        }


        public void FinishRoleCurrentClip()
        {
            mixer.GetBehaviour().FinishRole(mixer.GetBehaviour().GetCurrentInputIndex);
        }


        public void InitStatus()
        {
            if (mixer.GetBehaviour() != null) mixer.GetBehaviour().initialized = false;
        }
    }
}
