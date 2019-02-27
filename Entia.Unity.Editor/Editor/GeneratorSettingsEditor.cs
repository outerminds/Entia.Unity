using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CustomEditor(typeof(GeneratorSettings), isFallback = true)]
    public class GeneratorSettingsEditor : UnityEditor.Editor
    {
        public GeneratorSettings Target => target as GeneratorSettings;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var valid = Generator.TryTool(Target, false, out var tool);
            var alive = Generator.TryProcess(tool, out var process);

            EditorGUILayout.Separator();

            if (!valid)
            {
                var pattern = serializedObject.FindProperty("_tool").stringValue;
                EditorGUILayout.HelpBox(
                    $"Could not find a valid executable that matches '{pattern}'.",
                    MessageType.Error);
            }
            else if (alive)
            {
                EditorGUILayout.HelpBox(
$@"Generator process:
-> Idendifier: {process.Id}
-> Name: {process.ProcessName}
-> Start time: {process.StartTime}
-> Process time: {process.TotalProcessorTime}",
                    MessageType.Info);
            }
            else EditorGUILayout.HelpBox("Generator process was not found.", MessageType.Warning);

            using (LayoutUtility.Disable(!valid || alive))
                if (GUILayout.Button("Birth")) Generator.Birth(tool, Target, true);

            using (LayoutUtility.Disable(!valid))
                if (GUILayout.Button("Generate")) Generator.Generate(tool, Target, true);

            using (LayoutUtility.Disable(!valid || !alive))
                if (GUILayout.Button("Kill")) Generator.Kill(tool, Target, true);
        }
    }
}