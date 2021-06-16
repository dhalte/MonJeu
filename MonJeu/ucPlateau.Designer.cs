
namespace MonJeu
{
  partial class ucPlateau
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

    #region Code généré par le Concepteur de composants

    /// <summary>
    /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
    /// le contenu de cette méthode avec l'éditeur de code.
    /// </summary>
    private void InitializeComponent()
    {
      this.SuspendLayout();
      // 
      // ucPlateau
      // 
      this.Click += new System.EventHandler(this.ucPlateau_Click);
      this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ucPlateau_KeyUp);
      this.Resize += new System.EventHandler(this.ucPlateau_Resize);
      this.ResumeLayout(false);

    }

    #endregion
  }
}
