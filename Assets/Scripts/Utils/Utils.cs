using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Hammerplay.Utils {
	public static class Misc {

		public static float Interpolate(float input, float inputMin, float inputMax, float outputMin, float outputMax) {
			return outputMin + (((input - inputMin) * (outputMax - outputMin)) / (inputMax - inputMin));
		}

        public static float Interpolate(float input, float inputMin, float inputMax) {
            return Interpolate(input, inputMin, inputMax, 0, 1);
        }

    }
}
