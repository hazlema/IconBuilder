using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace IconBuilder {
    public partial class IconBuilder : Form {

        private int current = 0;
        private iconRedux icon = new iconRedux();

        [STAThread]
        static void Main(string[] args) {
            if (args.Length == 0)
                Application.Run(new IconBuilder(""));
            else
                Application.Run(new IconBuilder(string.Join(" ", args)));
        }

        // Load icon from command line
        public IconBuilder(string file) {  
            InitializeComponent();
            if (file != "" && File.Exists(file)) view(file);
        } 

        // Load the icon into a picture box
        private void view(string file) {
            btnPrev.Enabled = true;
            btnNext.Enabled = true;

            icon.readFile(file);   // Load the icon file
            view(0);               // calls view(int i) (Below)
        }
        private void view(int i) {
            current = i;           // Resets the Prev / Next Index
            view();                // calls view() (Below)
        }
        private void view() {
            pic.Image = icon.view(current);    // Load the current icon index into the Image
            var thisIcon = icon.get(current);  // Grab Info about the icon
            txtSize.Text = $"{thisIcon.Width} x {thisIcon.Height}";
        }

        // Utility function for the DragDrop event & FileMenu Events
        private void importFiles(string[] files) {
            foreach (string file in files) {
                if (Path.GetExtension(file).ToUpper() == ".ICO")
                    view(file);

                if ((Path.GetExtension(file).ToUpper() == ".JPG") ||
                    (Path.GetExtension(file).ToUpper() == ".BMP") ||
                    (Path.GetExtension(file).ToUpper() == ".PNG") ||
                    (Path.GetExtension(file).ToUpper() == ".GIF")) {

                    string ext = Path.GetExtension(file);
                    string name = file.Replace(ext, ".ico");

                    icon.build(new Bitmap(file), name);
                    view(name);
                }
            }
        }

        // UI: File Dialog
        private void mnuViewOrImport(object sender, EventArgs e) {
            string btn = ((Button)sender).Text;
            Console.WriteLine(btn);

            using (OpenFileDialog open = new OpenFileDialog()) {
                if (btn == "&View") {
                    open.Multiselect = false;
                    open.DefaultExt = "*.ico";
                    open.Filter = "ICO Files|*.ico";
                } else {
                    open.Multiselect = true;
                    open.DefaultExt = "*.bmp;*.png;*.gif;*.jpg;*.jpeg";
                    open.Filter = "All Formats|*.bmp;*.png;*.gif;*.jpg;*.jpeg|" +
                                  "Bitmap Graphics (*.bmp)|*.bmp|" +
                                  "Portable Network Graphics (*.png)|*.png|" +
                                  "Joint Photographic Experts Group (*.jpg, *.jpeg)|*.jpg;*.jpeg|" +
                                  "Graphic Interchange Format (*.gif)|*.gif";
                }

                if (open.ShowDialog() == DialogResult.OK)
                    if (btn == "&View") view(open.FileName); else importFiles(open.FileNames);
            }
        }

        // UI: Next, Previous Image
        private void NextPrev(object sender, EventArgs e) {
            string cmd = ((Button)sender).Text;

            if (cmd == ">>") current--; else current++;
            if (current == -1) current = icon.Count - 1;
            if (current == icon.Count) current = 0;
            view();
        }

        // DD: Drag an ICO View it or a picture(s) convert
        private void IconBuilder_DragDrop(object sender, DragEventArgs e) {
            string[] Files = (string[])e.Data.GetData(DataFormats.FileDrop);
            importFiles(Files);
        }

        // DD: Drag and Drop ON
        private void IconBuilder_DragEnter(object sender, DragEventArgs e) {
            e.Effect = DragDropEffects.All;
        }
    }
}

