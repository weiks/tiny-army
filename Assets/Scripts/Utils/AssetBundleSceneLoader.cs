using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;

namespace Hammerplay.Utils {
    public class AssetBundleSceneLoader : MonoBehaviour {

		// Variables
		/*public string url;
		[Header("Loading Screen Settings")]
		[SerializeField]
		private Text loaderText;
        AssetBundle assetBundle;

        private void Awake() {
            
        }

        private void Start() {
            StartCoroutine(DownloadScene());
        }

        /// <summary>
        /// Downloads the asset bundle from server
        /// <Returns> Asset Bundle </Returns>
        /// </summary>
        IEnumerator DownloadScene() {
			loaderText.text = "Loading Scene";
			UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(url);
            
			while (!www.isDone) {
				
				loaderText.text = string.Format("Downloading ({0}%)", (www.downloadProgress * 100).ToString("00"));

			}
			yield return www.SendWebRequest();
			if (www.isNetworkError || www.isHttpError) {
				loaderText.text = www.error;
				Debug.Log(www.error);
            }
            else {
                assetBundle = DownloadHandlerAssetBundle.GetContent(www);
                string[] scenes = assetBundle.GetAllScenePaths();

                foreach (string sceneName in scenes) {
                    LoadAssetBundleScene(sceneName);
                }
            }
        }

        /// <summary>
        /// Loads the scene with downloaded asset bundle
        /// </summary>
        public void LoadAssetBundleScene(string sceneName) {
            SceneManager.LoadScene(sceneName);
        }*/
		public string url;

		[SerializeField]
		private Text loaderText;

		private Transform loaderParent;
		private bool isDownload = false;

		private void Start() {
			StartCoroutine(SceneLoad(url));
		}

		IEnumerator SceneLoad(string url) {
			using (UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(url)) {
				loaderText.text = "Trying the bundle download";
		
				isDownload = true;

				StartCoroutine(progress(request));

				yield return request.SendWebRequest();
				isDownload = false;

				if (request.isNetworkError || request.isHttpError) {
					loaderText.text = string.Format("Error loading {0}", request.error) ;
				}
				else {
					Debug.Log("Bundle downloaded");
					//save the asset bundle
					AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

					string[] scenes = bundle.GetAllScenePaths();

					foreach (string sceneName in scenes) {
						LoadAssetBundleScene(sceneName);
					}

				}
			}

			yield return null;
		}

		IEnumerator progress(UnityWebRequest req) {
			while (isDownload) {

				loaderText.text = string.Format("Downloading {0}%", (req.downloadProgress * 100).ToString("00.0"));
				yield return new WaitForSeconds(0.1f);
			}
		}

		public void LoadAssetBundleScene(string sceneName) {
			SceneManager.LoadScene(sceneName);
		}
	}
}
