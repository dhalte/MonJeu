using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonJeu
{
  // Liste de cartes, de leur emplacement d'origine, de leur destination, du temps d'animation souhaité
  public class Animation
  {
    public DateTime Debut;
    public int Frames;
    public List<AnimationCarte> Cartes = new List<AnimationCarte>();

    public Animation()
    {
      Debut = DateTime.Now;
      // Lors du traitement de l'animation, Frame est incrémenté.
      // Mais lorsque le traitement s'applique pour la première fois (Frames == 0), la/les cartes sont effacées de Situation.
      // De même, lorsque Now > Debut+Duree, alors la/les cartes sont inscrites dans leur position définitive dans Situation.
      Frames = 0;
    }
  }
}
