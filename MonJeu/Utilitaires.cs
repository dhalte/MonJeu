using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonJeu
{
  class Utilitaires
  {
    public static void AjusteBordsCartes()
    {
      // le fichier contient les images de 52 cartes, 13 en largeur, 4 en hauteur
      // chaque image de carte a pour dimensions width=138, height=185
      // on dessine des bordures aux cartes, avec coins arrondis
      // la bordure fait 2 pixels de large, couleurs d3d5d3, d9dad9
      // l'arrondi de taille 9x9 est pris dans les ressources
      // fichierIn, obtenu sous Paint.NET, doit être sauvé au format 32 bits pour éviter les pixels fantômes lors du FromFile()
      string fichierIn = @"C:\Users\dhal2009\repos\Divers\MonJeu\MonJeu\Resources\Jeu52cartes.png";
      string fichierOut = @"C:\Users\dhal2009\repos\Divers\MonJeu\MonJeu\Resources\Jeu52cartesAjuste.png";

      Bitmap imgOut = (Bitmap)Image.FromFile(fichierIn);
      Graphics g = Graphics.FromImage(imgOut);
      Rectangle rcCarte = new Rectangle(0, 0, 138, 185);
      Rectangle rcArrondi = new Rectangle(0, 0, 9, 9);
      Debug.Assert(imgOut.Width == 13 * rcCarte.Width);
      Debug.Assert(imgOut.Height == 4 * rcCarte.Height);
      Image imgArrondiTopLeft = Properties.Resources.Arrondi;
      Debug.Assert(imgArrondiTopLeft.Width == rcArrondi.Width);
      Debug.Assert(imgArrondiTopLeft.Height == rcArrondi.Height);
      Brush brushErase = new SolidBrush(Color.Transparent);
      Pen pen1 = new Pen(Color.FromArgb(0xd3, 0xd5, 0xd3));
      Pen pen2 = new Pen(Color.FromArgb(0xd9, 0xda, 0xd9));
      Image imgArrondiTopRight = (Image)imgArrondiTopLeft.Clone();
      imgArrondiTopRight.RotateFlip(RotateFlipType.Rotate90FlipNone);
      Image imgArrondiBottomRight = (Image)imgArrondiTopLeft.Clone();
      imgArrondiBottomRight.RotateFlip(RotateFlipType.Rotate180FlipNone);
      Image imgArrondiBottomLeft = (Image)imgArrondiTopLeft.Clone();
      imgArrondiBottomLeft.RotateFlip(RotateFlipType.Rotate270FlipNone);
      for (int x = 0; x < 13; x++)
      {
        rcCarte.X = x * rcCarte.Width;
        for (int y = 0; y < 4; y++)
        {
          rcCarte.Y = y * rcCarte.Height;
          g.DrawLine(pen1, rcCarte.Left + rcArrondi.Width, rcCarte.Top, rcCarte.Right - rcArrondi.Width, rcCarte.Top);
          g.DrawLine(pen2, rcCarte.Left + rcArrondi.Width, rcCarte.Top + 1, rcCarte.Right - rcArrondi.Width, rcCarte.Top + 1);
          g.DrawLine(pen1, rcCarte.Left + rcArrondi.Width, rcCarte.Bottom - 1, rcCarte.Right - rcArrondi.Width, rcCarte.Bottom - 1);
          g.DrawLine(pen2, rcCarte.Left + rcArrondi.Width, rcCarte.Bottom - 2, rcCarte.Right - rcArrondi.Width, rcCarte.Bottom - 2);

          g.DrawLine(pen1, rcCarte.Left, rcCarte.Top + rcArrondi.Height, rcCarte.Left, rcCarte.Bottom - rcArrondi.Height);
          g.DrawLine(pen2, rcCarte.Left + 1, rcCarte.Top + rcArrondi.Height, rcCarte.Left + 1, rcCarte.Bottom - rcArrondi.Height);
          g.DrawLine(pen1, rcCarte.Right - 1, rcCarte.Top + rcArrondi.Height, rcCarte.Right - 1, rcCarte.Bottom - rcArrondi.Height);
          g.DrawLine(pen2, rcCarte.Right - 2, rcCarte.Top + rcArrondi.Height, rcCarte.Right - 2, rcCarte.Bottom - rcArrondi.Height);

          Erase(imgOut, rcCarte.Left, rcCarte.Top, rcArrondi.Width, rcArrondi.Height);
          g.DrawImage(imgArrondiTopLeft, rcCarte.Left, rcCarte.Top);
          Erase(imgOut, rcCarte.Right - rcArrondi.Width, rcCarte.Top, rcArrondi.Width, rcArrondi.Height);
          g.DrawImage(imgArrondiTopRight, rcCarte.Right - rcArrondi.Width, rcCarte.Top);
          Erase(imgOut, rcCarte.Right - rcArrondi.Width, rcCarte.Bottom - rcArrondi.Height, rcArrondi.Width, rcArrondi.Height);
          g.DrawImage(imgArrondiBottomRight, rcCarte.Right - rcArrondi.Width, rcCarte.Bottom - rcArrondi.Height);
          Erase(imgOut, rcCarte.Left, rcCarte.Bottom - rcArrondi.Height, rcArrondi.Width, rcArrondi.Height);
          g.DrawImage(imgArrondiBottomLeft, rcCarte.Left, rcCarte.Bottom - rcArrondi.Height);
        }
      }
      // Perd les pixels transparents
      // Clipboard.SetImage(imgOut);
      imgOut.Save(fichierOut);
    }

    private static void Erase(Bitmap imgOut, int left, int top, int width, int height)
    {
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          imgOut.SetPixel(x + left, y + top, Color.Transparent);
        }
      }
    }
  }
}
