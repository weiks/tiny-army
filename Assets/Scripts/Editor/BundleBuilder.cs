using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class BundleBuilder : Editor {

	public static string assetBundlePath = @"Assets/AssetBundle/public/assetbundles/";

	[MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles() {
		if (!Directory.Exists(assetBundlePath)) {
			//if it doesn't, create it
			Directory.CreateDirectory(assetBundlePath);
		}
		
        BuildPipeline.BuildAssetBundles(assetBundlePath, BuildAssetBundleOptions.None, BuildTarget.Android);
		AssetDatabase.Refresh();
    }
}
