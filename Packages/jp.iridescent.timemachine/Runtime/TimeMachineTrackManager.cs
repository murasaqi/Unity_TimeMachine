using System;
using System.Collections;
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
        WAIT,
        LOOP,
        SKIP,
        THOROUGH
    }
    
   
    
[ExecuteAlways]

    public class TimeMachineTrackManager : MonoBehaviour
    {
        [SerializeField] public PlayableDirector playableDirector;
        public bool muteInEditMode = false;
        internal List<TimelineClip> clips;

        public delegate void NextStateHandler();

        public delegate void InitHandler();

        private bool visible = true;

        public delegate void ForceMoveClip(int index);

        public event NextStateHandler OnNextState;
        public event InitHandler OnInit;
        public event ForceMoveClip OnForceMoveClip;


        public UnityEvent onInitialize;
        public UnityEvent onClipStart;
        public UnityEvent onClipEnd;
        public int currentClipCount = 0;
        private TimelineAsset timelineAsset;

        private TimeMachineControlTrack timeMachineControlTrack;
        

        public double FramePerSec
        {
            get
            {
                if (timelineAsset == null) timelineAsset = playableDirector.playableAsset as TimelineAsset;
                return timelineAsset ? 1d / timelineAsset.editorSettings.frameRate : 0;
            }
        }

        private void OnValidate()
        {
        }

        private void Start()
        {
            onClipStart.AddListener(() =>
            {
                // Debug.Log( "Clip Start");
            });
            
            onClipEnd.AddListener(() =>
            {
                // Debug.Log( "Clip End");
            });
            onInitialize.AddListener(() =>
            {
                // Debug.Log( "End Initialize");
            });
            
            Init();
        }

        private void OnEnable()
        {
            if(playableDirector != null)
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
            Init();
        }
        
        
        [ContextMenu("Init")]
        public void Init()
        {
           
            if (clips != null)
            {
                clips.Clear();
            }
            else
            {
                clips = new List<TimelineClip>();
            }
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
            TimelineAsset asset = playableDirector.playableAsset as TimelineAsset;
           

            foreach (var track in asset.GetOutputTracks())
            {
                if (track.GetType() == typeof(TimeMachineControlTrack))
                {
                    timeMachineControlTrack = track as TimeMachineControlTrack;
                    clips = timeMachineControlTrack.GetClips().ToList();
                }
            }

            foreach (var clip in clips)
            {
                
                var timeMachineClip = clip.asset as TimeMachineControlClip;
                timeMachineClip.isFinishOnStart = false;
                timeMachineClip.isFinishOnEnd = false;
                timeMachineClip.mute = false;
            }
            
            onInitialize.Invoke();
        }

        
        public List<TimeMachineControlClip> GetTimeMachineControlClips()
        {
            if(clips== null) return null;
            List<TimeMachineControlClip> timeMachineControlClips = new List<TimeMachineControlClip>();
            foreach (var clip in clips)
            {
                timeMachineControlClips.Add(clip.asset as TimeMachineControlClip);
            }

            return timeMachineControlClips;
        }
        public void EnableClickButton()
        {

        }

        public void ResetTimeline()
        {
            
            timeMachineControlTrack.Initialize();
            onInitialize.Invoke();
        }

   
        private void Update()
        {
        }

      

        public void Play()
        {
            playableDirector.Play();
        }
        

        public void Replay()
        {
            playableDirector.Stop();
            playableDirector.Play();
        }

        public void ResetAndReplay()
        {
            playableDirector.Stop();
            ResetTimeline();
            playableDirector.Play();
        }
        public void Resume()
        {
            playableDirector.Resume();
        }

        public void Pause()
        {
            playableDirector.Pause();
        }

        public void Stop()
        {
            playableDirector.time = 0f;
            playableDirector.Evaluate();
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
            if (playableDirector.state != PlayState.Playing)
            {
                playableDirector.Play();
            }
            var i = Mathf.Clamp(index, 0, timeMachineControlTrack.GetClips().Count());
            timeMachineControlTrack.ForceMoveClip(i);
        }

        public void MoveClip(string sectionName)
        {

            if (playableDirector.state != PlayState.Playing)
            {
                playableDirector.Play();
            }
            timeMachineControlTrack.ForceMoveClip(sectionName);

        }

        public void FinishRole()
        {
            Debug.Log("finish current clip");
            timeMachineControlTrack.FinishRoleCurrentClip();
        }
      

    }
}