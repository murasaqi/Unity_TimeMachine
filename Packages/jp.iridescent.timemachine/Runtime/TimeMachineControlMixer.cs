using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
using UnityEditor;

namespace Iridescent.TimeMachine
{
    public class TimeMachineControlMixer : PlayableBehaviour
    {
        internal List<TimelineClip> clips { get; set; }
        internal PlayableDirector playableDirector;
        private List<TimeMachineControlBehaviour> inputs = new List<TimeMachineControlBehaviour>();
        
        public int GetCurrentInputIndex => currentInputIndex;
        private TimeMachineControlBehaviour currentInput = null;
        public bool initialized = false;
        private TimeMachineTrackManager trackBinding = null;
        private int currentInputIndex = 0;


        public override void OnPlayableCreate(Playable playable)
        {
            initialized = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Debug.Log($"initialized: {initialized}");
            trackBinding = playerData as TimeMachineTrackManager;

            if (!trackBinding)
                return;

            if (!initialized)
            {
               

                var index = 0;
                foreach (var clip in clips)
                {
                    var c = clip.asset as TimeMachineControlClip;
                    c.clipIndex = index;
                    index++;
                }

                trackBinding.OnInit += InitEvents;
                trackBinding.OnForceMoveClip += ForceMoveClip;
                trackBinding.OnNextState += OnNextState;

                initialized = true;
            }

            double time = playableDirector.time;
            var i = 0;



            foreach (var clip in clips)
            {

                var inputPlayable = (ScriptPlayable<TimeMachineControlBehaviour>) playable.GetInput(i);
                var input = inputPlayable.GetBehaviour();
                var timeMachineControlClip = clip.asset as TimeMachineControlClip;
                
                if (timeMachineControlClip.mute) return;

                if (clip.start <= time && time < clip.start + clip.duration)
                {
                    // Debug.Log($"{clip.displayName}, {input.isFinishRole}");
                    currentInput = input;
                    currentInputIndex = i;
                    trackBinding.currentClipCount = currentInputIndex;
                    //  ======================= SKIP ========================= //
                    if (!timeMachineControlClip.isFinishRole && input.timeMachineClipEvent == TimeMachineClipEvent.SKIP)
                    {
                        // Debug.Log($"{clip.displayName}, SKIP");
                        playableDirector.time = clip.end - trackBinding.frameDuration;
                        timeMachineControlClip.isFinishRole = true;
                        break;

                    }

                }
                
                //  ======================= PAUSE ========================= //
                if (!timeMachineControlClip.isFinishRole && input.timeMachineClipEvent == TimeMachineClipEvent.PAUSE &&
                    time >= clip.end - trackBinding.frameDuration)
                {
                    // Debug.Log($"{clip.displayName}, PAUSE");
                    playableDirector.time = clip.end - trackBinding.frameDuration;
                    break;

                }

                //  ======================= LOOP ========================= //
                if (!timeMachineControlClip.isFinishRole && input.timeMachineClipEvent == TimeMachineClipEvent.LOOP &&
                    time >= clip.end - trackBinding.frameDuration)
                {
                    // Debug.Log($"{clip.displayName}, LOOP");
                    if (playableDirector.state == PlayState.Playing) playableDirector.time = clip.start;
                    break;
                }




                i++;

            }


        }

        public void InitEvents()
        {
            initialized = false;

            Debug.Log("init role");

            foreach (var clip in clips)
            {
                var timeMachineClip = clip.asset as TimeMachineControlClip;
                timeMachineClip.isFinishRole = false;
                // var c = clip as TimeMachineControlClip;
            }

        }




        public void ForceMoveClip(int index)
        {
            currentInputIndex = index;
            FinishRole(currentInputIndex-1);
            playableDirector.time = clips[index].start;

        }

        public void FinishRole(int index)
        {
            if(clips.Count <=index  && index < 0) return;
            
            var i = 0;
            foreach (var c in clips)
            {
                var timeMachineClip = c.asset as TimeMachineControlClip;
                timeMachineClip.isFinishRole = i <= index;
                i++;
            }

            i = 0;
            foreach (var c in clips)
            {
                Debug.Log($"{c.displayName},{(c.asset as TimeMachineControlClip).isFinishRole}");
                i++;
            }

            
        }

        public void OnNextState()

        {
            // currentInput.isFinishRole = true;
            playableDirector.time = clips[currentInputIndex].end;

        }
    }
}