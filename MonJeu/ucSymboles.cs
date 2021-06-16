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
  public partial class ucSymboles : Control
  {
    public IucSymboles parent;
    int NbCartes, NbCartesInFamille;
    Bitmap Symboles;
    List<int> Cartes;
    public ucSymboles()
    {
      InitializeComponent();
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
      // base.OnPaintBackground(pevent);
      // do nothing avoid flickering
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
      if (Symboles == null)
      {
        base.OnPaint(pe);
        return;
      }
      RectangleF rcSymbole = new RectangleF(0, 0, Symboles.Width / (NbCartesInFamille + 1), Symboles.Height / 4);
      RectangleF rcSymboleAffiche = new RectangleF(0, 0, ClientSize.Width / (NbCartesInFamille + 1.0f), ClientSize.Height / 4.0f);
      Brush vide = new SolidBrush(Color.FromArgb(0xcc, 0xcc, 0xcc));
      for (int y = 0, idxCarte = 1; y < 4; y++)
      {
        for (int x = 0; x < NbCartesInFamille; x++, idxCarte++)
        {
          if (Cartes.IndexOf(idxCarte) < 0)
          {
            pe.Graphics.DrawImage(Symboles, rcSymboleAffiche, rcSymbole, GraphicsUnit.Pixel);
          }
          else
          {
            pe.Graphics.FillRectangle(vide, rcSymboleAffiche);
          }
          rcSymbole.X += rcSymbole.Width;
          rcSymboleAffiche.X += rcSymboleAffiche.Width;
        }
        rcSymbole.X = 0;
        rcSymboleAffiche.X = 0;
        rcSymbole.Y += rcSymbole.Height;
        rcSymboleAffiche.Y += rcSymboleAffiche.Height;
      }
      // Affichage du bouton de suppression
      rcSymbole.Y = 0;
      rcSymboleAffiche.Y = 0;
      rcSymbole.X = NbCartesInFamille * rcSymbole.Width;
      rcSymboleAffiche.X = NbCartesInFamille * rcSymboleAffiche.Width;
      rcSymbole.Height = Symboles.Height;
      rcSymboleAffiche.Height = ClientSize.Height;
      pe.Graphics.DrawImage(Symboles, rcSymboleAffiche, rcSymbole, GraphicsUnit.Pixel);
    }

    private void ucSymboles_Resize(object sender, EventArgs e)
    {
      Invalidate();
    }

    private void ucSymboles_Click(object sender, EventArgs e)
    {
      Point p = Control.MousePosition;
      p = this.PointToClient(p);
      PointF pf = new PointF(p.X, p.Y);
      SizeF rcSymboleAffiche = new SizeF(ClientSize.Width / (NbCartesInFamille + 1.0f), ClientSize.Height / 4.0f);
      p.X = (int)Math.Floor(pf.X / rcSymboleAffiche.Width);
      p.Y = (int)Math.Floor(pf.Y / rcSymboleAffiche.Height);
      if (p.X == NbCartesInFamille)
      {
        int idxCarteSel = 0;
        EvtucSymboles evtucSymboles = new EvtucSymboles() { idxCarteSel = idxCarteSel, idxCarteRemoved = 0, valide = false };
        parent?.TraiteSaisie(evtucSymboles);
        if (evtucSymboles.valide)
        {
          Cartes.Remove(evtucSymboles.idxCarteRemoved);
          Invalidate();
        }
      }
      else
      {
        int idxCarteSel = 1 + p.X + NbCartesInFamille * p.Y;
        if (Cartes.IndexOf(idxCarteSel) < 0)
        {
          EvtucSymboles evtucSymboles = new EvtucSymboles() { idxCarteSel = idxCarteSel, idxCarteRemoved = 0, valide = false };
          parent?.TraiteSaisie(evtucSymboles);
          if (evtucSymboles.valide)
          {
            Cartes.Remove(evtucSymboles.idxCarteRemoved);
            Cartes.Add(evtucSymboles.idxCarteSel);
            Invalidate();
          }
        }
      }
    }

    internal void Init(int nbCartes, Bitmap symboles, Situation situation)
    {
      if (nbCartes <= 0 || (nbCartes % 4) != 0)
      {
        throw new ArgumentException("nbCartes");
      }
      if (symboles == null)
      {
        throw new ArgumentNullException("symboles");
      }
      NbCartes = nbCartes;
      NbCartesInFamille = nbCartes / 4;
      if ((symboles.Height % 4) != 0 || (symboles.Width % (NbCartesInFamille + 1)) != 0)
      {
        throw new ArgumentException("symboles");
      }
      Symboles = symboles;
      Cartes = new List<int>();
      for (int i = 0; i < 4; i++)
      {
        if (situation.FreeCells[i] > 0)
        {
          Cartes.Add(situation.FreeCells[i]);
        }
      }
      for (int idxCol = 0; idxCol < 8; idxCol++)
      {
        List<int> colonne = situation.Colonnes[idxCol];
        for (int idxPosCarte = 0; idxPosCarte < colonne.Count; idxPosCarte++)
        {
          int idxCarte = colonne[idxPosCarte];
          if (idxCarte > 0)
          {
            Cartes.Add(idxCarte);
          }
        }
      }
      Refresh();
    }
  }
}
