
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Serialization;
using UnityEngine.Timeline;
using UnityEngine.UI;

namespace Iridescent.TimeMachine
{
    
    public struct ClipButtonGUI
    {
        public TextMeshProUGUI textMeshProUGUI;
        public Image progressBarBackground;
        public Image progressBar;
        public float previousProgressWidth;
            
        public ClipButtonGUI(TextMeshProUGUI textMeshProUGUI, Image progressBarBackground, Image progressBar)
        {
            this.textMeshProUGUI = textMeshProUGUI;
            this.progressBarBackground = progressBarBackground;
            this.progressBar = progressBar;
            previousProgressWidth = 0;
        }
    }
    
    [ExecuteAlways]
    public class TimeMachineDebugViewer : MonoBehaviour
    {

        [SerializeField] public TimeMachineTrackManager timeMachineTrackManager;
        // [SerializeField] public PlayableDirector timeline;
        [Header("TC Format [scene,state,tc,frame,clipName]")]
        [SerializeField] public string tcFormat = "scene,state,tc,frame,clipName";
        [FormerlySerializedAs("textMeshProUGUI")] [SerializeField] public TextMeshProUGUI timeMachineStatusText;
        [SerializeField] public RectTransform clipButtonContainer;
        [SerializeField] public Color finishTextColor = Color.gray;
        [SerializeField] public Color defaultTextColor = Color.white;
        [SerializeField] public Color activeTextColor = new Color(0, 0.8f, 0.2f);
        [SerializeField] private Vector2 clipButtonSize = new Vector2(160, 100);
        private StringBuilder stringBuilder;
        private TimeMachineControlTrack timeMachineControlTrack = null;
        private TimelineAsset timelineAsset;
        private Dictionary<TimelineClip,ClipButtonGUI> clipButtonTextDictionary = new Dictionary<TimelineClip, ClipButtonGUI>();
        [SerializeField] private List<RectTransform> buttonRectTransforms = new List<RectTransform>();

        private bool _isPause = false;

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
            
            
            
