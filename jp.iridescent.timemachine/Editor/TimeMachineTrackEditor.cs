
#if  UNITY_EDITOR

using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.Timeline;
using UnityEngine.UIElements;

namespace Iridescent.TimeMachine
{
    [CustomEditor(typeof(TimeMachineControlTrack))]
    public class TimeMachineControlTrackEditor : Editor
    {
        // OnInspectorGUIではなくCreateInspectorGUIを使う
        public override VisualElement CreateInspectorGUI()
        {
            var track = target as TimeMachineControlTrack;
            // Inspector拡張の場合、VisualElementはnewする
            var root = new VisualElement();

            // デフォルトのInspector表示を追加
            IMGUIContainer defaultInspector = new IMGUIContainer(() => DrawDefaultInspector());
            root.Add(defaultInspector);

            
            var layoutButton = new Button();
            layoutButton.text = "Auto Layout clips";
            
            layoutButton.clicked+=()=>
            {
                track.AutoLayoutClips(track.clipMargin);
            };
            root.Add(layoutButton);

            var targetTrackNameTextField = new TextField();
            targetTrackNameTextField.label = "Target Track Name";
            root.Add(targetTrackNameTextField);

            var createButton = new Button();
            createButton.text = "Auto Create clips with Margin";
            createButton.tooltip =
                "Create clips with margin from target track. clip duration is (target clip duration - margin).";
            createButton.clicked += () =>
            {
                track.CreateClipsAccordingTo(targetTrackNameTextField.value, track.clipMargin);
            };
            root.Add(createButton);
            
            // var alignButton = new Button();
            // alignButton.text = "Align clips of target track";
            // alignButton.clicked += () =>
            // {
            //     track.AlignTargetClips(targetTrackNameTextField.value);
            // };
            // root.Add(alignButton);

            var button = new Button();
            button.text = "Initialize clips";
            
            button.clicked+=()=>
            {
                track.Initialize();
            };
            root.Add(button);
            return root;
        }
    }    
}

#endif