using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Aarthificial.Reanimation.Cels;
using Aarthificial.Reanimation.Nodes;
using UnityEditor;
using UnityEngine;

namespace Aarthificial.Reanimation.Editor.Nodes
{
    public class MyWindow : EditorWindow
    {
        string myString = "Hello World";
        bool groupEnabled;
        bool myBool = true;
        float myFloat = 1.23f;

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            myString = EditorGUILayout.TextField("Text Field", myString);

            groupEnabled = EditorGUILayout.BeginToggleGroup("Optional Settings", groupEnabled);
            myBool = EditorGUILayout.Toggle("Toggle", myBool);
            myFloat = EditorGUILayout.Slider("Slider", myFloat, -3, 3);
            EditorGUILayout.EndToggleGroup();
        }
    }
}