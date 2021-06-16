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
  public partial class ucPlateau : Control, IucSymboles
  {
    private readonly int[,] LgMaxSuite = {
      { 1,     2,   3,   4,   5 },
      { 2,     4,   6,   8,  10 },
      { 4,     8,  12,  16,  20 },
      { 8,    16,  24,  32,  40 },
      { 16,   32,  48,  64,  80 },
      { 32,   64,  96, 128, 160 },
      { 64,  128, 192, 256, 320 },
      { 128, 256, 384, 512, 540 },};
    public Situation Situation = new Situation();
    public Situation SituationInitiale;
    private int NbCartes, NbCartesInFamille;
    private Bitmap ImagesCartes;
    private Size szCarteNominale;
    private SizeF szCarte;
    // Dimensions horizontales
    private float margeHorizontale, sepCol, sepCLRGT, margeHorizontaleRGT;
    // Dimensions verticales
    private float margeVerticale, sepCLRGT_COL, heightHeadCarte, margeVerticaleCol;
    // idxYselected = -1 -> Free Cells
    // idxYselected = -2 -> Rangements
    // idxYselected >= 0 -> Colonne
    //
    // idxXselected < 0 -> aucune cellule, aucune carte n'est sélectionnée
    private int idxYselected, idxXselected = -1;
    private bool IsSelected { get => idxXselected >= 0; }
    private void UnSelect() => idxXselected = -1;
    private ModeJeu ModeJeu;
    private Animations Animations = new Animations();
    private bool AnimationEnCours => Animations.Count > 0;
    private readonly double AnimationDuree = 500;
    private /*System.Windows.Forms.*/ Timer AnimationTimer; // synchronisé avec son thread principal, pas de Invoke nécessaire

    public ucPlateau()
    {
      InitializeComponent();
      // Bizarrement, la propriété n'est pas disponible dans la fenêtre des propriétés du UserControl
      this.DoubleBuffered = true;
      AnimationTimer = new Timer();
      AnimationTimer.Interval = 50;
      AnimationTimer.Tick += AnimationTimer_Tick;
    }

    internal void InitNouveauJeu(int nbCartes, Bitmap imagesCartes)
    {
      if (nbCartes <= 0 || (nbCartes % 4) != 0)
      {
        throw new ArgumentException("nbCartes");
      }
      if (imagesCartes == null)
      {
        throw new ArgumentNullException("imagesCartes");
      }
      int nbCartesInFamille = nbCartes / 4;
      if ((imagesCartes.Width % nbCartesInFamille) != 0 || (imagesCartes.Height % 4) != 0)
      {
        throw new ArgumentException("imageCartes bad dimensions");
      }
      NbCartes = nbCartes;
      NbCartesInFamille = nbCartesInFamille;
      ImagesCartes = imagesCartes;
      szCarteNominale = new Size(imagesCartes.Width / nbCartesInFamille, imagesCartes.Height / 4);
      Situation.NouveauJeu(nbCartes);
      SituationInitiale = Situation.Clone();
      CalculeCoordonnees();
      ModeJeu = ModeJeu.Jeu;
      Invalidate();
    }

    internal void InitSaisieNouveauJeu(int nbCartes, Bitmap imagesCartes)
    {
      if (nbCartes <= 0 || (nbCartes % 4) != 0)
      {
        throw new ArgumentException("nbCartes");
      }
      if (imagesCartes == null)
      {
        throw new ArgumentNullException("imagesCartes");
      }
      int nbCartesInFamille = nbCartes / 4;
      if ((imagesCartes.Width % nbCartesInFamille) != 0 || (imagesCartes.Height % 4) != 0)
      {
        throw new ArgumentException("imageCartes bad dimensions");
      }
      NbCartes = nbCartes;
      NbCartesInFamille = nbCartesInFamille;
      ImagesCartes = imagesCartes;
      szCarteNominale = new Size(imagesCartes.Width / nbCartesInFamille, imagesCartes.Height / 4);
      Situation.InitSaisieNouveau();
      CalculeCoordonnees();
      Invalidate();
      // Sélection de la première case de la première colonne
      idxYselected = idxXselected = 0;
      ModeJeu = ModeJeu.Saisie;
    }

    internal void InitJeuApresSaisie()
    {
      Situation.InitJeuApresSaisie();
      SituationInitiale = Situation.Clone();
      ModeJeu = JeuValide() ? ModeJeu.Jeu : ModeJeu.Incorrect;
      CalculeCoordonnees();
      Invalidate();
    }

    // Pour chaque famille, ses cartes en cellules libres ou en colonnes doivent être uniques, de hauteurs contigües, depuis une hauteur h jusqu'au Roi
    // si h == As, le rangement de la famille est vide (ou vidé), sinon on y met la carte de la famille de hauteur h-1
    private bool JeuValide()
    {
      bool[] check = new bool[NbCartes];
      for (int i = 0; i < 4; i++)
      {
        int idxCarte = Situation.FreeCells[i];
        if (idxCarte < 0 || NbCartes < idxCarte)
        {
          Prevenir($"Valeur de carte invalide : {idxCarte}");
          return false;
        }
        if (idxCarte != 0)
        {
          if (check[idxCarte - 1])
          {
            Prevenir($"{HumanRead(idxCarte)} est en doublon");
            return false;
          }
          check[idxCarte - 1] = true;
        }
      }
      foreach (List<int> colonne in Situation.Colonnes)
      {
        foreach (int idxCarte in colonne)
        {
          if (idxCarte < 1 || NbCartes < idxCarte)
          {
            Prevenir($"Valeur de carte invalide : {idxCarte}");
            return false;
          }
          if (check[idxCarte - 1])
          {
            Prevenir($"{HumanRead(idxCarte)} est en doublon");
            return false;
          }
          check[idxCarte - 1] = true;
        }
      }
      for (int idxFamille = 0; idxFamille < 4; idxFamille++)
      {
        int idxCarteMin = 0;
        for (int idxCarte = 1 + NbCartesInFamille * idxFamille; idxCarte < 1 + NbCartesInFamille * (idxFamille + 1); idxCarte++)
        {
          if (idxCarteMin == 0)
          {
            if (check[idxCarte - 1])
            {
              idxCarteMin = idxCarte;
            }
          }
          else
          {
            if (!check[idxCarte - 1])
            {
              Prevenir($"{HumanRead(idxCarte)} est manquant");
              return false;
            }
          }
        }
        int idxCarteInRangement;
        if (idxCarteMin == 0)
        {
          // On n'a trouvé aucune carte de la famille, donc le rangement contient le roi
          idxCarteInRangement = NbCartesInFamille * (idxFamille + 1);
        }
        else if (idxCarteMin == 1 + NbCartesInFamille * idxFamille)
        {
          // L'as est présent, le rangement est vide
          idxCarteInRangement = 0;
        }
        else
        {
          idxCarteInRangement = idxCarteMin - 1;
        }
        Situation.Rangements[idxFamille] = idxCarteInRangement;
      }
      return true;
    }

    private void Prevenir(string msg)
    {
      MessageBox.Show(msg, null, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
    private readonly string[] Familles = { "Coeur", "Trèfle", "Carreau", "Pique" };
    private readonly string[] Hauteurs52 = { "As", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Valet", "Dame", "Roi" };
    private readonly string[] Hauteurs56 = { "As", "2", "3", "4", "5", "6", "7", "8", "9", "10", "Valet", "Cavalier", "Dame", "Roi" };
    private string HumanRead(int idxCarte)
    {
      if (idxCarte < 1 || NbCartes < idxCarte)
      {
        throw new ArgumentException("idxCarte");
      }
      idxCarte--;
      int famille = idxCarte / NbCartesInFamille;
      int hauteur = idxCarte % NbCartesInFamille;
      string sFamille = Familles[famille];
      string sHauteur = NbCartes == 52 ? Hauteurs52[hauteur] : Hauteurs56[hauteur];
      return $"{sHauteur} de {sFamille}";
    }

    internal void InitSaisie(int nbCartes, Bitmap imagesCartes)
    {
      if (nbCartes <= 0 || (nbCartes % 4) != 0)
      {
        throw new ArgumentException("nbCartes");
      }
      if (imagesCartes == null)
      {
        throw new ArgumentNullException("imagesCartes");
      }
      int nbCartesInFamille = nbCartes / 4;
      if ((imagesCartes.Width % nbCartesInFamille) != 0 || (imagesCartes.Height % 4) != 0)
      {
        throw new ArgumentException("imageCartes bad dimensions");
      }
      NbCartes = nbCartes;
      NbCartesInFamille = nbCartesInFamille;
      ImagesCartes = imagesCartes;
      szCarteNominale = new Size(imagesCartes.Width / nbCartesInFamille, imagesCartes.Height / 4);
      Situation.InitSaisie();
      // Sélection de la première case de la première colonne (même si elle n'est pas vide)
      idxYselected = idxXselected = 0;
      ModeJeu = ModeJeu.Saisie;
      CalculeCoordonnees();
      Invalidate();
    }

    public void TraiteSaisie(EvtucSymboles evtucSymboles)
    {
      if (ModeJeu != ModeJeu.Saisie)
      {
        return;
      }
      if (!IsSelected)
      {
        return;
      }
      evtucSymboles.valide = true;
      if (idxYselected == -1)
      {
        evtucSymboles.idxCarteRemoved = Situation.FreeCells[idxXselected];
        Situation.FreeCells[idxXselected] = evtucSymboles.idxCarteSel;
      }
      else
      {
        evtucSymboles.idxCarteRemoved = Situation.Colonnes[idxXselected][idxYselected];
        if (evtucSymboles.idxCarteSel == 0)
        {
          if (idxYselected < Situation.Colonnes[idxXselected].Count - 1)
          {
            Situation.Colonnes[idxXselected].RemoveAt(idxYselected);
          }
        }
        else
        {
          Situation.Colonnes[idxXselected][idxYselected] = evtucSymboles.idxCarteSel;
          if (evtucSymboles.idxCarteSel != 0 && Situation.Colonnes[idxXselected].Count - 1 == idxYselected)
          {
            idxYselected++;
            Situation.Colonnes[idxXselected].Add(0);
          }
        }
      }
      CalculeCoordonnees();
      Invalidate();
    }

    private void ucPlateau_Resize(object sender, EventArgs e)
    {
      CalculeCoordonnees();
      Invalidate();
    }

    private void CalculeCoordonnees()
    {
      Size szClient = ClientSize;
      if (szClient.Width == 0 || szClient.Height == 0)
      {
        return;
      }
      margeHorizontale = szCarteNominale.Width * 0.15f;
      sepCol = szCarteNominale.Width * 0.15f;
      float largeurNominale = 2 * margeHorizontale + 8 * szCarteNominale.Width + 7 * sepCol;
      float zoom = szClient.Width / largeurNominale;

      szCarte = new SizeF(szCarteNominale.Width * zoom, szCarteNominale.Height * zoom);
      margeVerticale = szCarte.Height * 0.15f;
      sepCLRGT_COL = szCarte.Height * 0.15f;
      float heightHeadCarteMin = szCarte.Height * 0.10f;
      int nbMaxCartesInColonne = 0;
      foreach (var colonne in Situation.Colonnes)
      {
        if (colonne.Count > nbMaxCartesInColonne)
        {
          nbMaxCartesInColonne = colonne.Count;
        }
      }
      float hauteurNominale = 2 * margeVerticale + 2 * szCarte.Height + sepCLRGT_COL + (nbMaxCartesInColonne - 1) * heightHeadCarteMin;
      if (hauteurNominale > szClient.Height)
      {
        float zoom1 = szClient.Height / hauteurNominale;
        if (zoom > zoom1)
        {
          zoom = zoom1;
          szCarte = new SizeF(szCarteNominale.Width * zoom, szCarteNominale.Height * zoom);
          margeVerticale = szCarte.Height * 0.20f;
          sepCLRGT_COL = szCarte.Height * 0.20f;
          heightHeadCarteMin = szCarte.Height * 0.10f;
        }
      }
      sepCLRGT = szCarte.Width * 0.10f;
      sepCol = szCarte.Width * 0.15f;
      heightHeadCarte = (szClient.Height - 2 * margeVerticale - 2 * szCarte.Height - sepCLRGT_COL) / (nbMaxCartesInColonne - 1);
      if (heightHeadCarte < heightHeadCarteMin)
      {
        heightHeadCarte = heightHeadCarteMin;
      }
      float heightHeadCarteMax = szCarte.Height * 0.30f;
      if (heightHeadCarteMax < heightHeadCarte)
      {
        heightHeadCarte = heightHeadCarteMax;
      }
      margeHorizontale = (szClient.Width - 8 * szCarte.Width - 7 * sepCol) / 2.0f;
      margeHorizontaleRGT = szClient.Width - margeHorizontale - 4 * szCarte.Width - 3 * sepCLRGT;
      margeVerticaleCol = margeVerticale + szCarte.Height + sepCLRGT_COL;
    }

    private void ucPlateau_KeyUp(object sender, KeyEventArgs e)
    {
      Debug.Print($"ucPlateau_KeyUp {e.KeyCode}, {e.KeyData}, {e.KeyValue}");
      if (ModeJeu == ModeJeu.Jeu && e.KeyCode == Keys.Back)
      {
        Animations.Clear();
        Situation = SituationInitiale.Clone();
        CalculeCoordonnees();
        Invalidate();
      }
    }

    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
      // base.OnPaintBackground(pevent);
      // Do nothing avoid flickering
    }

    protected override void OnPaint(PaintEventArgs pe)
    {
      Stopwatch sw = Stopwatch.StartNew();
      AnimationTimer.Stop();
      if (Animations.Count > 0 && Animations[0].Frames == 0)
      {
        // Effacer de la situation les cartes qui vont être jouées en animation
        foreach (AnimationCarte carte in Animations[0].Cartes)
        {
          if (carte.Ysrc == -1)
          {
            Situation.FreeCells[carte.Xsrc] = 0;
          }
          else
          {
            Situation.Colonnes[carte.Xsrc].Remove(carte.Carte);
          }
        }
      }
      Graphics g = pe.Graphics;
      Brush bg = new SolidBrush(Color.FromArgb(0, 127, 0));
      Brush brushVide = new SolidBrush(Color.FromArgb(0, 192, 0));
      g.FillRectangle(bg, ClientRectangle);
      RectangleF rc = new RectangleF();
      RectangleF rcSel = new RectangleF();
      for (int i = 0; i < 4; i++)
      {
        rc.Y = margeVerticale;
        rc.Width = szCarte.Width;
        rc.Height = szCarte.Height;
        float marge = margeHorizontale + i * (szCarte.Width + sepCLRGT);
        rc.X = marge;
        if (Situation.FreeCells[i] == 0)
        {
          g.FillRectangle(brushVide, rc);
        }
        else
        {
          PaintCarte(g, Situation.FreeCells[i], rc);
        }
        if (idxYselected == -1 && idxXselected == i)
        {
          rcSel = rc;
        }
        marge = margeHorizontaleRGT + i * (szCarte.Width + sepCLRGT);
        rc.X = marge;
        if (Situation.Rangements[i] == 0)
        {
          g.FillRectangle(brushVide, rc);
        }
        else
        {
          PaintCarte(g, Situation.Rangements[i], rc);
        }
      }
      for (int idxCol = 0; idxCol < 8; idxCol++)
      {
        List<int> colonne = Situation.Colonnes[idxCol];
        float margeCarte = margeVerticaleCol;
        rc.X = margeHorizontale + idxCol * (szCarte.Width + sepCol);
        for (int idxPosCarte = 0; idxPosCarte < colonne.Count; idxPosCarte++)
        {
          rc.Y = margeCarte;
          int idxCarte = colonne[idxPosCarte];
          // Lors d'une saisie, on prévoit des emplacements vides en bas des colonnes
          if (idxCarte == 0)
          {
            g.FillRectangle(brushVide, rc);
          }
          else
          {
            PaintCarte(g, idxCarte, rc);
          }
          if (idxYselected == idxPosCarte && idxXselected == idxCol)
          {
            rcSel = rc;
            if (idxPosCarte < colonne.Count - 1)
            {
              rcSel.Height = heightHeadCarte;
            }
          }
          margeCarte += heightHeadCarte;
        }
      }
      if (!rcSel.IsEmpty)
      {
        Pen penSel = new Pen(Color.FromArgb(127, 255, 0, 0), 5);
        g.DrawLines(penSel, new PointF[] { new PointF(rcSel.Left, rcSel.Top), new PointF(rcSel.Right - 1, rcSel.Top), new PointF(rcSel.Right - 1, rcSel.Bottom - 1), new PointF(rcSel.Left, rcSel.Bottom - 1), new PointF(rcSel.X, rcSel.Y) });
      }
      if (Animations.Count > 0)
      {
        Animation animation = Animations[0];
        double k = (DateTime.Now - animation.Debut).TotalMilliseconds / AnimationDuree;
        bool remove = k >= 1;
        if (k > 1) k = 1;
        foreach (AnimationCarte carte in Animations[0].Cartes)
        {
          RectangleF rcSrc = new RectangleF(); PointF rcDst = new PointF();
          rcSrc.Width = szCarte.Width;
          rcSrc.Height = szCarte.Height;
          if (carte.Ysrc == -1)
          {
            rcSrc.Y = margeVerticale;
            rcSrc.X = margeHorizontale + carte.Xsrc * (szCarte.Width + sepCLRGT);
          }
          else
          {
            rcSrc.Y = margeVerticaleCol + carte.Ysrc * heightHeadCarte;
            rcSrc.X = margeHorizontale + carte.Xsrc * (szCarte.Width + sepCol);
          }
          if (carte.Ydst == -1)
          {
            rcDst.Y = margeVerticale;
            rcDst.X = margeHorizontale + carte.Xdst * (szCarte.Width + sepCLRGT);
          }
          else if (carte.Ydst == -2)
          {
            rcDst.Y = margeVerticale;
            rcDst.X = margeHorizontaleRGT + carte.Xdst * (szCarte.Width + sepCLRGT);
          }
          else
          {
            float margeCarte = margeVerticaleCol + carte.Ysrc * heightHeadCarte;
            rcDst.Y = margeVerticaleCol + carte.Ydst * heightHeadCarte;
            rcDst.X = margeHorizontale + carte.Xdst * (szCarte.Width + sepCol);
          }
          rcSrc.X = (float)((1 - k) * rcSrc.X + k * rcDst.X);
          rcSrc.Y = (float)((1 - k) * rcSrc.Y + k * rcDst.Y);
          PaintCarte(g, carte.Carte, rcSrc);

        }
        if (remove)
        {
          // Ajouter les cartes dans leur destination
          foreach (AnimationCarte carte in Animations[0].Cartes)
          {
            if (carte.Ydst == -1)
            {
              Situation.FreeCells[carte.Xdst] = carte.Carte;
            }
            else if (carte.Ydst == -2)
            {
              Situation.Rangements[carte.Xdst] = carte.Carte;
            }
            else
            {
              Situation.Colonnes[carte.Xdst].Add(carte.Carte);
            }
          }
          Animations.RemoveAt(0);
          CalculeRangementAutomatique();
          if (Animations.Count == 0)
          {
            CalculeCoordonnees();
            Invalidate();
          }
        }
        else
        {
          Animations[0].Frames++;
        }
        if (Animations.Count > 0)
        {
          AnimationTimer.Start();
        }
      }
      sw.Stop();
      // entre 80 ms et 200 ms
      // Debug.Print($"On paint {sw.ElapsedMilliseconds} ms");
    }

    private void PaintCarte(Graphics g, int idxCarte, RectangleF rcDst)
    {
      if (idxCarte < 1 || NbCartes < idxCarte)
      {
        throw new ArgumentException("idxCarte");
      }
      int x = (idxCarte - 1) % NbCartesInFamille;
      int y = (idxCarte - 1) / NbCartesInFamille;
      x *= szCarteNominale.Width;
      y *= szCarteNominale.Height;
      RectangleF rcSrc = new RectangleF(x, y, szCarteNominale.Width, szCarteNominale.Height);
      g.DrawImage(ImagesCartes, rcDst, rcSrc, GraphicsUnit.Pixel);
    }

    private void ucPlateau_Click(object sender, EventArgs e)
    {
      if (ModeJeu == ModeJeu.Incorrect || AnimationEnCours)
      {
        return;
      }
      Point p = Control.MousePosition;
      p = this.PointToClient(p);
      PointF pf = new PointF(p.X, p.Y);
      RectangleF rc = new RectangleF();
      for (int i = 0; i < 4; i++)
      {
        rc.Y = margeVerticale;
        rc.Width = szCarte.Width;
        rc.Height = szCarte.Height;
        float marge = margeHorizontale + i * (szCarte.Width + sepCLRGT);
        rc.X = marge;
        if (rc.Contains(pf))
        {
          if (ModeJeu == ModeJeu.Saisie || !IsSelected)
          {
            idxYselected = -1;
            idxXselected = i;
            Invalidate();
            return;
          }
          TraiteMouvement(i, -1);
          return;
        }
        marge = margeHorizontaleRGT + i * (szCarte.Width + sepCLRGT);
        rc.X = marge;
        if (rc.Contains(pf))
        {
          if (ModeJeu == ModeJeu.Saisie || !IsSelected)
          {
            // On ne sélectionne pas une case de rangement
            return;
          }
          TraiteMouvement(i, -2);
          return;
        }
      }
      for (int idxCol = 0; idxCol < 8; idxCol++)
      {
        List<int> colonne = Situation.Colonnes[idxCol];
        float margeCarte = margeVerticaleCol;
        rc.X = margeHorizontale + idxCol * (szCarte.Width + sepCol);
        rc.Height = heightHeadCarte;
        rc.Y = margeCarte;
        for (int idxPosCarte = 0; idxPosCarte < colonne.Count; idxPosCarte++)
        {
          if (idxPosCarte == colonne.Count - 1)
          {
            rc.Height = szCarte.Height;
          }
          rc.Y = margeCarte;
          int idxCarte = colonne[idxPosCarte];
          // Lors d'une saisie, on prévoit des emplacements vides en bas des colonnes
          if (rc.Contains(pf))
          {
            if (ModeJeu == ModeJeu.Saisie || !IsSelected)
            {
              idxXselected = idxCol;
              idxYselected = idxPosCarte;
              Invalidate();
              return;
            }
            TraiteMouvement(idxCol, idxPosCarte);
            return;
          }
          margeCarte += heightHeadCarte;
        }
        if (ModeJeu == ModeJeu.Jeu && IsSelected)
        {
          rc.Height += szCarte.Height;
          if (rc.Contains(pf))
          {
            TraiteMouvement(idxCol, colonne.Count);
            return;
          }
        }
      }
      idxXselected = -1;
      Invalidate();
    }

    private void TraiteMouvement(int xDst, int yDst)
    {
      int idxCarteSource;
      int idxCarteDst;
      int hauteurSrc, hauteurDst;
      int nbCellulesLibresVides, nbColonnesVides;
      List<int> colonneSrc, colonneDst;
      Animation animation;
      AnimationCarte animationCarte;
      int lgSuite;
      #region TraiteMouvement Vérifications
      // Vérifications préalables
      if (idxXselected < 0 || 8 <= idxXselected)
      {
        throw new ArgumentException("idxXselected");
      }
      if (idxYselected < -1)
      {
        throw new ArgumentException("idxYselected");
      }
      if (xDst < 0 || 8 <= xDst)
      {
        throw new ArgumentException("idxXdest");
      }
      if (yDst < -2)
      {
        throw new ArgumentException("idxYdest");
      }
      if (idxYselected == -1 && 4 <= idxXselected)
      {
        throw new ArgumentException("idxXselected");
      }
      if ((yDst == -1 || yDst == -2) && 4 <= xDst)
      {
        throw new ArgumentException("idxXdest");
      }
      if (idxYselected == -1)
      {
        colonneSrc = null;
        idxCarteSource = Situation.FreeCells[idxXselected];
      }
      else
      {
        colonneSrc = Situation.Colonnes[idxXselected];
        if (colonneSrc.Count <= idxYselected)
        {
          throw new ArgumentException("idxYselected");
        }
        idxCarteSource = colonneSrc[idxYselected];
      }
      if (idxCarteSource < 0 || NbCartes < idxCarteSource)
      {
        throw new ArgumentException("idxCarteSource");
      }
      #endregion TraiteMouvement Vérifications

      #region Pas de carte source sélectionnée
      if (idxCarteSource == 0)
      {
        if (yDst == -2)
        {
          // dest est un rangement, mais aucune carte n'est sélectionnée
          idxXselected = -1;
        }
        else
        {
          // changement de carte sélectionnée
          idxYselected = yDst;
          idxXselected = xDst;
        }
        Invalidate();
        return;
      }
      #endregion Pas de carte source sélectionnée

      #region La destination est une cellule libre
      if (yDst == -1)
      {
        // dest est cellule libre
        if (Situation.FreeCells[xDst] != 0)
        {
          // Free Cell non vide, changement de carte sélectionnée
          idxYselected = yDst;
          idxXselected = xDst;
          Invalidate();
          return;
        }
        if (idxYselected == -1)
        {
          // De free cell vers free cell, pas très utile
          animation = new Animation();
          animationCarte = new AnimationCarte(idxCarteSource, idxXselected, idxYselected, xDst, yDst);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          idxXselected = -1;
          Invalidate();
          return;
        }
        if (idxYselected != colonneSrc.Count - 1)
        {
          // la carte sélectionnée n'est pas en bas de sa colonne, changement de carte sélectionnée
          idxYselected = yDst;
          idxXselected = xDst;
          Invalidate();
          return;
        }
        // une carte de bas de colonne vers une cellule libre vide
        animation = new Animation();
        animationCarte = new AnimationCarte(idxCarteSource, idxXselected, idxYselected, xDst, yDst);
        animation.Cartes.Add(animationCarte);
        Animations.Add(animation);
        idxXselected = -1;
        Invalidate();
        return;
      }
      #endregion // Si la destination est une cellule libre

      #region La destination est un rangement
      if (yDst == -2)
      {
        // On force xDst au rangement associé à la famille de la carte
        xDst = (idxCarteSource - 1) / NbCartesInFamille;
        if (idxYselected == -1)
        {
          // la source est une cellule libre
        }
        else
        {
          // la source est dans une colonne
          colonneSrc = Situation.Colonnes[idxXselected];
          if (colonneSrc.Count - 1 != idxYselected)
          {
            // elle n'est pas en bas de colonne
            idxXselected = -1;
            Invalidate();
            return;
          }
        }
        idxCarteDst = Situation.Rangements[xDst];
        hauteurSrc = 1 + (idxCarteSource - 1) % NbCartesInFamille;
        hauteurDst = idxCarteDst == 0 ? 0 : 1 + (idxCarteDst - 1) % NbCartesInFamille;
        if (hauteurSrc - 1 != hauteurDst)
        {
          // la carte sélectionnée ne peut être rangée
          idxXselected = -1;
          Invalidate();
          return;
        }
        animation = new Animation();
        animationCarte = new AnimationCarte(idxCarteSource, idxXselected, idxYselected, xDst, yDst);
        animation.Cartes.Add(animationCarte);
        Animations.Add(animation);
        idxXselected = -1;
        Invalidate();
        return;
      }
      #endregion Si la destination est un rangement

      #region La destination est une colonne
      colonneDst = Situation.Colonnes[xDst];
      if (idxYselected == -1)
      {
        // la source est une carte de cellule libre
        if (colonneDst.Count == 0)
        {
          // La colonne de destination est vide
          animation = new Animation();
          animationCarte = new AnimationCarte(idxCarteSource, idxXselected, idxYselected, xDst, 0);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          idxXselected = -1;
          Invalidate();
          return;
        }
        // On ne tient pas compte de la yDst, la carte doit être ajoutée en bas de colonne
        idxCarteDst = colonneDst[colonneDst.Count - 1];
        if (BprecedeA(idxCarteDst, idxCarteSource))
        {
          animation = new Animation();
          animationCarte = new AnimationCarte(idxCarteSource, idxXselected, idxYselected, xDst, colonneDst.Count);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          idxXselected = -1;
          Invalidate();
          return;
        }
        idxXselected = -1;
        Invalidate();
        return;
      }
      // La source est une carte de colonne
      colonneSrc = Situation.Colonnes[idxXselected];
      lgSuite = LgSuite(idxXselected, idxYselected);
      if (lgSuite < 0)
      {
        // La source n'est pas une carte dans une suite
        idxXselected = -1;
        Invalidate();
        return;
      }
      nbCellulesLibresVides = Situation.FreeCells.Count(c => c == 0);
      nbColonnesVides = Situation.Colonnes.Count(c => c.Count == 0);
      if (colonneSrc.Count == 0)
      {
        nbColonnesVides--;
      }
      if (lgSuite > LgMaxSuite[nbColonnesVides, nbCellulesLibresVides])
      {
        // La suite est trop longue
        idxXselected = -1;
        Invalidate();
        return;
      }
      if (colonneDst.Count == 0)
      {
        animation = new Animation();
        for (int idxPosCarte = idxYselected; idxPosCarte < colonneSrc.Count; idxPosCarte++)
        {
          animationCarte = new AnimationCarte(colonneSrc[idxPosCarte], idxXselected, idxPosCarte, xDst, idxPosCarte - idxYselected);
          animation.Cartes.Add(animationCarte);
        }
        Animations.Add(animation);
        idxXselected = -1;
        Invalidate();
        return;
      }

      idxCarteDst = colonneDst[colonneDst.Count - 1];
      if (!BprecedeA(idxCarteDst, colonneSrc[idxYselected]))
      {
        idxXselected = -1;
        Invalidate();
        return;
      }
      animation = new Animation();
      for (int idxPosCarte = idxYselected; idxPosCarte < colonneSrc.Count; idxPosCarte++)
      {
        animationCarte = new AnimationCarte(colonneSrc[idxPosCarte], idxXselected, idxPosCarte, xDst, colonneDst.Count + idxPosCarte - idxYselected);
        animation.Cartes.Add(animationCarte);
      }
      Animations.Add(animation);
      idxXselected = -1;
      Invalidate();
      #endregion La destination est une colonne

    }

    private int LgSuite(int idxXselected, int idxYselected)
    {
      if (idxXselected < 0 || 8 <= idxXselected)
      {
        throw new ArgumentException("idxXselected");
      }
      int lgCol;
      List<int> colonne;
      if (idxYselected < 0)
      {
        throw new ArgumentException("idxYselected");
      }
      colonne = Situation.Colonnes[idxXselected];
      lgCol = colonne.Count;
      if (lgCol <= idxYselected)
      {
        throw new ArgumentException("idxYselected");
      }
      int lgSuite = 1;
      for (int y = idxYselected + 1; y < lgCol; y++)
      {
        if (!BprecedeA(colonne[y - 1], colonne[y]))
        {
          lgSuite = -1;
          break;
        }
        lgSuite++;
      }
      return lgSuite;
    }

    private bool BprecedeA(int carteA, int carteB)
    {
      if (carteA <= 0 || NbCartes < carteA)
      {
        throw new ArgumentException("carteA");
      }
      if (carteB <= 0 || NbCartes < carteB)
      {
        throw new ArgumentException("carteB");
      }
      int familleA = (carteA - 1) / NbCartesInFamille;
      int familleB = (carteB - 1) / NbCartesInFamille;
      int couleurA = familleA % 2;
      int couleurB = familleB % 2;
      if (couleurA == couleurB)
      {
        return false;
      }
      int hauteurA = (carteA - 1) % NbCartesInFamille;
      int hauteurB = (carteB - 1) % NbCartesInFamille;
      return hauteurA == hauteurB + 1;
    }

    private void AnimationTimer_Tick(object sender, EventArgs e)
    {
      Invalidate();
    }

    // Recherche si on peut ranger une carte automatiquement
    private void CalculeRangementAutomatique()
    {
      int idxCarte;
      // Critères de recherche de niveau 1
      for (int i = 0; i < 4; i++)
      {
        // Pour chaque carte en cellule libre
        idxCarte = Situation.FreeCells[i];
        if (idxCarte > 0 && RangementAutomatiqueNiveau1(idxCarte))
        {
          int famille = (idxCarte - 1) / NbCartesInFamille;
          Animation animation = new Animation();
          AnimationCarte animationCarte = new AnimationCarte(idxCarte, i, -1, famille, -2);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          Invalidate();
          return;
        }
      }
      for (int i = 0; i < 8; i++)
      {
        // Pour chaque carte en bas de colonne non vide
        List<int> colonne = Situation.Colonnes[i];
        int idxPosLastCarte = colonne.Count - 1;
        if (idxPosLastCarte < 0)
        {
          continue;
        }
        idxCarte = colonne[idxPosLastCarte];
        if (RangementAutomatiqueNiveau1(idxCarte))
        {
          int famille = (idxCarte - 1) / NbCartesInFamille;
          Animation animation = new Animation();
          AnimationCarte animationCarte = new AnimationCarte(idxCarte, i, idxPosLastCarte, famille, -2);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          Invalidate();
          return;
        }
      }
      // Critères de recherche de niveau 2
      for (int i = 0; i < 4; i++)
      {
        // Pour chaque carte en cellule libre
        idxCarte = Situation.FreeCells[i];
        if (idxCarte > 0 && RangementAutomatiqueNiveau2(idxCarte))
        {
          int famille = (idxCarte - 1) / NbCartesInFamille;
          Animation animation = new Animation();
          AnimationCarte animationCarte = new AnimationCarte(idxCarte, i, -1, famille, -2);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          Invalidate();
          return;
        }
      }
      for (int i = 0; i < 8; i++)
      {
        // Pour chaque carte en bas de colonne non vide
        List<int> colonne = Situation.Colonnes[i];
        int idxPosLastCarte = colonne.Count - 1;
        if (idxPosLastCarte < 0)
        {
          continue;
        }
        idxCarte = colonne[idxPosLastCarte];
        if (RangementAutomatiqueNiveau2(idxCarte))
        {
          int famille = (idxCarte - 1) / NbCartesInFamille;
          Animation animation = new Animation();
          AnimationCarte animationCarte = new AnimationCarte(idxCarte, i, idxPosLastCarte, famille, -2);
          animation.Cartes.Add(animationCarte);
          Animations.Add(animation);
          Invalidate();
          return;
        }
      }

    }

    // Cette méthode est appelée pour chaque carte à découvert
    private bool RangementAutomatiqueNiveau1(int idxCarte)
    {
      if (idxCarte <= 0 || NbCartes < idxCarte)
      {
        throw new ArgumentException("idxCarte");
      }
      int hauteur = 1 + (idxCarte - 1) % NbCartesInFamille;
      if (hauteur == 1)
      {
        return true;
      }
      int famille = (idxCarte - 1) / NbCartesInFamille;
      int idxCarteRangee = Situation.Rangements[famille];
      int hauteurCarteRangee = idxCarteRangee == 0 ? 0 : 1 + (idxCarteRangee - 1) % NbCartesInFamille;
      if (hauteurCarteRangee + 1 != hauteur)
      {
        return false;
      }
      if (hauteur == 2)
      {
        // En toute rigueur, les DEUX devraient être traités dans le RangementAutomatiqueNiveau2
        // mais cela compliquerait son algorithme inutilement
        return true;
      }
      int famille1 = (famille + 1) % 4;
      int hauteur1 = 1 + (Situation.Rangements[famille1] - 1) % NbCartesInFamille;
      if (hauteur1 < hauteur - 1)
      {
        return false;
      }
      famille1 = (famille + 3) % 4;
      hauteur1 = 1 + (Situation.Rangements[famille1] - 1) % NbCartesInFamille;
      if (hauteur1 < hauteur - 1)
      {
        return false;
      }
      return true;
    }

    // Cette méthode est appelée pour chaque carte à découvert, après l'appel à RangementAutomatiqueNiveau1
    // Donc la carte ne peut être un As car un As est toujours rangé automatiquement en niveau 1
    // Il faut que la carte soit en cellule libre ou en bas d'une colonne
    private bool RangementAutomatiqueNiveau2(int idxCarte)
    {
      if (idxCarte <= 1 || NbCartes < idxCarte)
      {
        throw new ArgumentException("idxCarte");
      }
      int hauteur = 1 + (idxCarte - 1) % NbCartesInFamille;
      // Les DEUX sont traités dans le RangementAutomatiqueNiveau1
      if (hauteur == 2)
      {
        return false;
      }
      int famille = (idxCarte - 1) / NbCartesInFamille;
      int idxCarteRangee = Situation.Rangements[famille];
      int hauteurCarteRangee = idxCarteRangee == 0 ? 0 : 1 + (idxCarteRangee - 1) % NbCartesInFamille;
      if (hauteurCarteRangee + 1 != hauteur)
      {
        // La carte ne peut tout simplement pas être rangée.
        return false;
      }
      int famille1 = (famille + 1) % 4;
      int idxCarte1Rangee = Situation.Rangements[famille1];
      int hauteur1Rangee = idxCarte1Rangee == 0 ? 0 : 1 + (idxCarte1Rangee - 1) % NbCartesInFamille;
      if (hauteur1Rangee + 2 < hauteur)
      {
        // la carte de famille opposée ne peut pas encore être rangée
        return false;
      }
      famille1 = (famille + 3) % 4;
      idxCarte1Rangee = Situation.Rangements[famille1];
      hauteur1Rangee = idxCarte1Rangee == 0 ? 0 : 1 + (idxCarte1Rangee - 1) % NbCartesInFamille;
      if (hauteur1Rangee + 2 < hauteur)
      {
        // la carte de famille opposée ne peut pas encore être rangée
        return false;
      }
      // Même couleur, autre famille
      famille1 = (famille + 2) % 4;
      idxCarte1Rangee = Situation.Rangements[famille1];
      hauteur1Rangee = idxCarte1Rangee == 0 ? 0 : 1 + (idxCarte1Rangee - 1) % NbCartesInFamille;
      return hauteur1Rangee + 2 >= hauteur;
    }
  }

}

