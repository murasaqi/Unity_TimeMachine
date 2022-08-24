
#if  UNITY_EDITOR

using UnityEditor;
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