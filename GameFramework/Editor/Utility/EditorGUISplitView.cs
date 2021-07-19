using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorGUISplitView
{

	public enum Direction {
		Horizontal,
		Vertical
	}

	Direction splitDirection;
	float splitNormalizedPosition;
	bool resize;
	public Vector2 scrollPosition;
	Rect availableRect;


	public EditorGUISplitView(Direction splitDirection) {
		splitNormalizedPosition = 0.5f;
		this.splitDirection = splitDirection;
	}

	public void BeginSplitView() {
		Rect tempRect;

		if(splitDirection == Direction.Horizontal)
			tempRect = EditorGUILayout.BeginHorizontal (GUILayout.ExpandWidth(true));
		else 
			tempRect = EditorGUILayout.BeginVertical (GUILayout.ExpandHeight(true));
		
		if (tempRect.width > 0.0f) {
			availableRect = tempRect;
		}
		if(splitDirection == Direction.Horizontal)
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width(availableRect.width * splitNormalizedPosition));
		else
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Height(availableRect.height * splitNormalizedPosition));
	}

	public void Split() {
		GUILayout.EndScrollView();
		ResizeSplitFirstView ();
	}

	public void EndSplitView() {

		if(splitDirection == Direction.Horizontal)
			EditorGUILayout.EndHorizontal ();
		else 
			EditorGUILayout.EndVertical ();
	}

	private void ResizeSplitFirstView(){

		Rect resizeHandleRect;

		if(splitDirection == Direction.Horizontal)
			resizeHandleRect = new Rect (availableRect.width * splitNormalizedPosition, availableRect.y, 2f, availableRect.height);
		else
			resizeHandleRect = new Rect (availableRect.x,availableRect.height * splitNormalizedPosition, availableRect.width, 2f);

		GUI.DrawTexture(resizeHandleRect,EditorGUIUtility.whiteTexture);

		if(splitDirection == Direction.Horizontal)
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeHorizontal);
		else
			EditorGUIUtility.AddCursorRect(resizeHandleRect,MouseCursor.ResizeVertical);

		if( Event.current.type == EventType.MouseDown && resizeHandleRect.Contains(Event.current.mousePosition)){
			resize = true;
		}
		if(resize){
			if(splitDirection == Direction.Horizontal)
				splitNormalizedPosition = Event.current.mousePosition.x / availableRect.width;
			else
				splitNormalizedPosition = Event.current.mousePosition.y / availableRect.height;
		}
		if(Event.current.type == EventType.MouseUp)
			resize = false;        
	}
}

