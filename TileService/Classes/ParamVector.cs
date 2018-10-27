using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TileService
{
    public class ParamVector : Param
    {
        public int x, y, z;
        public string layers;
        public StyleSet styleset;

        public ParamVector(HttpRequest request)
            : base(request)
        {
            styleset = Config.styleSets[GetStringParam("styleset").ToLower()];
            x = GetIntParam("x");
            y = GetIntParam("y");
            z = GetIntParam("z");
            layers = GetStringParam("layers").ToLower();
        }

        /// <summary>
        /// returned result contains empty strings which should not be visited at all
        /// </summary>
        /// <returns></returns>
        public string[] GetQueryStringsByRenderGroup()
        {
            List<string> encodeStrings = styleset.EncodeLayerByRenderGroup(layers, z);
            string[] result = new string[encodeStrings.Count];
            for (int i = 0; i < encodeStrings.Count; i++)
            {
                if (encodeStrings[i] != "")
                    result[i] = styleset.name + "/" + encodeStrings[i] + "/" + z + "/" + x + "/" + y + ".png";
                else
                    result[i] = "";
            }
            return result;
        }
    }
}