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
    // [Serializable]
    // public struct TimeMachineControlClipValue
    // {
    //     public int index;
    //     public TimeMachineClipEvent clipEvent;
    //     public string name;
    //     public double duration;
    //     public double start;
    // }


    [Serializable]
    public enum TimeMachineClipEvent
    {
        WAIT,
        LOOP,
        SKIP,
        THOROUGH
    }
    
   

//[RequireComponent(typeof(Canvas))]
// [RequireComponent(typeof(PlayableDirector))]
// [ExecuteInEditMode] 
[ExecuteAlways]

    public class TimeMachineTrackManager : MonoBehaviour
    {
        [SerializeField] public PlayableDirector playableDirector;
        [SerializeField] public List<TimelineClip> clips;

        public delegate void NextStateHandler();

        public delegate void InitHandler();

        private bool visible = true;

        public delegate void ForceMoveClip(int index);

        public event NextStateHandler OnNextState;
        public event InitHandler OnInit;
        public event ForceMoveClip OnForceMoveClip;


        public UnityEvent onEndInitialize;
        public int currentClipCount = 0;
        private TimelineAsset timelineAsset;

        private TimeMachineControlTrack timeMachineControlTrack;
        // [SerializeField] public bool initialized;

        public PlayState state => playableDirector.state;

        public int clipCount
        {
            get => transform.childCount;
        }

        public double frameDuration
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
            timelineAsset = playableDirector.playableAsset as TimelineAsset;
            TimelineAsset asset = playableDirector.playableAsset as TimelineAsset;
            if (clips != null)
            {
                clips.Clear();
            }
            else
            {
                clips = new List<TimelineClip>();
            }

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
        }

        public void EnableClickButton()
        {

        }

        public void ResetTimeline()
        {
            if (OnInit != null) OnInit.Invoke();
            
            timeMachineControlTrack.Initialize();
        }

   
        private void Update()
        {
        }

        public void MoveNextClip()
        {
            MoveClip(currentClipCount + 1);
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
            var i = Mathf.Clamp(index, 0, timeMachineControlTrack.GetClips().Count());
            timeMachineControlTrack.ForceMoveClip(i);
        }

        public void FinishCurrentClip()
        {
            timeMachineControlTrack.FinishRoleCurrentClip();
        }
        // public void UpdateStatus()
        // {
        //     var totalWidth = 0f;
        //     var count = currentClipCount;
        //     var loopCounter = 0;
        //     var maxWidth = GetComponent<RectTransform>().rect.width;
        //
        //     
        //     
        //     if (pause)
        //     {
        //         Timeline.time = pauseTime;
        //         Timeline.Evaluate();
        //     }
        //
        //     foreach (var clip in clips)
        //     {
        //
        //         clip.SetActive(false);
        //     }
        //
        //     while (totalWidth < maxWidth)
        //     {
        //
        //         if (count >= clipValus.Count) break;
        //         var clip = clips[count];
        //         var info = clipValus[count];
        //         float width = MaxTimelineWidth * (float)info.duration;
        //         if (totalWidth + width > maxWidth) break;
        //         clip.gameObject.SetActive(true);
        //         var rect = clip.GetComponent<RectTransform>();
        //         var uiText = clip.transform.GetChild(0).GetComponent<Text>();
        //
        //         clip.GetComponent<Image>().color = waitingClipColor;
        //         clip.name = info.name;
        //         rect.sizeDelta = new Vector2(width, rect.rect.height);
        //         uiText.rectTransform.sizeDelta = new Vector2(width, rect.rect.height);
        //         uiText.text = info.name;
        //         count++;
        //         loopCounter++;
        //         totalWidth += width;
        //     }
        //
        //     if (currentClipCount >= 0)
        //     {
        //         recentClipImage = clips[currentClipCount].GetComponent<Image>();
        //     }
        //
        //
        //
        // }
        //
        // public void NextState()
        // {
        //     nextStateHandler();
        //     //            Timeline.time = 0;
        //     //            Timeline.Pause();
        // }
        //
        //
        // public void StartTimeline()
        // {
        //     pause = false;
        //     Timeline.Play();    
        //     
        // }
        //
        // public void PauseTimeline()
        // {
        //     Timeline.Pause();
        //     pauseTime = Timeline.time;
        //     pause = true;
        // }
        // public void Seek()
        // {
        //     //            isSeek = true;
        //     //            Timeline.time = seekBar.value * Timeline.duration;
        //     //            isSeek = false;
        // }

    }
}