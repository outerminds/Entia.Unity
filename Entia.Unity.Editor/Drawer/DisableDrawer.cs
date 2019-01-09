using UnityEditor;
using UnityEngine;

namespace Entia.Unity.Editor
{
	[CustomPropertyDrawer(typeof(DisableAttribute))]
	public sealed class DisableDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			using (LayoutUtility.Disable()) EditorGUI.PropertyField(position, property, label, true);
		}
	}
}
