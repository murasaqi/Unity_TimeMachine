using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    [Serializable]
    public enum TimeMachineClipEvent
    {
        WAIT,
        LOOP,
        SKIP,
        THOROUGH,
        RESTART
    }


    [ExecuteAlways]
    public class TimeMachineTrackManager : MonoBehaviour
    {
        [SerializeField] public PlayableDirector playableDirector;
        public bool muteInEditMode = false;
        internal List<TimelineClip> clips;

        public TimelineClip GetClip(int index)
        {
            return clips[index];
        }

        public delegate void NextStateHandler();

        public delegate void InitHandler();

        private bool visible = true;

        public delegate void ForceMoveClip(int index, float offsetTime = 0f);

        public event NextStateHandler OnNextState;
        public event InitHandler OnInit;
        public event ForceMoveClip OnForceMoveClip;


        public UnityEvent onInitialize;
        public UnityEvent onClipStart;
        public UnityEvent onClipEnd;
        public int currentClipCount = 0;
        private TimelineAsset timelineAsset;

        [FormerlySerializedAs("forceUnMuteTimeMachineTrackOnAwake")] [SerializeField]
        private bool unMuteTrackOnStart = false;

        private TimeMachineControlTrack timeMachineControlTrack;
        public TextMeshProUGUI debugTextMesh;
        public bool muteAllClip = false;
        private StringBuilder stringBuilder = new StringBuilder();

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

        private void Awake()
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
            if (playableDirector != null)
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

                    if (unMuteTrackOnStart)
                    {
                        if (timeMachineControlTrack != null) timeMachineControlTrack.muted = false;
                    }
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
            if (clips == null) return null;
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
            if (debugTextMesh)
            {
                stringBuilder.Clear();
                if (timeMachineControlTrack == null ||
                    timeMachineControlTrack.timeMachineControlMixer == null ||
                    timeMachineControlTrack.timeMachineControlMixer.GetCurrentTimelineClip == null)
                {
                    return;
                }

                var dateTime = TimeSpan.FromSeconds(playableDirector.time);
                var currentClip = timeMachineControlTrack.timeMachineControlMixer.GetCurrentTimelineClip.displayName;
                stringBuilder.Append($"[{currentClip}]  ");
                stringBuilder.Append(dateTime.ToString(@"hh\:mm\:ss\:ff"));
                stringBuilder.Append(" ");
                stringBuilder.Append((Mathf.CeilToInt((float)timelineAsset.editorSettings.frameRate *
                                                      (float)playableDirector.time)));
                stringBuilder.Append("f  ");
                debugTextMesh.text = stringBuilder.ToString();
            }
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

        public void MoveClip(string sectionName, float offsetTime = 0f)
        {
            if (playableDirector.state != PlayState.Playing)
            {
                playableDirector.Play();
            }

            timeMachineControlTrack.ForceMoveClip(sectionName, offsetTime);
        }

        public void FinishRole()
        {
            Debug.Log("finish current clip");
            timeMachineControlTrack.FinishRoleCurrentClip();
        }
    }
}