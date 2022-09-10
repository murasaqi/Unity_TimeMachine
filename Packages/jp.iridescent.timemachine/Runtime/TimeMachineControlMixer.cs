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
        
      
       
        public bool initialized = false;
        private TimeMachineTrackManager trackBinding = null;
        private TimeMachineControlBehaviour currentInput = null;
        // private TimeMachineControlClip currentClip = null;
        private TimelineClip currentTimelineClip = null;
        private int currentInputIndex = 0;
        public int GetCurrentInputIndex => currentInputIndex;
        // public TimeMachineControlClip GetCurrentClip => currentClip;
        public TimelineClip GetCurrentTimelineClip => currentTimelineClip;
        public TimeMachineTrackManager timeMachineTrackManager => trackBinding;

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
                
                if (timeMachineControlClip.mute) continue;

                var isFinishOnStart = timeMachineControlClip.isFinishOnStart;
                var isFinishOnEnd = timeMachineControlClip.isFinishOnEnd;
                var onStartEvent = timeMachineControlClip.OnClipStartEvent;
                var onEndEvent = timeMachineControlClip.OnClipEndEvent;
                
                if (clip.start <= time && time < clip.start + clip.duration)
                {
                    currentInput = input;
                    currentTimelineClip = clip;
                    currentInputIndex = i;
                    trackBinding.currentClipCount = currentInputIndex;
                }
                
                //  ======================= SKIP ========================= //
                if (!isFinishOnStart && onStartEvent == TimeMachineClipEvent.SKIP)
                {
                    playableDirector.time = clip.end - trackBinding.frameDuration;
                    timeMachineControlClip.isFinishOnStart = true;
                    break;
                }
                if (!isFinishOnEnd &&  onEndEvent == TimeMachineClipEvent.SKIP &&
                    time >= clip.end - trackBinding.frameDuration)
                {
                    if (i < clips.Count)
                    {
                        playableDirector.time = clips[i + 1].start;     
                    }
                    timeMachineControlClip.isFinishOnEnd = true;
                    break;
                }
                
                //  ======================= PAUSE ========================= //
                if (!isFinishOnStart &&  onStartEvent == TimeMachineClipEvent.WAIT &&
                    time > clip.start)
                {
                    playableDirector.time = clip.start;
                    break;

                }

                
                if (!isFinishOnEnd &&  onEndEvent == TimeMachineClipEvent.WAIT &&
                    time >= clip.end - trackBinding.frameDuration)
                {
                    playableDirector.time = clip.end - trackBinding.frameDuration;
                    break;

                }

                //  ======================= LOOP ========================= //
                if (!isFinishOnEnd && onEndEvent == TimeMachineClipEvent.LOOP &&
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
                timeMachineClip.isFinishOnStart = false;
                timeMachineClip.isFinishOnEnd = false;
                // timeMachineClip.mute = false;
            }

        }




        public void ForceMoveClip(int index)
        {
            if(clips.Count == 0 || clips == null) return;
            currentInputIndex = index;
            FinishRole(currentInputIndex-1);
            playableDirector.time = clips[index].start;

        }

        public void FinishRole(int index, bool finishOnClipStart = true, bool finishOnClipEnd = true)
        {
            if(clips.Count <=index  && index < 0) return;
            
            var i = 0;
            foreach (var c in clips)
            {
                var timeMachineClip = c.asset as TimeMachineControlClip;
                if (i <= index)
                {
                    timeMachineClip.isFinishOnStart = finishOnClipStart;
                    timeMachineClip.isFinishOnEnd = finishOnClipEnd;
                }
                i++;
            }

            i = 0;
            foreach (var c in clips)
            {
                Debug.Log($"{c.displayName},{(c.asset as TimeMachineControlClip).isFinishOnStart},{(c.asset as TimeMachineControlClip).isFinishOnEnd}");
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