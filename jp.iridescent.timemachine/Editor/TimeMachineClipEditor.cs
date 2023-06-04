#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    [CustomTimelineEditor(typeof(TimeMachineControlClip))]
    public class TimeMachineControlClipEditor: ClipEditor
    {
        [InitializeOnLoad]
        class EditorInitialize
        {
            static EditorInitialize()
            {
                playableDirector = GetMasterDirector();  
                pauseIconTexture = Resources.Load<Texture2D>("icon_pause");
                muteTexture = Resources.Load<Texture2D>("icon_ignore");
                skipTexture = Resources.Load<Texture2D>("icon_skip");
                loopTexture = Resources.Load<Texture2D>("icon_loop");
                playTexture = Resources.Load<Texture2D>("icon_play");
                dotTexture = Resources.Load<Texture2D>("icon_dot");
              
            } 
            
            static PlayableDirector GetMasterDirector() { return TimelineEditor.masterDirector; }
        }
     
        private static PlayableDirector playableDirector;
        private static Texture2D pauseIconTexture;
        private static Texture2D playTexture;
        private static Texture2D muteTexture;
        private static Texture2D skipTexture;
        private static Texture2D loopTexture;
        private static Texture2D dotTexture;
        private static Color iconColor = new Color(143f / 255f, 242f / 255f, 216f / 255f);
        public override ClipDrawOptions GetClipOptions(TimelineClip clip)
        {
            return new ClipDrawOptions
            {
                errorText = GetErrorText(clip),
                highlightColor = GetDefaultHighlightColor(clip),
                icons = Enumerable.Empty<Texture2D>(),
                tooltip = "Tooltip"
            };
        }
        
        
        public override void OnClipChanged(TimelineClip clip)
        {
        
            // Debug.Log($"{clip.displayName} OnClipChanged");
            // var timeMachineControlClip = (TimeMachineControlClip)clip.asset;
            // if (timeMachineControlClip == null)
            //     return;
            //
            // if(timeMachineControlClip.mixer == null) return;
            //
            //
            // var sameNameCount = 0;
            // foreach (var c in timeMachineControlClip.mixer.clips)
            // {
            //    var asset = c.asset as TimeMachineControlClip;
            //    if (asset != timeMachineControlClip)
            //    {
            //        if (asset.sectionName == timeMachineControlClip.sectionName)
            //        {
            //            sameNameCount++;
            //        }
            //    }
            // }
            //
            // timeMachineControlClip.clipIndex = timeMachineControlClip.mixer.clips.IndexOf(clip);
            //
            // timeMachineControlClip.sectionName = sameNameCount == 0 ?  timeMachineControlClip.sectionName: $"{timeMachineControlClip.sectionName} ({sameNameCount})";
            //
            // // SetDirty
            // EditorUtility.SetDirty(timeMachineControlClip);
            // AssetDatabase.SaveAssets();

        }
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            Debug.Log(clonedFrom);
            var timeMachineControlClip = (TimeMachineControlClip)clip.asset;
            if (clonedFrom != null)
            {
                var clonedTimeMachineControlClip = (TimeMachineControlClip)clonedFrom.asset;
                timeMachineControlClip.sectionName = clonedTimeMachineControlClip.sectionName + "(clone)";
            }
            

        }


        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);


            var timelineClip = (TimeMachineControlClip) clip.asset;
      
            var iconSize = 12;
            var margin = 2;
            var onEndIconPosition = new Rect(region.position.width - iconSize - margin, region.position.height/2f-iconSize/2f, iconSize, iconSize);
            var onStartIconPosition = new Rect( margin, region.position.height/2f-iconSize/2f, iconSize, iconSize);
            var alpha = timelineClip.mute ? 0.5f : 1f;
            var color = timelineClip.mute ? Color.white : iconColor;
            var isFinishOnStart = timelineClip.isFinishOnStart;
            var isFinishOnEnd = timelineClip.isFinishOnEnd;
            Texture2D onStartIcon = null;
            Texture2D onEndIcon = null;
            if (timelineClip.onClipStartAction == TimeMachineClipEvent.LOOP)
            {
                onStartIcon = loopTexture;
            }
            
            if (timelineClip.onClipStartAction == TimeMachineClipEvent.SKIP)
            {
                onStartIcon = skipTexture;
            }
            
            if (timelineClip.onClipStartAction == TimeMachineClipEvent.THOROUGH)
            {
                onStartIcon = playTexture;
            }

            if (timelineClip.onClipStartAction == TimeMachineClipEvent.WAIT)
            {
                onStartIcon = pauseIconTexture;
            }

            
            
            if (timelineClip.onClipEndAction == TimeMachineClipEvent.LOOP)
            {
                onEndIcon = loopTexture;
            }
            
            if (timelineClip.onClipEndAction == TimeMachineClipEvent.SKIP)
            {
                onEndIcon = skipTexture;
            }
            
            if (timelineClip.onClipEndAction == TimeMachineClipEvent.THOROUGH)
            {
                onEndIcon = playTexture;
            }

            if (timelineClip.onClipEndAction == TimeMachineClipEvent.WAIT)
            {
                onEndIcon = pauseIconTexture;
            }
            if (timelineClip.mute)
            {
                onStartIcon = muteTexture;
                onEndIcon = muteTexture;
            }
           
            
            if(onStartIcon){
                GUI.DrawTexture(onStartIconPosition,
                onStartIcon, ScaleMode.ScaleAndCrop,
                true,
                0,
                timelineClip.isFinishOnStart ? new Color(1,1,1,0.5f): iconColor, 0, 0);}
            
            if(onEndIcon){
                GUI.DrawTexture(onEndIconPosition,
                onEndIcon, ScaleMode.ScaleAndCrop,
                true,
                0,
                timelineClip.isFinishOnEnd ? new Color(1,1,1,0.5f): iconColor, 0, 0);
                
            }
            
        }

     

        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            base.GetSubTimelines(clip, director, subTimelines);
        }
    }
}
#endif