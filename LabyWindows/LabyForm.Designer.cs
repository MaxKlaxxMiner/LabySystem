namespace LabyWindows
{
  partial class LabyForm
  {
    /// <summary>
    /// Erforderliche Designervariable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Verwendete Ressourcen bereinigen.
    /// </summary>
    /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Vom Windows Form-Designer generierter Code

    /// <summary>
    /// Erforderliche Methode für die Designerunterstützung.
    /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
    /// </summary>
    private void InitializeComponent()
    {
      this.components = new System.ComponentModel.Container();
      this.gamePictureBox1 = new System.Windows.Forms.PictureBox();
      this.timer1 = new System.Windows.Forms.Timer(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.gamePictureBox1)).BeginInit();
      this.SuspendLayout();
      // 
      // gamePictureBox1
      // 
      this.gamePictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gamePictureBox1.Location = new System.Drawing.Point(0, 0);
      this.gamePictureBox1.Name = "gamePictureBox1";
      this.gamePictureBox1.Size = new System.Drawing.Size(1142, 600);
      this.gamePictureBox1.TabIndex = 0;
      this.gamePictureBox1.TabStop = false;
      // 
      // timer1
      // 
      this.timer1.Enabled = true;
      this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
      // 
      // LabyForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1142, 600);
      this.Controls.Add(this.gamePictureBox1);
      this.Name = "LabyForm";
      this.Text = "LabyWindows v0.01";
      this.Load += new System.EventHandler(this.LabyForm_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.LabyForm_KeyDown);
      ((System.ComponentModel.ISupportInitialize)(this.gamePictureBox1)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PictureBox gamePictureBox1;
    private System.Windows.Forms.Timer timer1;
  }
}

