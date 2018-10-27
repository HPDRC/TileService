using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileService
{
    public class Mask
    {
        private bool[] maskBits;

        public Mask(string maskStart, string maskEnd, List<Layer> layers)
        {
            int s = -1, e = -1;
            for (int i = 0; i < layers.Count; i++)
            {
                if (maskStart != null && layers[i].name == maskStart)
                    s = i + 1;
                else if (maskEnd != null && layers[i].name == maskEnd)
                    e = i;
            }
            if (maskStart == null)
                s = 0;
            if (maskEnd == null)
                e = layers.Count - 1;
            if (s == -1 || e == -1)
                throw new Exception("Mask: layer not found: " + maskStart + ", " + maskEnd);
            if (s > e)
                throw new Exception("Mask: end layer is in front of start layer: " + maskStart + ", " + maskEnd);

            maskBits = new bool[layers.Count];
            for (int i = 0; i < layers.Count; i++)
            {
                if (i < s || i > e)
                    maskBits[i] = false;
                else
                    maskBits[i] = true;
            }
        }

        /// <summary>
        /// return encoded string if success or empty string "" if no layer is wanted
        /// </summary>
        /// <param name="layerWanted"></param>
        /// <returns></returns>
        public string GenerateLayerEncodeString(bool[] layerWanted)
        {
            if (layerWanted.Length != maskBits.Length)
                throw new Exception("Mask: length doesn't match!");

            // generate encoded string
            string result = "";
            bool isAllZero = true;
            for (int i = 0; i < layerWanted.Length; i++)
            {
                if (layerWanted[i] && maskBits[i])
                {
                    result += "1";
                    isAllZero = false;
                }
                else
                    result += "0";
            }
            return isAllZero ? "" : result;
        }
    }
}