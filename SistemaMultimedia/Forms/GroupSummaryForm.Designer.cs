namespace SistemaMultimedia.Forms
{
    partial class GroupSummaryForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.DataGridView ddvSummary;
        private System.Windows.Forms.Button btnMostrar;
        private System.Windows.Forms.Button btnMostrarTodos;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartSummary;
        private System.Windows.Forms.DataVisualization.Charting.Chart chartTotal;

        // Controles reorganizados
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabDatos;
        private System.Windows.Forms.TabPage tabCantidad;
        private System.Windows.Forms.TabPage tabTotal;
        private System.Windows.Forms.Panel panelButtons;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupSummaryForm));
            ddvSummary = new DataGridView();
            btnMostrar = new Button();
            btnMostrarTodos = new Button();
            btnCerrar = new Button();
            chartSummary = new System.Windows.Forms.DataVisualization.Charting.Chart();
            chartTotal = new System.Windows.Forms.DataVisualization.Charting.Chart();
            tabMain = new TabControl();
            tabDatos = new TabPage();
            tabCantidad = new TabPage();
            tabTotal = new TabPage();
            panelButtons = new Panel();
            ((System.ComponentModel.ISupportInitialize)ddvSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartSummary).BeginInit();
            ((System.ComponentModel.ISupportInitialize)chartTotal).BeginInit();
            tabMain.SuspendLayout();
            tabDatos.SuspendLayout();
            tabCantidad.SuspendLayout();
            tabTotal.SuspendLayout();
            panelButtons.SuspendLayout();
            SuspendLayout();
            // 
            // ddvSummary
            // 
            ddvSummary.BackgroundColor = Color.FromArgb(40, 40, 40);
            ddvSummary.ColumnHeadersHeight = 29;
            ddvSummary.Dock = DockStyle.Fill;
            ddvSummary.Location = new Point(3, 3);
            ddvSummary.Name = "ddvSummary";
            ddvSummary.RowHeadersWidth = 51;
            ddvSummary.Size = new Size(886, 506);
            ddvSummary.TabIndex = 0;
            // 
            // btnMostrar
            // 
            btnMostrar.Cursor = Cursors.Hand;
            btnMostrar.ForeColor = Color.Black;
            btnMostrar.Location = new Point(382, 10);
            btnMostrar.Name = "btnMostrar";
            btnMostrar.Size = new Size(147, 37);
            btnMostrar.TabIndex = 1;
            btnMostrar.Text = "Mostrar";
            btnMostrar.UseVisualStyleBackColor = true;
            btnMostrar.Click += btnMostrar_Click;
            // 
            // btnMostrarTodos
            // 
            btnMostrarTodos.Cursor = Cursors.Hand;
            btnMostrarTodos.ForeColor = Color.Black;
            btnMostrarTodos.Location = new Point(108, 10);
            btnMostrarTodos.Name = "btnMostrarTodos";
            btnMostrarTodos.Size = new Size(147, 37);
            btnMostrarTodos.TabIndex = 2;
            btnMostrarTodos.Text = "Mostrar todo";
            btnMostrarTodos.UseVisualStyleBackColor = true;
            // 
            // btnCerrar
            // 
            btnCerrar.Cursor = Cursors.Hand;
            btnCerrar.ForeColor = Color.Black;
            btnCerrar.Location = new Point(656, 10);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(147, 37);
            btnCerrar.TabIndex = 3;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = true;
            // 
            // chartSummary
            // 
            chartArea1.AxisX.Interval = 1D;
            chartArea1.AxisX.LabelStyle.Angle = -45;
            chartArea1.AxisY.LabelStyle.Format = "N0";
            chartArea1.Name = "AreaCantidad";
            chartSummary.ChartAreas.Add(chartArea1);
            chartSummary.Dock = DockStyle.Fill;
            chartSummary.Location = new Point(3, 3);
            chartSummary.Name = "chartSummary";
            series1.ChartArea = "AreaCantidad";
            series1.IsValueShownAsLabel = true;
            series1.Name = "Cantidad";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Int32;
            chartSummary.Series.Add(series1);
            chartSummary.Size = new Size(886, 506);
            chartSummary.TabIndex = 4;
            chartSummary.Text = "chartSummary";
            // 
            // chartTotal
            // 
            chartArea2.AxisX.Interval = 1D;
            chartArea2.AxisX.LabelStyle.Angle = -45;
            chartArea2.AxisY.LabelStyle.Format = "C2";
            chartArea2.Name = "AreaTotal";
            chartTotal.ChartAreas.Add(chartArea2);
            chartTotal.Dock = DockStyle.Fill;
            chartTotal.Location = new Point(3, 3);
            chartTotal.Name = "chartTotal";
            series2.ChartArea = "AreaTotal";
            series2.IsValueShownAsLabel = true;
            series2.Label = "#VALY{N2}";
            series2.Name = "Total";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.String;
            series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
            chartTotal.Series.Add(series2);
            chartTotal.Size = new Size(886, 506);
            chartTotal.TabIndex = 5;
            chartTotal.Text = "chartTotal";
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabDatos);
            tabMain.Controls.Add(tabCantidad);
            tabMain.Controls.Add(tabTotal);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(900, 550);
            tabMain.TabIndex = 0;
            // 
            // tabDatos
            // 
            tabDatos.Controls.Add(ddvSummary);
            tabDatos.Location = new Point(4, 34);
            tabDatos.Name = "tabDatos";
            tabDatos.Padding = new Padding(3);
            tabDatos.Size = new Size(892, 512);
            tabDatos.TabIndex = 0;
            tabDatos.Text = "Datos";
            tabDatos.UseVisualStyleBackColor = true;
            // 
            // tabCantidad
            // 
            tabCantidad.Controls.Add(chartSummary);
            tabCantidad.Location = new Point(4, 34);
            tabCantidad.Name = "tabCantidad";
            tabCantidad.Padding = new Padding(3);
            tabCantidad.Size = new Size(892, 512);
            tabCantidad.TabIndex = 1;
            tabCantidad.Text = "Cantidad";
            tabCantidad.UseVisualStyleBackColor = true;
            // 
            // tabTotal
            // 
            tabTotal.Controls.Add(chartTotal);
            tabTotal.Location = new Point(4, 34);
            tabTotal.Name = "tabTotal";
            tabTotal.Padding = new Padding(3);
            tabTotal.Size = new Size(892, 512);
            tabTotal.TabIndex = 2;
            tabTotal.Text = "Total";
            tabTotal.UseVisualStyleBackColor = true;
            // 
            // panelButtons
            // 
            panelButtons.Controls.Add(btnMostrarTodos);
            panelButtons.Controls.Add(btnMostrar);
            panelButtons.Controls.Add(btnCerrar);
            panelButtons.Dock = DockStyle.Bottom;
            panelButtons.Location = new Point(0, 550);
            panelButtons.Name = "panelButtons";
            panelButtons.Size = new Size(900, 50);
            panelButtons.TabIndex = 2;
            // 
            // GroupSummaryForm
            // 
            BackColor = Color.FromArgb(30, 30, 30);
            ClientSize = new Size(900, 600);
            Controls.Add(tabMain);
            Controls.Add(panelButtons);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "GroupSummaryForm";
            Text = "Resumen por categoría";
            Load += GroupSummaryForm_Load;
            ((System.ComponentModel.ISupportInitialize)ddvSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartSummary).EndInit();
            ((System.ComponentModel.ISupportInitialize)chartTotal).EndInit();
            tabMain.ResumeLayout(false);
            tabDatos.ResumeLayout(false);
            tabCantidad.ResumeLayout(false);
            tabTotal.ResumeLayout(false);
            panelButtons.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}