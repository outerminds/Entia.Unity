using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
    [CreateAssetMenu(fileName = "Settings", menuName = "Entia/Generator/Settings")]
    public class GeneratorSettings : ScriptableObject
    {
        public static GeneratorSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = AssetDatabase.FindAssets($"t:{nameof(GeneratorSettings)}")
                        .Select(AssetDatabase.GUIDToAssetPath)
                        .Select(AssetDatabase.LoadAssetAtPath<GeneratorSettings>)
                        .FirstOrDefault(settings => settings != null);
                }

                return _instance;
            }
        }
        static GeneratorSettings _instance;

        /// <summary>
        /// The path to the generator executable.
        /// </summary>
        public string Tool => PathUtility.Replace(_tool);
        /// <summary>
        /// If 'true', the generator will be triggered when an input file is imported.
        /// </summary>
        public bool Automatic => _automatic;
        /// <summary>
        /// If 'true', the generator will be triggered when an input file is modified.
        /// </summary>
        public bool Watch => _watch;
        /// <summary>
        /// If 'true', the generator will spawn a visible console window and all logs will be printed to the console.
        /// </summary>
        public bool Debug => _debug;
        /// <summary>
        /// Delay in seconds after which the generation will be aborted.
        /// </summary>
        public float Timeout => _timeout;
        /// <summary>
        /// Files or directories that will be included in the generation.
        /// These paths are recursive.
        /// </summary>
        public string[] Inputs => Array.ConvertAll(_inputs, PathUtility.Replace);
        /// <summary>
        /// Directory where the generated files will be copied to.
        /// </summary>
        public string Output => PathUtility.Replace(_output);
        /// <summary>
        /// Files or directories that will be included as assembly references in the generation.
        /// These paths are non-recursive.
        /// </summary>
        public string[] Assemblies => Array.ConvertAll(_assemblies, PathUtility.Replace);
        /// <summary>
        /// Namespace suffix that will be added to hold the generated types.
        /// </summary>
        public string Suffix => _suffix;
        /// <summary>
        /// Log file path relative to the output path.
        /// </summary>
        public string Log => _log;

        [SerializeField, Tooltip("The path to the generator executable.")]
        string _tool = Path.Combine("{Entia.Generator}", "Entia.Unity.Generate.dll");
        [SerializeField, Tooltip("If 'true', the generator will be triggered when an input file is imported.")]
        bool _automatic = true;
        [SerializeField, Tooltip("If 'true', the generator will be triggered when an input file is modified.")]
        bool _watch = true;
        [SerializeField, Tooltip("If 'true', the generator will spawn a visible console window and all logs will be printed to the console.")]
        bool _debug = false;
        [SerializeField, Tooltip("Delay in seconds after which the generation will be aborted.")]
        float _timeout = 10f;
        [SerializeField, Tooltip(
@"Files or directories that will be included in the generation.
These paths are recursive.")]
        string[] _inputs = { Path.Combine("{Assets}", "Scripts"), Path.Combine("{Assets}", "Plugins") };
        [SerializeField, Tooltip("Directory where the generated files will be copied to.")]
        string _output = Path.Combine("{Assets}", "Generated");
        [SerializeField, Tooltip(
@"Files or directories that will be included as assembly references in the generation.
These paths are non-recursive.")]
        string[] _assemblies = { "{Editor.Mono}", "{Editor.Mono.Facades}", "{Editor.Managed}" };
        [SerializeField, Tooltip("Namespace suffix that will be added to hold the generated types.")]
        string _suffix = "Generated";
        [SerializeField, Tooltip("Log file path relative to the output path.")]
        string _log = "Log.txt";
    }
}
