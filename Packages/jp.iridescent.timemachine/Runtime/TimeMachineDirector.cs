using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{



    [Serializable]
    public enum TimeMachineClipEvent
    {
        PAUSE,
        LOOP,
        SKIP,
        THOROUGH
    }


    [ExecuteAlways]
    public class TimeMachineDirector: MonoBehaviour
    {


        public delegate void ForceMoveSection(int index);
        public delegate void InitHandler();
        public delegate void NextStateHandler();

        [SerializeField] public PlayableDirector playableDirector;
        [SerializeField] public List<TimelineClip> clips;

        public UnityEvent onEndInitialize;
        public UnityEvent onTimelinePlay;
        public UnityEvent onSectionChange;
        public int currentClipCount;
        private TimelineAsset timelineAsset;

        private TimeMachineTrack timeMachineTrack;

        public PlayState state => playableDirector.state;


        public double frameDuration
        {
            get
            {
                if (timelineAsset == null) timelineAsset = playableDirector.playableAsset as TimelineAsset;
                return timelineAsset ? 1d / timelineAsset.editorSettings.frameRate : 0;
            }
        }


        private void Start()
        {
            Init();
        }

        private void OnEnable()
        {
            Init();
        }

        // public event NextStateHandler OnNextState;
        public event InitHandler OnInit;
        public event ForceMoveSection OnForceMoveSection;


        [ContextMenu("Init")]
        public void Init()
        {
            if (playableDirector == null) return;
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
            if (timelineAsset == null) return;

            if (clips != null)
            {
                clips.Clear();
            }
            else
            {
                clips = new List<TimelineClip>();
            }

            foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            {
                if (track is TimeMachineTrack)
                {
                    timeMachineTrack = track as TimeMachineTrack;
                    clips = timeMachineTrack.GetClips().ToList();
                }
            }

            foreach (TimelineClip clip in clips)
            {

                TimeMachineClip timeMachineClip = clip.asset as TimeMachineClip;
                if (timeMachineClip == null) return;
                timeMachineClip.isFinishRole = false;
                timeMachineClip.isFinishRole = false;
            }
        }

        public void ResetTimeline()
        {
            OnInit?.Invoke();
        }


        public void MoveNextClip()
        {
            MoveClip(currentClipCount + 1);
        }

        public void Play()
        {
            playableDirector.Play();
        }

        public void Pause()
        {
            playableDirector.Pause();
        }

        public void Stop()
        {
            playableDirector.Stop();
        }

        public void Evaluate()
        {
            playableDirector.Evaluate();
        }

        public void MovePreviousClip()
        {
        }

        public void MoveClip(int index)
        {
            int i = Mathf.Clamp(index, 0, timeMachineTrack.GetClips().Count());
            timeMachineTrack.ForceMoveClip(i);
            OnForceMoveSection?.Invoke(i);
        }

        public void FinishCurrentClip()
        {
            timeMachineTrack.FinishRoleCurrentClip();
        }
    }
}
