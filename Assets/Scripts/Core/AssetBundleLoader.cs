using UnityEngine;

public enum AssetBundlePath { None, abilities}

public class AssetBundleLoader : MonoBehaviour
{
    const string bundlePath = "Assets/Game/AssetBundles";

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            print(GetAssetFromBundle("Turret", AssetBundlePath.abilities));
            //var prefab = myLoadedAssetBundle.LoadAsset<GameObject>(_assetBundle);
        }
    }

    public object GetAssetFromBundle(string _assetName, AssetBundlePath _assetBundlePath)
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
