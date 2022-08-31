using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    public class TimeMachineMixer: PlayableBehaviour
    {
        private TimeMachineBehaviour currentInput;
        // private TimeMachineControlClip currentClip = null;



        public bool initialized;
        private List<TimeMachineBehaviour> inputs = new List<TimeMachineBehaviour>();
        internal PlayableDirector playableDirector;
        internal TimeMachineTrack timeMachineTrack;
        internal List<TimelineClip> clips { get; set; }
        public int GetCurrentInputIndex { get; private set; }
        // public TimeMachineControlClip GetCurrentClip => currentClip;
        public TimelineClip GetCurrentTimelineClip { get; private set; }
        public TimeMachineDirector TimeMachineDirector { get; private set; }

        public override void OnPlayableCreate(Playable playable)
        {
            initialized = false;
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            // Debug.Log($"initialized: {initialized}");
            TimeMachineDirector = playerData as TimeMachineDirector;

            if (!TimeMachineDirector || timeMachineTrack)
                return;

            if (!initialized)
            {


                int index = 0;
                foreach (TimelineClip clip in clips)
                {
                    TimeMachineClip c = clip.asset as TimeMachineClip;
                    c.clipIndex = index;
                    index++;
                }

                TimeMachineDirector.OnInit += InitEvents;
                TimeMachineDirector.OnForceMoveSection += ForceMoveSection;
                // trackBinding.OnNextState += OnNextState;

                initialized = true;
            }

            double time = playableDirector.time;
            int i = 0;



            foreach (TimelineClip clip in clips)
            {

                Debug.Log(clip.displayName);
                ScriptPlayable<TimeMachineBehaviour> inputPlayable = (ScriptPlayable<TimeMachineBehaviour>)playable.GetInput(i);
                TimeMachineBehaviour input = inputPlayable.GetBehaviour();
                TimeMachineClip timeMachineClip = clip.asset as TimeMachineClip;

                bool isMuteOrFinishRole = timeMachineTrack.muteClipRole || timeMachineClip.isFinishRole;
                // if (timeMachineClip.mute) return;

                if (clip.start <= time && time < clip.start + clip.duration)
                {
                    // Debug.Log($"{clip.displayName}, {input.isFinishRole}");
                    currentInput = input;
                    // currentClip = timeMachineControlClip;
                    GetCurrentTimelineClip = clip;
                    GetCurrentInputIndex = i;
                    TimeMachineDirector.currentClipCount = GetCurrentInputIndex;
                    //  ======================= SKIP ========================= //
                    if (!isMuteOrFinishRole && timeMachineClip.timeMachineClipEvent == TimeMachineClipEvent.SKIP)
                    {
                        Debug.Log($"{clip.displayName}, SKIP");
                        playableDirector.time = clip.end - TimeMachineDirector.frameDuration;
                        timeMachineClip.isFinishRole = true;
                        break;

                    }

                }

                //  ======================= PAUSE ========================= //
                if (!isMuteOrFinishRole && timeMachineClip.timeMachineClipEvent == TimeMachineClipEvent.PAUSE &&
                    time >= clip.end - TimeMachineDirector.frameDuration)
                {
                    Debug.Log($"{clip.displayName}, PAUSE");
                    playableDirector.time = clip.end - TimeMachineDirector.frameDuration;
                    break;

                }

                //  ======================= LOOP ========================= //
                if (!isMuteOrFinishRole && timeMachineClip.timeMachineClipEvent == TimeMachineClipEvent.LOOP &&
                    time >= clip.end - TimeMachineDirector.frameDuration)
                {
                    Debug.Log($"{clip.displayName}, LOOP");
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

            foreach (TimelineClip clip in clips)
            {
                TimeMachineClip timeMachineClip = clip.asset as TimeMachineClip;
                timeMachineClip.isFinishRole = false;
            }

        }




        public void ForceMoveSection(int index)
        {
            if (clips.Count == 0 || clips == null) return;
            GetCurrentInputIndex = index;
            FinishRole(GetCurrentInputIndex - 1);
            playableDirector.time = clips[index].start;

        }

        public void FinishRole(int index)
        {
            if (clips.Count <= index && index < 0) return;

            int i = 0;
            foreach (TimelineClip c in clips)
            {
                TimeMachineClip timeMachineClip = c.asset as TimeMachineClip;
                timeMachineClip.isFinishRole = i <= index;
                i++;
            }

            i = 0;
            foreach (TimelineClip c in clips)
            {
                Debug.Log($"{c.displayName},{(c.asset as TimeMachineClip).isFinishRole}");
                i++;
            }


        }

        public void OnNextState()

        {
            // currentInput.isFinishRole = true;
            playableDirector.time = clips[GetCurrentInputIndex].end;

        }
    }
}
