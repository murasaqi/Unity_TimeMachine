using System.Collections.Generic;
using System.Linq;
using UnityEditor;
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
        public bool muteAll = false;
        public TimeMachineControlMixer timeMachineControlMixer
        {
            get { return mixer.GetBehaviour(); }
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            mixer = ScriptPlayable<TimeMachineControlMixer>.Create(graph, inputCount);
            mixer.GetBehaviour().clips = GetClips().ToList();
            mixer.GetBehaviour().playableDirector = go.GetComponent<PlayableDirector>();
            mixer.GetBehaviour().initialized = false;
            mixer.GetBehaviour().timeMachineControlTrack = this;
            return mixer;
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
                
            }
        }

        public void ForceMoveClip(int i)
        {
            Debug.Log($"move {i}");
            if(mixer.GetBehaviour() == null) return;
            mixer.GetBehaviour().ForceMoveClip(i);
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