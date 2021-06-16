
namespace MonJeu
{
  partial class MainForm
  {
    /// <summary>
    /// Variable nécessaire au concepteur.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Nettoyage des ressources utilisées.
    /// </summary>
    /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Code généré par le Concepteur Windows Form

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.ucPlateau = new MonJeu.ucPlateau();
      this.mainLayout = new System.Windows.Forms.TableLayoutPanel();
      this.ucSymboles = new MonJeu.ucSymboles();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.mnuFichier = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuJeu = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuSaisir = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuJouer = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuNouveau = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuSaisirNouveau = new System.Windows.Forms.ToolStripMenuItem();
      this.mainLayout.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // ucPlateau
      // 
      this.mainLayout.SetColumnSpan(this.ucPlateau, 3);
      this.ucPlateau.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucPlateau.Location = new System.Drawing.Point(3, 3);
      this.ucPlateau.Name = "ucPlateau";
      this.ucPlateau.Size = new System.Drawing.Size(1034, 461);
      this.ucPlateau.TabIndex = 0;
      this.ucPlateau.Text = "ucPlateau1";
      // 
      // mainLayout
      // 
      this.mainLayout.ColumnCount = 3;
      this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 364F));
      this.mainLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
      this.mainLayout.Controls.Add(this.ucPlateau, 0, 0);
      this.mainLayout.Controls.Add(this.ucSymboles, 1, 1);
      this.mainLayout.Dock = System.Windows.Forms.DockStyle.Fill;
      this.mainLayout.Location = new System.Drawing.Point(0, 24);
      this.mainLayout.Name = "mainLayout";
      this.mainLayout.RowCount = 2;
      this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
      this.mainLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 160F));
      this.mainLayout.Size = new System.Drawing.Size(1040, 627);
      this.mainLayout.TabIndex = 1;
      // 
      // ucSymboles
      // 
      this.ucSymboles.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucSymboles.Location = new System.Drawing.Point(341, 470);
      this.ucSymboles.Name = "ucSymboles";
      this.ucSymboles.Size = new System.Drawing.Size(358, 154);
      this.ucSymboles.TabIndex = 1;
      this.ucSymboles.Text = "ucSymboles1";
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFichier,
            this.mnuJeu});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1040, 24);
      this.menuStrip1.TabIndex = 2;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // mnuFichier
      // 
      this.mnuFichier.Name = "mnuFichier";
      this.mnuFichier.Size = new System.Drawing.Size(54, 20);
      this.mnuFichier.Text = "&Fichier";
      // 
      // mnuJeu
      // 
      this.mnuJeu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSaisirNouveau,
            this.mnuNouveau,
            this.mnuSaisir,
            this.mnuJouer});
      this.mnuJeu.Name = "mnuJeu";
      this.mnuJeu.Size = new System.Drawing.Size(36, 20);
      this.mnuJeu.Text = "&Jeu";
      // 
      // mnuSaisir
      // 
      this.mnuSaisir.Name = "mnuSaisir";
      this.mnuSaisir.Size = new System.Drawing.Size(180, 22);
      this.mnuSaisir.Text = "&Saisir";
      this.mnuSaisir.Click += new System.EventHandler(this.mnuSaisir_Click);
      // 
      // mnuJouer
      // 
      this.mnuJouer.Name = "mnuJouer";
      this.mnuJouer.Size = new System.Drawing.Size(180, 22);
      this.mnuJouer.Text = "&Jouer";
      this.mnuJouer.Click += new System.EventHandler(this.mnuJouer_Click);
      // 
      // mnuNouveau
      // 
      this.mnuNouveau.Name = "mnuNouveau";
      this.mnuNouveau.Size = new System.Drawing.Size(180, 22);
      this.mnuNouveau.Text = "&Nouveau";
      this.mnuNouveau.Click += new System.EventHandler(this.mnuNouveau_Click);
      // 
      // mnuSaisirNouveau
      // 
      this.mnuSaisirNouveau.Name = "mnuSaisirNouveau";
      this.mnuSaisirNouveau.Size = new System.Drawing.Size(180, 22);
      this.mnuSaisirNouveau.Text = "Saisir nouveau";
      this.mnuSaisirNouveau.Click += new System.EventHandler(this.mnuSaisirNouveau_Click);
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1040, 651);
      this.Controls.Add(this.mainLayout);
      this.Controls.Add(this.menuStrip1);
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "MainForm";
      this.Text = "Mon MonJeu";
      this.Load += new System.EventHandler(this.Form1_Load);
      this.mainLayout.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private ucPlateau ucPlateau;
    private System.Windows.Forms.TableLayoutPanel mainLayout;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem mnuFichier;
    private System.Windows.Forms.ToolStripMenuItem mnuJeu;
    private System.Windows.Forms.ToolStripMenuItem mnuSaisir;
    private System.Windows.Forms.ToolStripMenuItem mnuJouer;
    private ucSymboles ucSymboles;
    private System.Windows.Forms.ToolStripMenuItem mnuNouveau;
    private System.Windows.Forms.ToolStripMenuItem mnuSaisirNouveau;
  }
}

