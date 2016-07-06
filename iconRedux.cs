using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace IconBuilder  {
    public class iconRedux : IEnumerable {
        private MemoryStream    icoStream = new MemoryStream();
        private iconHeader      icoHeader  = new iconHeader();
        private List<iconEntry> icoEntries = new List<iconEntry>();

        public iconRedux() { }

        IEnumerator IEnumerable.GetEnumerator() {
            return (IEnumerator)GetEnumerator();
        }
        public IconEnum GetEnumerator() {
            return new IconEnum(icoEntries);
        }
      
        // TODO: 
        // Handle invalid icon files
        
        // Loading
        private bool readStream(string filename) {
            try {
                using (FileStream icoFile = new FileStream(filename, FileMode.Open, FileAccess.Read)) {
                    byte[] icoArray = new byte[icoFile.Length];
                    icoFile.Read(icoArray, 0, (int)icoFile.Length);
                    icoStream = new MemoryStream(icoArray);
                }
            } catch { return false; } finally { }

            return true;
        }
        public bool readFile(string filename) {

            if (this.readStream(filename)) {
                this.icoHeader = new iconHeader(icoStream);

                this.icoEntries.Clear();
                for (int counter = 0; counter < icoHeader.Count; counter++)
                    this.icoEntries.Add(new iconEntry(icoStream));

                return true;
            }

            return false;
        }

        // Properties
        public int Count { get { return icoEntries.Count; } }

        // Methods
        public Bitmap view(iconEntry i) {
            using (MemoryStream newIcon = new MemoryStream()) {
                BinaryWriter writer = new BinaryWriter(newIcon);

                // Write it
                #region Debug
                //Debug.WriteLine("");
                //Debug.WriteLine("Render Image - View");
                //Debug.Indent();
                //Debug.WriteLine($"Reserved: {icoHeader.Reserved}");
                //Debug.WriteLine($"    Type: {icoHeader.Type}");
                //Debug.WriteLine($"   Count: 1");
                //Debug.WriteLine($"   Width: {i.Width}");
                //Debug.WriteLine($"  Height: {i.Height}");
                //Debug.WriteLine($"  Colors: {i.ColorCount}");
                //Debug.WriteLine($"Reserved: {i.Reserved}");
                //Debug.WriteLine($"  Planes: {i.Planes}");
                //Debug.WriteLine($"BitCount: {i.BitCount}");
                //Debug.WriteLine($"  Length: {i.BytesInRes}");
                //Debug.WriteLine($"  Offset: 22");
                Debug.Unindent();
                #endregion
                writer.Write(icoHeader.Reserved);
                writer.Write(icoHeader.Type);
                writer.Write((Int16)1);                 // One Icon
                writer.Write(i.Width);
                writer.Write(i.Height);
                writer.Write(i.ColorCount);
                writer.Write(i.Reserved);
                writer.Write(i.Planes);
                writer.Write(i.BitCount);
                writer.Write(i.BytesInRes);
                writer.Write((Int32)22);                // Memory position

                // Grab the icon
                byte[] tmpBuffer = new byte[i.BytesInRes];
                icoStream.Position = i.ImageOffset;
                icoStream.Read(tmpBuffer, 0, i.BytesInRes);
                writer.Write(tmpBuffer);

                // Finish up
                writer.Flush();
                newIcon.Flush();
                icoStream.Position = 0;

                return new Bitmap(newIcon);
            }
        }
        public Bitmap view(int i) {
            return view(icoEntries[i]);
        }
        public iconEntry get(int i) {
            if (i <= icoEntries.Count)
                return icoEntries[i];

            return null;
        }
        public void build(Bitmap bmp, string filename) {
            int[]           Sizes  = { 128, 96, 72, 64, 48, 32, 24, 16 };
            FileStream      fs     = new FileStream(filename, FileMode.Create, FileAccess.Write);
            BinaryWriter    writer = new BinaryWriter(fs);
            List<iconEntry> Images = new List<iconEntry>();

            // Prepare Images
            foreach (int ndx in Sizes) {
                Image tmp = scaleAndCenter(bmp, ndx, ndx);
                Images.Add(new iconEntry(tmp));
            }

            // 6 Bytes for the header and 16 for the entry
            Int32 Offset = 6 + (16 * Images.Count);

            #region Debug
            //Debug.WriteLine("");
            //Debug.WriteLine("Render Image - Create");
            //Debug.Indent();
            //Debug.WriteLine($"Reserved: 0");
            //Debug.WriteLine($"    Type: 1");
            //Debug.WriteLine($"   Count: {Images.Count}");
            #endregion
            writer.Seek(0, SeekOrigin.Begin);
            writer.Write((Int16)0);
            writer.Write((Int16)1);
            writer.Write((Int16)Images.Count);

            // Write the icon entries
            foreach (iconEntry img in Images) {
                #region Debug
                //Debug.WriteLine("");
                //Debug.WriteLine($"   Width: {img.Width}");
                //Debug.WriteLine($"  Height: {img.Height}");
                //Debug.WriteLine($"  Colors: 0");
                //Debug.WriteLine($"Reserved: 0");
                //Debug.WriteLine($"  Planes: 1");
                //Debug.WriteLine($"BitCount: 32");
                //Debug.WriteLine($"  Length: {img.imageBytes.Length}");
                //Debug.WriteLine($"  Offset: {Offset}");
                #endregion
                writer.Write((byte)img.Width);
                writer.Write((byte)img.Height);
                writer.Write((byte)0);
                writer.Write((byte)0);
                writer.Write((Int16)1);
                writer.Write((Int16)32);
                writer.Write((Int32)img.imageBytes.Length);
                writer.Write((Int32)Offset);

                Offset += (Int32)img.imageBytes.Length;
            }

            foreach (iconEntry img in Images) 
                writer.Write(img.imageBytes);

            //Debug.Unindent();
            writer.Close();
            fs.Close();
        }
        public Image scaleAndCenter(Image image, int maxWidth, int maxHeight) {
            // Scale
            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);

            // Center
            var startX = (Math.Abs(maxWidth - newWidth) / 2);
            var startY = (Math.Abs(maxHeight - newHeight) / 2);

            var centered = new Bitmap(maxWidth, maxHeight);
            using (var graphics = Graphics.FromImage(centered))
                graphics.DrawImage(newImage, startX, startY, newWidth, newHeight);

            return centered;
        }

        // IEnumerable
        public class IconEnum : IEnumerator {
            public List<iconEntry> icoEntries;
            int position = -1;

            public IconEnum(List<iconEntry> list) { icoEntries = list; }
            public bool MoveNext() { position++; return (position < icoEntries.Count); }
            public void Reset() { position = -1; }
            object IEnumerator.Current { get { return Current; } }

            public iconEntry Current {
                get {
                    try { return icoEntries[position]; } catch (IndexOutOfRangeException) { throw new InvalidOperationException(); }
                }
            }
        }
    }
}
