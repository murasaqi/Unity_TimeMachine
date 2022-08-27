
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Iridescent.TimeMachine
{
    
    [ExecuteAlways]
    public class TimeMachineDebugViewer : MonoBehaviour
    {

        [SerializeField] public TimeMachineTrackManager timeMachineTrackManager;
        // [SerializeField] public PlayableDirector timeline;
        [SerializeField] public TextMeshProUGUI textMeshProUGUI;
        [SerializeField] public RectTransform clipButtonContainer;
        [SerializeField] public Color finishTextColor = Color.gray;
        [SerializeField] public Color defaultTextColor = Color.white;
        [SerializeField] public Color activeTextColor = new Color(0, 0.8f, 0.2f);
        [SerializeField] private Vector2 clipButtonSize = new Vector2(160, 100);
        private StringBuilder stringBuilder;
        private TimeMachineControlTrack timeMachineControlTrack = null;
        private TimelineAsset timelineAsset;
        private Dictionary<TimelineClip,TextMeshProUGUI> clipButtonTextDictionary = new Dictionary<TimelineClip, TextMeshProUGUI>();
        [SerializeField] private List<RectTransform> buttonRectTransforms = new List<RectTransform>();

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }



        public void InitTrack()
        {
            if(timeMachineTrackManager.playableDirector == null) return;
            if(timeMachineControlTrack == null)
            {
                timelineAsset  = timeMachineTrackManager.playableDirector.playableAsset as TimelineAsset;
                foreach (var trackAsset in timelineAsset.GetOutputTracks())
                {
                    if(trackAsset is TimeMachineControlTrack)
                    {
                        timeMachineControlTrack = trackAsset as TimeMachineControlTrack;
                        break;
                    }
                }
            }
        }
        
        [ContextMenu("Init")]
        public void Init()
        {

            timeMachineTrackManager = GameObject.FindObjectOfType<TimeMachineTrackManager>();
            
            InitTrack();
            if(timeMachineControlTrack == null) return;

            if(clipButtonContainer == null) return;
            for (int i =clipButtonContainer.childCount-1 ; i >= 0; i--)
            {
                DestroyImmediate(clipButtonContainer.GetChild(i).gameObject);
            }
            buttonRectTransforms.Clear();
            clipButtonTextDictionary.Clear();
            if(timeMachineTrackManager ==null) return;
            var buttonPrefab = Resources.Load<Button>("TimeMachinePrefab/TimeMachineClipButton");
            if (timeMachineControlTrack.hasClips)
            {
                var i = 0;
                foreach (var timelineClip in timeMachineControlTrack.GetClips())
                {
                    var button = Instantiate(buttonPrefab);
                    var textMesh =  button.GetComponentInChildren<TextMeshProUGUI>();
                    textMesh.text = GetClipButtonName(timelineClip);
                    button.onClick.AddListener(() =>
                    {
                        var asset = timelineClip.asset as TimeMachineControlClip;
                        Debug.Log(asset.clipIndex);
                        timeMachineTrackManager.MoveClip(asset.clipIndex);
                    });
                    clipButtonTextDictionary.Add(timelineClip,textMesh);
                    button.transform.SetParent(clipButtonContainer);
                    buttonRectTransforms.Add(button.GetComponent<RectTransform>());
                    buttonRectTransforms.Last().sizeDelta = clipButtonSize;
                    i++;
                }
            }
            
            var finishButton = Instantiate(buttonPrefab);
            var finishButtonTextMeshProUGUI =  finishButton.GetComponentInChildren<TextMeshProUGUI>();
            finishButtonTextMeshProUGUI.text = "Finish";
            finishButton.onClick.AddListener(() =>
            {
               timeMachineTrackManager.FinishCurrentClip();
            });
            finishButtonTextMeshProUGUI.color = Color.yellow;
            finishButton.transform.SetParent(clipButtonContainer);
            buttonRectTransforms.Add(finishButton.GetComponent<RectTransform>());
            buttonRectTransforms.Last().sizeDelta = clipButtonSize;
        }

        private string GetClipButtonName(TimelineClip clip)
        {
            var asset = clip.asset as TimeMachineControlClip;
            return $"{TimeSpan.FromSeconds(clip.start).ToString(@"hh\:mm\:ss\:ff")}\n[{asset.timeMachineClipEvent}] {clip.displayName}\n{TimeSpan.FromSeconds(clip.end).ToString(@"hh\:mm\:ss\:ff")}";
        }

        // Update is called once per frame
        void Update()
        {
            if(stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }

            stringBuilder.Clear();
            
            if(timeMachineTrackManager.playableDirector == null) return;
            
            if(timelineAsset == null) InitTrack();
            
            if(timeMachineControlTrack == null) return;
            var fps = (float)timelineAsset.editorSettings.frameRate;
            var dateTime = TimeSpan.FromSeconds(timeMachineTrackManager.playableDirector.time);
            if(timeMachineControlTrack.timeMachineControlMixer == null) return;
            if(timeMachineControlTrack.timeMachineControlMixer.GetCurrentTimelineClip == null) return;
            var currentClip = timeMachineControlTrack.timeMachineControlMixer.GetCurrentTimelineClip;
            var timeMachineControlClip = currentClip.asset as TimeMachineControlClip;
            if(timeMachineControlClip == null) return;
            var clipName = currentClip != null ? timeMachineControlClip.sectionName : "null";
            stringBuilder.Append($"[{timeMachineTrackManager.playableDirector.name}]  ");
            stringBuilder.Append($"{timeMachineTrackManager.playableDirector.state} ");
            stringBuilder.Append(dateTime.ToString(@"hh\:mm\:ss\:ff"));
            stringBuilder.Append(" ");
            stringBuilder.Append((Mathf.CeilToInt(fps * (float) timeMachineTrackManager.playableDirector.time)));
            stringBuilder.Append("f  ");
            stringBuilder.Append($"clip: {clipName}");
            
            if(textMeshProUGUI != null)
            {
                textMeshProUGUI.text = stringBuilder.ToString();
            }



            var reachCurrentClip = false;
            foreach (var clipTextPair in clipButtonTextDictionary)
            {
                var clip = clipTextPair.Key;
                var asset = clip.asset as TimeMachineControlClip;
                
                if (clipTextPair.Key == currentClip)
                {
                    clipTextPair.Value.color = new Color(activeTextColor.r,activeTextColor.g,activeTextColor.b,0.7f  + Mathf.Sin(Time.time*2)*0.3f);
                    clipTextPair.Value.text =GetClipButtonName(clip);
                        reachCurrentClip = true;
                }
                else
                {
                    clipTextPair.Value.color = reachCurrentClip ? defaultTextColor:finishTextColor;
                    clipTextPair.Value.text = GetClipButtonName(clip);
                }
            }


            var buttonIndex = 0;
            foreach (var button in buttonRectTransforms)
            {
                if (buttonIndex == timeMachineTrackManager.currentClipCount - 1 ||
                    buttonIndex == timeMachineTrackManager.currentClipCount ||
                    buttonIndex == timeMachineTrackManager.currentClipCount + 1)
                {
                    button.gameObject.SetActive(true);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
                 
                button.sizeDelta = clipButtonSize;
                buttonIndex++;
            }
            
        }
    }
}