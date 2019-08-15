using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

    namespace Hammerplay.TinyArmy {
    public class PointsStore : MonoBehaviour {
        [SerializeField]
        private Button backButton;

        [SerializeField]
        private Button characterCategoryButton;

        [SerializeField]
        private Button backgroundCategoryButton;

        [SerializeField]
        private Button gunFrameCategoryButton;

        [SerializeField]
        private Button dogTagsCategoryButton;

        [SerializeField]
        private GameObject categoryPanel;

        [SerializeField]
        private GameObject[] pointStorePanels;

        [SerializeField]
        private PointStoreCategory selectedCategory = 0;

        private void OnEnable() {
            backButton.onClick.AddListener(OnBackButton);
            characterCategoryButton.onClick.AddListener(()=> OpenPanel(PointStoreCategory.Characters));
            backgroundCategoryButton.onClick.AddListener(() => OpenPanel(PointStoreCategory.Backgrounds));
            gunFrameCategoryButton.onClick.AddListener(() => OpenPanel(PointStoreCategory.GunFrames));
            dogTagsCategoryButton.onClick.AddListener(() => OpenPanel(PointStoreCategory.DogTags));
            if (pointStorePanels != null) {
                OpenPanel(selectedCategory);
            }
        }

        private void OnDisable() {
            backButton.onClick.RemoveAllListeners();
            characterCategoryButton.onClick.RemoveAllListeners();
            backgroundCategoryButton.onClick.RemoveAllListeners();
            gunFrameCategoryButton.onClick.RemoveAllListeners();
            dogTagsCategoryButton.onClick.RemoveAllListeners();
            selectedCategory = 0;
        }

        private void Awake() {
            pointStorePanels = new GameObject[categoryPanel.transform.childCount];
            for (int i = 0; i < pointStorePanels.Length; i++) {
                pointStorePanels[i] = categoryPanel.transform.GetChild(i).gameObject;
            }
            OpenPanel(selectedCategory);
        }

        private void OpenPanel(PointStoreCategory pointStoreCategory) {
            for (int i = 0; i < pointStorePanels.Length; i++) {
                pointStorePanels[i].SetActive(false);
            }
            pointStorePanels[(int)pointStoreCategory].SetActive(true);
            selectedCategory = pointStoreCategory;
        }

        private void OnBackButton() {
            MapSelectionPanel._instance.ToggleSelectionButton();
            //PlayerSelectionPanel._instance.ToggleSelectionButton();
            gameObject.SetActive(false);
        }
    }

    public enum PointStoreCategory {
        Characters,
        Backgrounds,
        GunFrames,
        DogTags
    }
}
