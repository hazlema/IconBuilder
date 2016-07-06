using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace IconBuilder {
    public class iconEntry {
        public byte   Width       { get; } = (byte)0;    // Image Width
        public byte   Height      { get; } = (byte)0;    // Image Width
        public byte   ColorCount  { get; } = (byte)0;    // Number of colors
        public byte   Reserved    { get; } = (byte)0;    // Always 0
        public Int16  Planes      { get; } = (Int16)1;   // 0 or 1, color planes
        public Int16  BitCount    { get; } = (Int16)32;  // Bits per pixel
        public Int32  BytesInRes  { get; }               // size of the image's data in bytes
        public Int32  ImageOffset { get; set; }          // offset of BMP or PNG data
        public byte[] imageBytes  { get; } = null;       // Internal use

        // Constructors
        public iconEntry() { } 
        public iconEntry(MemoryStream icoStream) {
            BinaryReader icoFile = new BinaryReader(icoStream);

            this.Width = icoFile.ReadByte();
            this.Height = icoFile.ReadByte();
            this.ColorCount  = icoFile.ReadByte();
            this.Reserved    = icoFile.ReadByte();
            this.Planes      = icoFile.ReadInt16();
            this.BitCount    = icoFile.ReadInt16();
            this.BytesInRes  = icoFile.ReadInt32();
            this.ImageOffset = icoFile.ReadInt32();
        }
        public iconEntry(Image img) {
            imageBytes = toByteArray(img);
            Width      = (byte)img.Width;
            Height     = (byte)img.Height;
            BytesInRes = imageBytes.Length;
        }

        // Function to convert to byte array
        public static byte[] toByteArray(Image img) {
            using (MemoryStream mStream = new MemoryStream()) {
                img.Save(mStream, ImageFormat.Png);
                return mStream.ToArray();
            }
        }
    }
}
