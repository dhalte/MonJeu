using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonJeu
{
  public partial class MainForm : Form
  {
    public MainForm()
    {
      InitializeComponent();
    }
    private float symboleHeight;
    private void Form1_Load(object sender, EventArgs e)
    {
      symboleHeight = mainLayout.RowStyles[1].Height;
      mainLayout.RowStyles[1].Height = 0;
      ucPlateau.InitNouveauJeu(52, Properties.Resources.Jeu52cartes);
      ucSymboles.parent = ucPlateau;
    }

    private void mnuSaisir_Click(object sender, EventArgs e)
    {
      ucPlateau.InitSaisie(52, Properties.Resources.Jeu52cartes);
      ucSymboles.Init(52, Properties.Resources.Symboles, ucPlateau.Situation);
      mainLayout.RowStyles[1].Height = symboleHeight;
    }

    private void mnuJouer_Click(object sender, EventArgs e)
    {
      mainLayout.RowStyles[1].Height = 0;
      ucPlateau.InitJeuApresSaisie();
    }

    private void mnuNouveau_Click(object sender, EventArgs e)
    {
      mainLayout.RowStyles[1].Height = 0;
      ucPlateau.InitNouveauJeu(52, Properties.Resources.Jeu52cartes);
      ucSymboles.parent = ucPlateau;
    }

    private void mnuSaisirNouveau_Click(object sender, EventArgs e)
    {
      ucPlateau.InitSaisieNouveauJeu(52, Properties.Resources.Jeu52cartes);
      ucSymboles.Init(52, Properties.Resources.Symboles, ucPlateau.Situation);
      mainLayout.RowStyles[1].Height = symboleHeight;
    }
  }
}
