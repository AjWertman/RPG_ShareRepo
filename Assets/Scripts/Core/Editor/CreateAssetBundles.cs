using UnityEditor;
using System.IO;

namespace RPGProject.Core
{
    /// <summary>
    /// Builds asset bundles in the editor.
    /// </summary>
    public class CreateAssetBundles
    {
        [MenuItem("Assets/Build AssetBundles")]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/Game/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
            {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            //Refactor - BuildTarget is set to windows, but could be mac, ios, console, etc
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        }
    }
}
