using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonJeu
{
  public class Situation
  {
    public int[] FreeCells = new int[4];
    public int[] Rangements = new int[4];
    public List<int>[] Colonnes = new List<int>[8];
    public Situation()
    {
      Init();
    }
    private void Init()
    {
      for (int i = 0; i < 4; i++)
      {
        Rangements[i] = FreeCells[i] = 0;
      }
      for (int i = 0; i < 8; i++)
      {
        Colonnes[i] = new List<int>();
      }
    }
    public void NouveauJeu(int nbCartes)
    {
      Init();
      List<int> idxCartes = new List<int>();
      for (int i = 1; i <= nbCartes; i++)
      {
        idxCartes.Add(i);
      }
      Random rnd = new Random();
      int seed = rnd.Next();
      // Pour pouvoir au besoin reproduire le jeu dans la phase de mise au point
      Debug.Print($"seed={seed}");
      rnd = new Random(seed);
      for (int i = 0; i < nbCartes; i++)
      {
        int idx = rnd.Next(idxCartes.Count);
        int idxCarte = idxCartes[idx];
        idxCartes.RemoveAt(idx);
        Colonnes[i % 8].Add(idxCarte);
      }
    }
    public void InitSaisieNouveau()
    {
      Init();
      for (int i = 0; i < 8; i++)
      {
        Colonnes[i].Add(0);
      }
    }

    internal void InitSaisie()
    {
      for (int i = 0; i < 8; i++)
      {
        Colonnes[i].Add(0);
      }
    }

    internal void InitJeuApresSaisie()
    {
      foreach (var colonne in Colonnes)
      {
        while (colonne.Count > 0 && colonne[colonne.Count - 1] == 0)
        {
          colonne.RemoveAt(colonne.Count - 1);
        }
      }
    }

    private Situation(Situation situation)
    {
      Array.Copy(FreeCells, situation.FreeCells, 4);
      Array.Copy(Rangements, situation.Rangements, 4);
      for (int idxCol = 0; idxCol < 8; idxCol++)
      {
        Colonnes[idxCol] = new List<int>();
        Colonnes[idxCol].AddRange(situation.Colonnes[idxCol]);
      }
    }
    internal Situation Clone()
    {
      return new Situation(this);
    }
  }
}
