
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

        [SerializeField] public TimeMachineDirector timeMachineDirector;
        // [SerializeField] public PlayableDirector timeline;
        [Header("TC Format [scene,state,tc,frame,clipName]")]
        [SerializeField] public string tcFormat = "scene,state,tc,frame,clipName";
        [SerializeField] public TextMeshProUGUI textMeshProUGUI;
        [SerializeField] public RectTransform clipButtonContainer;
        [SerializeField] public Color finishTextColor = Color.gray;
        [SerializeField] public Color defaultTextColor = Color.white;
        [SerializeField] public Color activeTextColor = new Color(0, 0.8f, 0.2f);
        [SerializeField] private Vector2 clipButtonSize = new Vector2(160, 100);
        private StringBuilder stringBuilder;
        private TimeMachineTrack timeMachineTrack = null;
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
            if(timeMachineDirector.playableDirector == null) return;
            if(timeMachineTrack == null)
            {
                timelineAsset  = timeMachineDirector.playableDirector.playableAsset as TimelineAsset;
                foreach (var trackAsset in timelineAsset.GetOutputTracks())
                {
                    if(trackAsset is TimeMachineTrack)
                    {
                        timeMachineTrack = trackAsset as TimeMachineTrack;
                        break;
                    }
                }
            }
        }
        
        [ContextMenu("Init")]
        public void Init()
        {

            timeMachineDirector = GameObject.FindObjectOfType<TimeMachineDirector>();
            
            InitTrack();
            if(timeMachineTrack == null) return;

            if(clipButtonContainer == null) return;
            for (int i =clipButtonContainer.childCount-1 ; i >= 0; i--)
            {
                DestroyImmediate(clipButtonContainer.GetChild(i).gameObject);
            }
            buttonRectTransforms.Clear();
            clipButtonTextDictionary.Clear();
            if(timeMachineDirector ==null) return;
            var buttonPrefab = Resources.Load<Button>("TimeMachinePrefab/TimeMachineClipButton");
            if (timeMachineTrack.hasClips)
            {
                var i = 0;
                foreach (var timelineClip in timeMachineTrack.GetClips())
                {
                    var button = Instantiate(buttonPrefab);
                    var textMesh =  button.GetComponentInChildren<TextMeshProUGUI>();
                    textMesh.text = GetClipButtonName(timelineClip);
                    button.onClick.AddListener(() =>
                    {
                        var asset = timelineClip.asset as TimeMachineClip;
                        Debug.Log(asset.clipIndex);
                        timeMachineDirector.MoveClip(asset.clipIndex);
                    });
                    clipButtonTextDictionary.Add(timelineClip,textMesh);
                    button.transform.SetParent(clipButtonContainer);
                    button.transform.localScale = Vector3.one;
                    button.transform.localPosition = Vector3.zero;
                    button.transform.localEulerAngles = Vector3.zero;
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
               timeMachineDirector.FinishCurrentClip();
            });
            finishButtonTextMeshProUGUI.color = Color.yellow;
            finishButton.transform.SetParent(clipButtonContainer);
            buttonRectTransforms.Add(finishButton.GetComponent<RectTransform>());
            buttonRectTransforms.Last().sizeDelta = clipButtonSize;
        }

        private string GetClipButtonName(TimelineClip clip)
        {
            var asset = clip.asset as TimeMachineClip;

            
            return $"{TimeSpan.FromSeconds(clip.start).ToString(@"hh\:mm\:ss\:ff")}\n[{asset.timeMachineClipEvent}] {clip.displayName}\n{TimeSpan.FromSeconds(clip.end).ToString(@"hh\:mm\:ss\:ff")}";
        }

        private void DestroyButtons()
        {
            for (int i =clipButtonContainer.childCount-1 ; i >= 0; i--)
            {
                DestroyImmediate(clipButtonContainer.GetChild(i).gameObject);
            }
            
            clipButtonTextDictionary.Clear();
            buttonRectTransforms.Clear();
        }

        private void OnDestroy()
        {
            DestroyButtons();
        }

        private void OnApplicationQuit()
        {
            DestroyButtons();
        }

        private void OnDisable()
        {
            DestroyButtons();
        }


        private void UpdateTC()
        {
            if(stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }

            var format = tcFormat.Split();
            stringBuilder.Clear();
      
            var fps = (float)timelineAsset.editorSettings.frameRate;
            var dateTime = TimeSpan.FromSeconds(timeMachineDirector.playableDirector.time);
            // stringBuilder.Append($"[{timeMachineTrackManager.playableDirector.name}]  ");
            // stringBuilder.Append($"{timeMachineTrackManager.playableDirector.state} ");
            // stringBuilder.Append(dateTime.ToString(@"hh\:mm\:ss\:ff"));
            // stringBuilder.Append(" ");
            // stringBuilder.Append((Mathf.CeilToInt(fps * (float) timeMachineTrackManager.playableDirector.time)));
            // stringBuilder.Append("f  ");
            // stringBuilder.Append($"clip: {clipName}");
            var currentClip = timeMachineTrack.timeMachineMixer.GetCurrentTimelineClip;
            var timeMachineControlClip = currentClip.asset as TimeMachineClip;
            var clipName = currentClip != null ? timeMachineControlClip.sectionName : "null";
            foreach (var f in format)
            {
                switch (f)
                {
                    case "scene" :
                        stringBuilder.Append($"[{timeMachineDirector.playableDirector.name}]  ");
                        break;
                    case   "state" :
                        stringBuilder.Append($"{timeMachineDirector.playableDirector.state} ");
                        break;
                    case "tc" :
                        stringBuilder.Append($"{dateTime.ToString(@"hh\:mm\:ss\:ff")} ");
                        break;
                    case "frame" :
                        stringBuilder.Append(($"{Mathf.CeilToInt(fps * (float) timeMachineDirector.playableDirector.time)} f"));
                        break;
                    case "clipName" :
                        stringBuilder.Append($"clip: {clipName} ");
                        break;
                }
            }
            
            if(textMeshProUGUI != null)
            {
                textMeshProUGUI.text = stringBuilder.ToString();
            }

        }
        // Update is called once per frame
        void Update()
        {
            
            if(timeMachineDirector.playableDirector == null) return;
            
            if(timelineAsset == null) InitTrack();
            
            if(timeMachineTrack == null) return;
            
            
            if(timeMachineTrack.timeMachineMixer == null) return;
            if(timeMachineTrack.timeMachineMixer.GetCurrentTimelineClip == null) return;
            var currentClip = timeMachineTrack.timeMachineMixer.GetCurrentTimelineClip;
            var timeMachineControlClip = currentClip.asset as TimeMachineClip;
            if(timeMachineControlClip == null) return;
            
            
           
            
         
            UpdateTC();

            var reachCurrentClip = false;
            foreach (var clipTextPair in clipButtonTextDictionary)
            {
                var clip = clipTextPair.Key;
                var asset = clip.asset as TimeMachineClip;
                
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
                if (buttonIndex == timeMachineDirector.currentClipCount - 1 ||
                    buttonIndex == timeMachineDirector.currentClipCount ||
                    buttonIndex == timeMachineDirector.currentClipCount + 1)
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