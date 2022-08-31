using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Iridescent.TimeMachine
{
    [CustomTimelineEditor(typeof(TimeMachineClip))]
    public class TimeMachineControlClipEditor: ClipEditor
    {

        private static PlayableDirector playableDirector;
        private static Texture2D pauseIconTexture;
        private static Texture2D playTexture;
        private static Texture2D muteTexture;
        private static Texture2D skipTexture;
        private static Texture2D loopTexture;
        private static Texture2D dotTexture;
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

            TimeMachineClip timeMachineControlClip = (TimeMachineClip)clip.asset;
            if (timeMachineControlClip == null)
                return;
            clip.displayName = $"#{timeMachineControlClip.clipIndex} {timeMachineControlClip.sectionName}";


        }
        public override void OnCreate(TimelineClip clip, TrackAsset track, TimelineClip clonedFrom)
        {
            base.OnCreate(clip, track, clonedFrom);

        }


        public override void DrawBackground(TimelineClip clip, ClipBackgroundRegion region)
        {
            base.DrawBackground(clip, region);


            TimeMachineClip timelineClip = (TimeMachineClip)clip.asset;

            TimeMachineTrack track = clip.GetParentTrack() as TimeMachineTrack;
            int iconSize = 12;
            int tallySize = 8;
            int margin = 2;
            Rect iconPosition = new Rect(region.position.width - iconSize - margin, region.position.height / 2f - iconSize / 2f, iconSize, iconSize);
            Rect tallyPosition = new Rect(margin, region.position.height / 2f - tallySize / 2f, tallySize, tallySize);
            float alpha = track.muteClipRole ? 0.5f : 1f;
            Color color = track.muteClipRole ? Color.white : new Color(0, 1f, 0.1f);
            bool isFinish = timelineClip.isFinishRole;
            Texture2D icon = null;
            if (timelineClip.timeMachineClipEvent == TimeMachineClipEvent.LOOP)
            {
                icon = loopTexture;
            }

            if (timelineClip.timeMachineClipEvent == TimeMachineClipEvent.SKIP)
            {
                icon = skipTexture;
            }

            if (timelineClip.timeMachineClipEvent == TimeMachineClipEvent.THOROUGH)
            {
                icon = playTexture;
            }

            if (timelineClip.timeMachineClipEvent == TimeMachineClipEvent.PAUSE)
            {
                icon = pauseIconTexture;
            }

            if (track.muteClipRole)
            {
                icon = muteTexture;
            }


            GUI.DrawTexture(iconPosition,
                            icon, ScaleMode.ScaleAndCrop,
                            true,
                            0,
                            new Color(color.r, color.g, color.b, alpha), 0, 0);

            GUI.DrawTexture(tallyPosition,
                            dotTexture, ScaleMode.ScaleAndCrop,
                            true,
                            0,
                            timelineClip.isFinishRole ? new Color(1, 1, 1, 0.5f) : new Color(0f, 1f, 0.1f, 1f), 0, 0);

        }



        public override void GetSubTimelines(TimelineClip clip, PlayableDirector director, List<PlayableDirector> subTimelines)
        {
            base.GetSubTimelines(clip, director, subTimelines);
        }

        [InitializeOnLoad]
        private class EditorInitialize
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
            private static PlayableDirector GetMasterDirector() { return TimelineEditor.masterDirector; }
        }
    }
}
