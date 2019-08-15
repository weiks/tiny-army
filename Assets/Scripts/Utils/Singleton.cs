﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils
{
    public class Singleton<T> : MonoBehaviour where T : Component
    {
        protected static T _instance;

        public static T Instance {
            get {
                if (_instance == null) {
                    _instance = FindObjectOfType<T>();
                    if (_instance == null) {
                        GameObject obj = new GameObject();
                        _instance = obj.AddComponent<T>();
                    }
                }
                return _instance;
            }
        }

        // If you are using awake in the base class, make sure to call base.Awake() before everything.
        protected virtual void Awake() {
            if (!Application.isPlaying) {
                return;
            }
            _instance = this as T;
        }
    }
}
