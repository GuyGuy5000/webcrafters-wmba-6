//Done By: Emmanuel James

using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wmbaApp.Models
{
    public class ScoreKeeping
    {
        // Fields or Properties
        private int AtBats;
        private int[] Hits ;
        private int Doubles;
        private int Triples;
        private int HomeRuns;
        private int PlateAppearances;
        private int Walks;
        private int HitByPitches;
        private int Sacrifices;
        private int CatcherInterference;

        // Construvtor that instantiates the fields.
        public ScoreKeeping(int atBats, int [] hits, int doubles, int triples, int homeRuns, int plateAppearances, 
            int walks, int hitsByPitches, int sacrifices, int catcherInterference)
        {
            this.AtBats = atBats;
            this.Hits = hits;
            this.Doubles = doubles;
            this.Triples = triples;
            this.HomeRuns = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.Walks = walks;
            this.HitByPitches = hitsByPitches;
            this.Sacrifices = sacrifices;
            this.CatcherInterference = catcherInterference;
        }

        // Properties to determine the type of hit. PS: Not so sure about this implementation, It can be modified.
        public int TotalSingles()
        {
            return Hits.Count(h => h == 1);
        }

        public int TotalDoubles()
        {
            return Hits.Count(h => h == 2);
        }

        public int TotalTriples()
        {
            return Hits.Count(h => h == 3);
        }

        // I don't know how we are gonna evaluate a Home-Run. This method evaluates a Hom-Run as four hits by the player.
        public int TotalHomeRuns()
        {
            return Hits.Count(h => h == 4); 
        }




        // This gets the total hits of a player
        public int TotalHits()
        {
            return Hits.Sum();
        }


        // Does the total AtBats
        public int PAtBats
        {
            get
            {
                AtBats = PlateAppearances - Walks - HitByPitches - Sacrifices - CatcherInterference;
                return AtBats;
            }
        }

        // Calculates OPS
        public double CalculateOBP() 
        {
            double OPS = (double)(TotalSingles() + TotalDoubles() * 2 + TotalTriples() * 3 + TotalHomeRuns() * 4) / AtBats;
            return Math.Round(OPS, 3);
        }


        // Calculates OBP
        public double CalculateSLG()
        {
            double OBP = (double)(TotalHits() + Walks + HitByPitches) / (AtBats + Walks + HitByPitches + Sacrifices);
            return Math.Round(OBP, 3);
        }

        // Calculates OPS
        public double CalculateOPS()
        {
            double OPS = (double)(CalculateOBP() + CalculateSLG());
            return Math.Round(OPS, 3);
        }

        // Calculates the B.Avg
        public double CalculateBAvg()
        {
            int totalAtBats = PAtBats;

            if (totalAtBats == 0)
            {
                return 0.0;
            }

            double result = (double)TotalHits() / totalAtBats;
            return Math.Round(result, 3);
        }


    }
}
