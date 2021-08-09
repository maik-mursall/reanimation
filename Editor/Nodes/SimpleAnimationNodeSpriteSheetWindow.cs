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
    public class SimpleAnimationNodeSpriteSheetWindow : EditorWindow
    {
        private int _framesPerClip = 10;

        private string _driverName = "driverName";
        private bool _autoIncrement = true;

        private bool _constructSwitch = true;
        private string _switchDriverName = "switchDriverName";
        private bool _switchAutoIncrement = false;

        void OnGUI()
        {
            GUILayout.Label("Base Settings", EditorStyles.boldLabel);
            _framesPerClip = EditorGUILayout.IntField("Frames per clip", _framesPerClip);
            
            _driverName = EditorGUILayout.TextField("Driver Name", _driverName);
            _autoIncrement = EditorGUILayout.Toggle("Auto Increment", _autoIncrement);
            
            _constructSwitch = EditorGUILayout.BeginToggleGroup("Construct Switch", _constructSwitch);
            
            _switchDriverName = EditorGUILayout.TextField("Driver Name", _switchDriverName);
            _switchAutoIncrement = EditorGUILayout.Toggle("Auto Increment", _switchAutoIncrement);
            
            EditorGUILayout.EndToggleGroup();

            if (GUILayout.Button("Begin"))
            {
                CreateAnimationDrivers();
                
                Close();
            }
        }

        private void CreateAnimationDrivers()
        {
            var trailingNumbersRegex = new Regex(@"(\d+$)");

            var texture = Selection.GetFiltered<Texture2D>(SelectionMode.Assets).First();

            var path = AssetDatabase.GetAssetPath(texture);
            var sprites = AssetDatabase.LoadAllAssetsAtPath(path).OfType<Sprite>();
            
            var baseName = trailingNumbersRegex.Replace(texture.name, "animation_");
            var assetPath = Path.Combine(Path.GetDirectoryName(path) ?? Application.dataPath, baseName);

            if (!Directory.Exists(assetPath))
                Directory.CreateDirectory(assetPath);

            var cels = sprites
                .OrderBy(
                    sprite =>
                    {
                        var match = trailingNumbersRegex.Match(sprite.name);
                        return match.Success ? int.Parse(match.Groups[0].Captures[0].ToString()) : 0;
                    }
                )
                .Select(sprite => new SimpleCel(sprite))
                .ToList();

            var animationNodes = new List<ReanimatorNode>();
            
            for (var i = 0; i < cels.Count / _framesPerClip; i++)
            {
                var asset = SimpleAnimationNode.Create<SimpleAnimationNode>(
                    cels: cels.GetRange(i * _framesPerClip, _framesPerClip).ToArray(),
                    driver: new ControlDriver(_driverName, _autoIncrement)
                );
                
                asset.name = baseName + i;
                
                AssetDatabase.CreateAsset(asset, Path.Combine(assetPath, asset.name + ".asset"));
                
                animationNodes.Add(asset);
            }

            if (_constructSwitch)
            {
                var asset = SwitchNode.Create(
                    nodes: animationNodes.ToArray(),
                    driver: new ControlDriver(_switchDriverName, _switchAutoIncrement)
                );
                
                asset.name = baseName + "switch";
                
                AssetDatabase.CreateAsset(asset, Path.Combine(assetPath, asset.name + ".asset"));
            }

            AssetDatabase.SaveAssets();
        }
    }
}