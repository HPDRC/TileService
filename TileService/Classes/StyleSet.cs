using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;

namespace TileService
{
    public class StyleSet
    {
        public string name;
        public string bgColor;
        public int tileWidth;
        public int tileHeight;
        public List<Layer> layers = new List<Layer>();
        public Dictionary<string, int[]> layerGroups = new Dictionary<string, int[]>();
        public List<Mask> masks = new List<Mask>();

        public StyleSet(string name)
        {
            this.name = name;
            this.bgColor = Helper.GetStringConfig(name + "_bg_color");
            this.tileWidth = Helper.GetIntConfig(name + "_tile_width");
            this.tileHeight = Helper.GetIntConfig(name + "_tile_height");
            string layerListString = Helper.GetStringConfig(name + "_layer_list");
            string layerGroupString = Helper.GetStringConfig(name + "_layer_group");
            string renderGroupString = Helper.GetStringConfig(name + "_render_group");
            
            // parse layers
            foreach (string layerDesc in layerListString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
                layers.Add(new Layer(layerDesc));

            // every layer is a special layerGroup which only contains itself
            for (int i = 0; i < layers.Count; i++)
                layerGroups.Add(layers[i].name, new int[1] { i });

            // parse layerGroups
            foreach (string layerGroupDesc in layerGroupString.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] sections = layerGroupDesc.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                string groupName = sections[0].Trim();
                int[] layerIndex = new int[sections.Length-1];
                for (int i = 1; i < sections.Length; i++)
                {
                    string layerName = sections[i].Trim();
                    layerIndex[i - 1] = -1;
                    for (int j = 0; j < layers.Count; j++)
                    {
                        if (layers[j].name == layerName)
                        {
                            layerIndex[i-1] = j;
                            break;
                        }
                    }
                    if (layerIndex[i - 1] < 0)
                        throw new Exception("Group " + groupName + " specified a wrong layer name. Check web.config for error.");
                }

                // If a layerGroup has the same name as a layer, it will overwrite the definition of the layer
                layerGroups[groupName] = layerIndex;
            }

            // add a special "all" layer group
            int[] index = new int[layers.Count];
            for (int i = 0; i < layers.Count; i++)
                index[i] = i;
            layerGroups.Add("all", index);

            // parse render groups
            string[] renderGroups = renderGroupString.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            if (renderGroups.Length != 0)
            {
                for (int i = 0; i < renderGroups.Length; i++)
                    masks.Add(new Mask(i == 0 ? null : renderGroups[i - 1], renderGroups[i], layers));
                masks.Add(new Mask(renderGroups[renderGroups.Length - 1], null, layers));
            }
            else
                masks.Add(new Mask(null, null, layers));
        }

        public string EncodeLayer(string layersRequested, int zoom)
        {
            bool[] layerWanted = LayerToWantedArray(layersRequested, zoom);
            string result = "";
            foreach (bool isWanted in layerWanted)
                result += isWanted ? "1" : "0";
            return result;
        }

        public List<string> EncodeLayerByRenderGroup(string layersRequested, int zoom)
        {
            bool[] layerWanted = LayerToWantedArray(layersRequested, zoom);

            // apply masks
            List<string> results = new List<string>();
            for (int i = 0; i < masks.Count; i++)
            {
                string result = masks[i].GenerateLayerEncodeString(layerWanted);
                results.Add(result);
            }

            return results;
        }

        private bool[] LayerToWantedArray(string layersRequested, int zoom)
        {
            // get wanted layers array
            int n = layers.Count;
            bool[] layerWanted = new bool[n];
            for (int i = 0; i < n; i++)
                layerWanted[i] = false;
            foreach (string layer in layersRequested.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (!layerGroups.ContainsKey(layer))
                    continue;
                foreach (int index in layerGroups[layer])
                    layerWanted[index] = true;
            }

            // remove invisible layers
            for (int i = 0; i < n; i++)
                if (!layers[i].IsVisibleAt(zoom))
                    layerWanted[i] = false;

            return layerWanted;
        }
    }
}