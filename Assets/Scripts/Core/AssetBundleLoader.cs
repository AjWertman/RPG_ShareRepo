using System;
using System.Collections.Generic;
using UnityEngine;

public enum AssetBundlePath { None, abilities}

public class AssetBundleLoader : MonoBehaviour 
{
    Dictionary<AssetBundlePath, AssetBundle> assetBundles = new Dictionary<AssetBundlePath, AssetBundle>();
    const string bundlePath = "Assets/Game/AssetBundles";

    private void Awake()
    {
        CreateAssetBundles();
    }

    private void CreateAssetBundles()
    {
        int bundlePathLength = Enum.GetValues(typeof(AssetBundlePath)).Length;
        for (int i = 0; i < bundlePathLength; i++)
        {
            AssetBundlePath assetBundlePath = (AssetBundlePath)i;
            if (assetBundlePath == AssetBundlePath.None) continue;

            AssetBundle assetBundle= GetAssetBundle(assetBundlePath);

            if (assetBundle != null) assetBundles.Add(assetBundlePath, assetBundle);
        }
    }

    public object GetAssetFromBundle(Type _type, string _assetName, AssetBundlePath _assetBundlePath)
    {
        AssetBundle loadedAssetBundle = GetAssetBundle(_assetBundlePath);

        if (!loadedAssetBundle.Contains(_assetName))
        {
            Debug.Log("Does not contain asset");
            return null;
        }
        return loadedAssetBundle.LoadAsset(_assetName);
    }

    private AssetBundle GetAssetBundle(AssetBundlePath _assetBundlePath)
    {
        if (assetBundles.ContainsKey(_assetBundlePath)) return assetBundles[_assetBundlePath];

        AssetBundle loadedAssetBundle = AssetBundle.LoadFromFile(GetPath(_assetBundlePath));
        if (loadedAssetBundle == null)
        {
            Debug.Log("AssetBundle is null");
            return null;
        }
        return loadedAssetBundle;
    }

    private string GetPath(AssetBundlePath _assetBundlePath)
    {
        return (bundlePath + "/" + _assetBundlePath.ToString());
    }
}
