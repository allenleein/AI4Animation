﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEditor.SceneManagement;

public class MotionProcessor : EditorWindow {

	public static EditorWindow Window;
	public static Vector2 Scroll;

	public string Directory = string.Empty;
	public bool[] Active = new bool[0];
	public MotionData[] Data = new MotionData[0];

	[MenuItem ("Addons/Motion Processor")]
	static void Init() {
		Window = EditorWindow.GetWindow(typeof(MotionProcessor));
		Scroll = Vector3.zero;
	}
	
	void OnGUI() {
		Scroll = EditorGUILayout.BeginScrollView(Scroll);

		Utility.SetGUIColor(UltiDraw.Black);
		using(new EditorGUILayout.VerticalScope ("Box")) {
			Utility.ResetGUIColor();

			Utility.SetGUIColor(UltiDraw.Grey);
			using(new EditorGUILayout.VerticalScope ("Box")) {
				Utility.ResetGUIColor();

				Utility.SetGUIColor(UltiDraw.Orange);
				using(new EditorGUILayout.VerticalScope ("Box")) {
					Utility.ResetGUIColor();
					EditorGUILayout.LabelField("Processor");
				}

				if(Utility.GUIButton("Verify Data", UltiDraw.DarkGrey, UltiDraw.White)) {
					VerifyData();
				}

				if(Utility.GUIButton("Examine Data", UltiDraw.DarkGrey, UltiDraw.White)) {
					ExamineData();
				}

				if(Utility.GUIButton("Process Data", UltiDraw.DarkGrey, UltiDraw.White)) {
					ProcessData();
				}

				EditorGUILayout.BeginHorizontal();
				if(Utility.GUIButton("Enable All", UltiDraw.DarkGrey, UltiDraw.White)) {
					for(int i=0; i<Active.Length; i++) {
						Active[i] = true;
					}
				}
				if(Utility.GUIButton("Disable All", UltiDraw.DarkGrey, UltiDraw.White)) {
					for(int i=0; i<Active.Length; i++) {
						Active[i] = false;
					}
				}
				EditorGUILayout.EndHorizontal();

				using(new EditorGUILayout.VerticalScope ("Box")) {
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Assets/", GUILayout.Width(45f));
					LoadDirectory(EditorGUILayout.TextField(Directory));
					EditorGUILayout.EndHorizontal();

					for(int i=0; i<Data.Length; i++) {
						if(Active[i]) {
							Utility.SetGUIColor(UltiDraw.DarkGreen);
						} else {
							Utility.SetGUIColor(UltiDraw.DarkRed);
						}
						using(new EditorGUILayout.VerticalScope ("Box")) {
							Utility.ResetGUIColor();
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.LabelField((i+1).ToString(), GUILayout.Width(20f));
							Active[i] = EditorGUILayout.Toggle(Active[i], GUILayout.Width(20f));
							Data[i] = (MotionData)EditorGUILayout.ObjectField(Data[i], typeof(MotionData), true);
							EditorGUILayout.EndHorizontal();
						}
					}
				}

			}
		}

		EditorGUILayout.EndScrollView();
	}

	private void LoadDirectory(string directory) {
		if(Directory != directory) {
			Directory = directory;
			Data = new MotionData[0];
			Active = new bool[0];
			string path = "Assets/"+Directory;
			if(AssetDatabase.IsValidFolder(path)) {
				string[] files = AssetDatabase.FindAssets("t:MotionData", new string[1]{path});
				Data = new MotionData[files.Length];
				Active = new bool[files.Length];
				for(int i=0; i<files.Length; i++) {
					Data[i] = (MotionData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(files[i]), typeof(MotionData));
					Active[i] = true;
				}
			}
		}
	}

	private void VerifyData() {
		int errors = 0;
		for(int i=0; i<Data.Length; i++) {
			if(Active[i]) {
				for(int f=0; f<Data[i].GetTotalFrames(); f++) {
					float style = 0f;
					for(int s=0; s<Data[i].Frames[f].StyleValues.Length; s++) {
						style += Data[i].Frames[f].StyleValues[s];
					}
					if(style != 1f) {
						Debug.Log("One-hot failed in file " + Data[i].name + " at frame " + (f+1) + "!");
						errors += 1;
					}
				}
			}
		}
		Debug.Log("Errors: " + errors);
	}

	private void ExamineData() {
		float sum = 0f;
		float[] style = new float[Data[0].Styles.Length];
		for(int i=0; i<Data.Length; i++) {
			if(Active[i]) {
				for(int f=0; f<Data[i].GetTotalFrames(); f++) {
					for(int s=0; s<Data[i].Frames[f].StyleValues.Length; s++) {
						float value = Data[i].Frames[f].StyleValues[s];
						style[s] += value;
						sum += value;
					}
				}
			}
		}
		for(int i=0; i<style.Length; i++) {
			Debug.Log(Data[0].Styles[i] + " -> " + style[i] / sum + "%");
		}
	}

	private void ProcessData() {
        for(int i=0; i<Data.Length; i++) {
            if(Active[i]) {
				//Data[i].HeightMapSize = 0.25f;
				//Data[i].DepthMapResolution = 20;
				//Data[i].DepthMapSize = 10f;
				//Data[i].DepthMapDistance = 10f;
             	EditorUtility.SetDirty(Data[i]);
            }
		}
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
	}

}
#endif