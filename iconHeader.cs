using System;
using System.IO;

namespace IconBuilder {
    public class iconHeader {
        public Int16 Reserved { get; } = (Int16)0;   // Always 0
        public Int16 Type     { get; } = (Int16)1;   // 1 for .ICO, 2 for cursor
        public Int16 Count    { get; } = (Int16)1;   // Starts with 1

        // Constructors
        public iconHeader() { }
        public iconHeader(MemoryStream icoStream) {
            BinaryReader icoFile = new BinaryReader(icoStream);

            this.Reserved = icoFile.ReadInt16();
            this.Type     = icoFile.ReadInt16();
            this.Count    = icoFile.ReadInt16();
        }
    }
}
