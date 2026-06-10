namespace SistemaMultimedia.Forms
{
    partial class GalleryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.ListView listViewGallery;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GalleryForm));
            listViewGallery = new ListView();
            SuspendLayout();
            // 
            // listViewGallery
            // 
            listViewGallery.BackColor = Color.FromArgb(30, 30, 30);
            resources.ApplyResources(listViewGallery, "listViewGallery");
            listViewGallery.ForeColor = Color.White;
            listViewGallery.Name = "listViewGallery";
            listViewGallery.UseCompatibleStateImageBehavior = false;
            listViewGallery.View = View.List;
            // 
            // GalleryForm
            // 
            BackColor = Color.FromArgb(30, 30, 30);
            resources.ApplyResources(this, "$this");
            Controls.Add(listViewGallery);
            ForeColor = Color.White;
            Name = "GalleryForm";
            ResumeLayout(false);
        }
    }
}
