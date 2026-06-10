namespace SistemaMultimedia.Forms
{
    partial class FrmEnviarCorreo
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEnviarCorreo));
            txtTo = new TextBox();
            txtSubject = new TextBox();
            txtBody = new RichTextBox();
            lblTo = new Label();
            lblSubject = new Label();
            lblMessage = new Label();
            lblAttachment = new Label();
            btnSend = new Button();
            btnCancel = new Button();
            progressBar = new ProgressBar();
            panelHeader = new Panel();
            lblHeader = new Label();
            pbStatus = new PictureBox();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbStatus).BeginInit();
            SuspendLayout();
            // 
            // txtTo
            // 
            txtTo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtTo.ForeColor = Color.Black;
            txtTo.Location = new Point(126, 90);
            txtTo.Margin = new Padding(4, 5, 4, 5);
            txtTo.Name = "txtTo";
            txtTo.Size = new Size(684, 31);
            txtTo.TabIndex = 0;
            // 
            // txtSubject
            // 
            txtSubject.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtSubject.ForeColor = Color.Black;
            txtSubject.Location = new Point(126, 140);
            txtSubject.Margin = new Padding(4, 5, 4, 5);
            txtSubject.Name = "txtSubject";
            txtSubject.Size = new Size(684, 31);
            txtSubject.TabIndex = 1;
            // 
            // txtBody
            // 
            txtBody.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            txtBody.ForeColor = Color.Black;
            txtBody.Location = new Point(18, 211);
            txtBody.Margin = new Padding(4, 5, 4, 5);
            txtBody.Name = "txtBody";
            txtBody.Size = new Size(793, 212);
            txtBody.TabIndex = 2;
            txtBody.Text = "";
            // 
            // lblTo
            // 
            lblTo.AutoSize = true;
            lblTo.Location = new Point(18, 94);
            lblTo.Margin = new Padding(4, 0, 4, 0);
            lblTo.Name = "lblTo";
            lblTo.Size = new Size(106, 25);
            lblTo.TabIndex = 3;
            lblTo.Text = "Para (email):";
            // 
            // lblSubject
            // 
            lblSubject.AutoSize = true;
            lblSubject.Location = new Point(45, 140);
            lblSubject.Margin = new Padding(4, 0, 4, 0);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(73, 25);
            lblSubject.TabIndex = 4;
            lblSubject.Text = "Asunto:";
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Location = new Point(18, 181);
            lblMessage.Margin = new Padding(4, 0, 4, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(81, 25);
            lblMessage.TabIndex = 5;
            lblMessage.Text = "Mensaje:";
            // 
            // lblAttachment
            // 
            lblAttachment.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblAttachment.ForeColor = Color.White;
            lblAttachment.Location = new Point(18, 428);
            lblAttachment.Margin = new Padding(4, 0, 4, 0);
            lblAttachment.Name = "lblAttachment";
            lblAttachment.Size = new Size(794, 39);
            lblAttachment.TabIndex = 6;
            lblAttachment.Text = "(archivo adjunto)";
            lblAttachment.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // btnSend
            // 
            btnSend.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSend.BackColor = Color.White;
            btnSend.FlatStyle = FlatStyle.Flat;
            btnSend.ForeColor = Color.Black;
            btnSend.ImageAlign = ContentAlignment.MiddleLeft;
            btnSend.Location = new Point(555, 471);
            btnSend.Margin = new Padding(4, 5, 4, 5);
            btnSend.Name = "btnSend";
            btnSend.Size = new Size(121, 45);
            btnSend.TabIndex = 7;
            btnSend.Text = "Enviar";
            btnSend.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnSend.UseVisualStyleBackColor = false;
            btnSend.Click += btnSend_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.BackColor = Color.White;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.Black;
            btnCancel.Location = new Point(685, 471);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(121, 45);
            btnCancel.TabIndex = 8;
            btnCancel.Text = "Cancelar";
            btnCancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // progressBar
            // 
            progressBar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            progressBar.ForeColor = Color.Black;
            progressBar.Location = new Point(18, 471);
            progressBar.Margin = new Padding(4, 5, 4, 5);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(529, 45);
            progressBar.TabIndex = 9;
            // 
            // panelHeader
            // 
            panelHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelHeader.BackColor = Color.FromArgb(30, 30, 30);
            panelHeader.Controls.Add(lblHeader);
            panelHeader.Controls.Add(pbStatus);
            panelHeader.Location = new Point(0, 0);
            panelHeader.Margin = new Padding(4, 5, 4, 5);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(829, 80);
            panelHeader.TabIndex = 10;
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = true;
            lblHeader.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblHeader.ForeColor = Color.White;
            lblHeader.Location = new Point(18, 21);
            lblHeader.Margin = new Padding(4, 0, 4, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(274, 30);
            lblHeader.TabIndex = 11;
            lblHeader.Text = "Enviar archivo por correo";
            // 
            // pbStatus
            // 
            pbStatus.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pbStatus.Location = new Point(742, 5);
            pbStatus.Margin = new Padding(4, 5, 4, 5);
            pbStatus.Name = "pbStatus";
            pbStatus.Size = new Size(69, 66);
            pbStatus.SizeMode = PictureBoxSizeMode.CenterImage;
            pbStatus.TabIndex = 12;
            pbStatus.TabStop = false;
            // 
            // FrmEnviarCorreo
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(829, 538);
            Controls.Add(panelHeader);
            Controls.Add(progressBar);
            Controls.Add(btnCancel);
            Controls.Add(btnSend);
            Controls.Add(lblAttachment);
            Controls.Add(lblMessage);
            Controls.Add(lblSubject);
            Controls.Add(lblTo);
            Controls.Add(txtBody);
            Controls.Add(txtSubject);
            Controls.Add(txtTo);
            Font = new Font("Segoe UI", 9F);
            ForeColor = Color.White;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmEnviarCorreo";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Enviar archivo por correo";
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbStatus).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TextBox txtTo;
        private System.Windows.Forms.TextBox txtSubject;
        private System.Windows.Forms.RichTextBox txtBody;
        private System.Windows.Forms.Label lblTo;
        private System.Windows.Forms.Label lblSubject;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblAttachment;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Panel panelHeader;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.PictureBox pbStatus;
    }
}
