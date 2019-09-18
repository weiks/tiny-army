using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Hammerplay.TinyArmy {
public class CloudVariables : MonoBehaviour {

	    public static int Highscore { get; set; }
        public static int[] ImportantValues { get; set; }

        private void Awake(){
            ImportantValues = new int[3]; 
        }
    }
}