            var resetButton = Instantiate(buttonPrefab);
            var resetButtonTextMeshProUGUI =  resetButton.GetComponentInChildren<TextMeshProUGUI>();
            resetButtonTextMeshProUGUI.text = "Reset";
            resetButton.onClick.AddListener(() =>
            {
                timeMachineTrackManager.ResetAndReplay();
            });
            resetButtonTextMeshProUGUI.color = Color.yellow;
            resetButton.transform.SetParent(clipButtonContainer);
            buttonRectTransforms.Add(resetButton.GetComponent<RectTransform>());
            buttonRectTransforms.Last().sizeDelta = clipButtonSize;
            
            
            
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
                    var clipButtonGUI = new ClipButtonGUI();
                    clipButtonGUI.textMeshProUGUI = textMesh;
                    clipButtonGUI.progressBarBackground = button.transform.GetChild(0).GetComponent<Image>();
                    clipButtonGUI.progressBar = clipButtonGUI.progressBarBackground.transform.GetChild(0).GetComponent<Image>();
                    clipButtonTextDictionary.Add(timelineClip,clipButtonGUI);
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
               timeMachineTrackManager.FinishRole();
            });
            finishButtonTextMeshProUGUI.color = Color.yellow;
            finishButton.transform.SetParent(clipButtonContainer);
            buttonRectTransforms.Add(finishButton.GetComponent<RectTransform>());
            buttonRectTransforms.Last().sizeDelta = clipButtonSize;
            
            var pauseAndPlayButton = Instantiate(buttonPrefab);
            var pauseAndPlayButtonTextMeshProUGUI =  pauseAndPlayButton.GetComponentInChildren<TextMeshProUGUI>();
            pauseAndPlayButtonTextMeshProUGUI.text = "Pause and Play";
            pauseAndPlayButton.onClick.AddListener(() =>
            {
                if (_isPause)
                {
                    timeMachineTrackManager.Play();
                }
                else
                {
                    timeMachineTrackManager.Pause();
                }
            });
            pauseAndPlayButtonTextMeshProUGUI.color = Color.yellow;
            pauseAndPlayButton.transform.SetParent(clipButtonContainer);
            buttonRectTransforms.Add(pauseAndPlayButton.GetComponent<RectTransform>());
            buttonRectTransforms.Last().sizeDelta = clipButtonSize;
        }

        private string GetClipButtonName(TimelineClip clip)
        {
            var asset = clip.asset as TimeMachineControlClip;

            
            return $"{TimeSpan.FromSeconds(clip.start).ToString(@"mm\:ss")}\n[{asset.onClipStartAction.ToString()[0]}] {clip.displayName} [{asset.onClipEndAction.ToString()[0]}]\n{TimeSpan.FromSeconds(clip.end).ToString(@"mm\:ss")}";
        }

        private void DestroyButtons()
        {
            if(clipButtonContainer == null || clipButtonContainer.childCount == 0) return;
            for (int i =clipButtonContainer.childCount-1 ; i >= 0; i--)
            {
                if(clipButtonContainer.GetChild(i) != null && clipButtonContainer.GetChild(i).gameObject != null)DestroyImmediate(clipButtonContainer.GetChild(i).gameObject);
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
            if(timelineAsset == null) return;
            if(stringBuilder == null)
            {
                stringBuilder = new StringBuilder();
            }

            var format = tcFormat.Split();
            stringBuilder.Clear();
      
            var fps = (float)timelineAsset.editorSettings.frameRate;
            var dateTime = TimeSpan.FromSeconds(timeMachineTrackManager.playableDirector.time);
            var currentClip = timeMachineControlTrack.timeMachineControlMixer.CurrentTimelineClip;
            var timeMachineControlClip = currentClip.asset as TimeMachineControlClip;
            var clipName = currentClip != null ? timeMachineControlClip.sectionName : "null";
            foreach (var f in format)
            {
                switch (f)
                {
                    case "scene" :
                        stringBuilder.Append($"[{timeMachineTrackManager.playableDirector.name}]  ");
                        break;
                    case   "state" :
                        stringBuilder.Append($"{timeMachineTrackManager.playableDirector.state} ");
                        break;
                    case "tc" :
                        stringBuilder.Append($"{dateTime.ToString(@"hh\:mm\:ss\:ff")} ");
                        break;
                    case "frame" :
                        stringBuilder.Append(($"{Mathf.CeilToInt(fps * (float) timeMachineTrackManager.playableDirector.time)} f"));
                        break;
                    case "clipName" :
                        stringBuilder.Append($"clip: {clipName} ");
                        break;
                }
            }
            
            if(timeMachineStatusText != null)
            {
                timeMachineStatusText.text = stringBuilder.ToString();
            }

        }
        // Update is called once per frame
        void Update()
        {
            
            if(timeMachineTrackManager == null) return;
            
            if(timelineAsset == null) InitTrack();
            
            if(timeMachineControlTrack == null) return;
            
            
            if(timeMachineControlTrack.timeMachineControlMixer == null) return;
            if(timeMachineControlTrack.timeMachineControlMixer.CurrentTimelineClip == null) return;
            var currentClip = timeMachineControlTrack.timeMachineControlMixer.CurrentTimelineClip;
            var timeMachineControlClip = currentClip.asset as TimeMachineControlClip;
            if(timeMachineControlClip == null) return;
            
            
           
            
         
            UpdateTC();

            var reachCurrentClip = false;
            foreach (var clipTextPair in clipButtonTextDictionary)
            {
                var clip = clipTextPair.Key;
                var asset = clip.asset as TimeMachineControlClip;
                var clipButtonGUI = clipTextPair.Value;
                if (clipTextPair.Key == currentClip)
                {
                    clipButtonGUI.textMeshProUGUI.color = new Color(activeTextColor.r,activeTextColor.g,activeTextColor.b,0.6f  + Mathf.Sin(Time.time*4)*0.4f);
                    clipButtonGUI.textMeshProUGUI.text =GetClipButtonName(clip);
                        reachCurrentClip = true;
                        
                }
                else
                {
                    clipTextPair.Value.textMeshProUGUI.color = reachCurrentClip ? defaultTextColor:finishTextColor;
                    clipTextPair.Value.textMeshProUGUI.text = GetClipButtonName(clip);
                }
                
                var progress =Mathf.Clamp( (float)(timeMachineTrackManager.playableDirector.time - clip.start) / (float)(clip.end - clip.start),0f,1f);
                var progressBarBackground = clipTextPair.Value.progressBarBackground;
                var progressWidth = progressBarBackground.rectTransform.rect.width * (float)progress;
                var progressBar = clipTextPair.Value.progressBar;

                Debug.Log($"{asset.sectionName}, {progressBar.rectTransform.sizeDelta.x}, {progressWidth}");
                if(progressBar.rectTransform.sizeDelta.x != progressWidth)
                {
                    clipTextPair.Value.progressBar.rectTransform.rect.Set(0,0,progressWidth,progressBar.rectTransform.rect.height);
                    progressBar.rectTransform.sizeDelta = new Vector2(progressWidth, progressBar.rectTransform.sizeDelta.y);
                }

            }
            
            
        }
    }
}