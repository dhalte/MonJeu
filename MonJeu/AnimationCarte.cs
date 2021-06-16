using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonJeu
{
  public class AnimationCarte
  {
    public int Carte;
    public int Xsrc;
    public int Ysrc;
    public int Xdst;
    public int Ydst;

    public AnimationCarte(int carte, int xsrc, int ysrc, int xdst, int ydst)
    {
      Carte = carte;
      Xsrc = xsrc;
      Ysrc = ysrc;
      Xdst = xdst;
      Ydst = ydst;
    }
  }
}
