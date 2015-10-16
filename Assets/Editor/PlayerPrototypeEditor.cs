using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PlayerPrototype))]
class PlayerPrototypeEditor : Editor {

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
	}
}
