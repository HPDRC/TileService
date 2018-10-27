using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.IO;

namespace TileService
{
    public class PngImage
    {
        private Bitmap final;
        Graphics g;

        public PngImage(int width, int height, string bgColor)
        {
            // prepare final image
            final = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);
            g = Graphics.FromImage(final);
            if (bgColor != "")
                g.Clear(ColorTranslator.FromHtml(bgColor));
            g.CompositingMode = CompositingMode.SourceOver;
        }

        public void Add(Stream imageStream)
        {
            // copy images onto the final image
            Point origin = new Point(0, 0);
            using (Image tmp = Image.FromStream(imageStream, false, false))
            {
                g.DrawImage(tmp, origin);
            }
        }

        public byte[] Save()
        {
            byte[] byteArray = null;
            using (MemoryStream stream = new MemoryStream())
            {
                final.Save(stream, ImageFormat.Png);
                stream.Close();
                byteArray = stream.ToArray();
            }
            return byteArray;
        }
    }
}