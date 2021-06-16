using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonJeu
{
  public class Carte
  {
    public Couleur Couleur;
    public Famille Famille;
    // 1=As, ... 13 ou 14=Roi, 0=absence de carte (dans la saisie d'un jeu initial)
    public int Hauteur;
  }
}
