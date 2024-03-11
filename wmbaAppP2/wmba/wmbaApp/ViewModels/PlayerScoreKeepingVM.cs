//Done By: Emmanuel James, Nadav Hilu

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public enum AtBatOutcome
    {
        Single,
        Double,
        Triple,
        HomeRun,
        Ball,
        Strike,
        FoulBall,
        FoulTip,
        FlyOut,
        GroundOut,
        HitByPitch,
        IntentionalWalk,
        CatcherInterference,
    }

    public class PlayerScoreKeepingVM
    {
        //Fields and Properties
        public int PlateAppearances { get; set; } = 0;
        public int AtBats { get; set; } = 0;
        public int Hits { get; set; } = 0;
        public int Singles { get; set; } = 0;
        public int Doubles { get; set; } = 0;
        public int Triples { get; set; } = 0;
        public int HR { get; set; } = 0;
        public int RBI { get; set; } = 0;
        public int BB { get; set; } = 0;
        public int HitByPitches { get; set; } = 0;
        public int Sacrifices { get; set; } = 0;
        public int CatcherInterference { get; set; } = 0;
        public int Balls { get; set; } = 0;
        public int Strikes { get; set; } = 0;
        public int Fouls { get; set; } = 0;
        public int FoulStrikes { get; set; } = 0; //used to subtract from pitch count when adding strikes, balls, and fouls
        public int Runs { get; set; } = 0;
        public int Outs { get; set; } = 0;
        public bool FirstBase { get; set; }
        public bool SecondBase { get; set; }
        public bool ThirdBase { get; set; }
        public bool BasesLoaded => FirstBase && SecondBase && ThirdBase;
        public string Name { get; set; }
        public int ID { get; set; }
        public List<AtBatOutcome> AtBatActions { get; set; } = new List<AtBatOutcome>();


        public PlayerScoreKeepingVM()
        {

        }
        public PlayerScoreKeepingVM(string name, int ID)
        {
            this.Name = name;
            this.ID = ID;
            this.AtBatActions = new List<AtBatOutcome>();
        }

        // Constructor that instantiates the fields.
        public PlayerScoreKeepingVM(int atBats, int hits, int singles, int doubles, int triples, int homeRuns, int plateAppearances,
            int walks, int hitsByPitches, int sacrifices, int catcherInterference, int ballsCount, int strikesCount,
            int foulsCount, int runsCount, int outsCount)
        {
            this.AtBats = atBats;
            this.Hits = hits;
            this.Singles = singles;
            this.Doubles = doubles;
            this.Triples = triples;
            this.HR = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.BB = walks;
            this.HitByPitches = hitsByPitches;
            this.Sacrifices = sacrifices;
            this.CatcherInterference = catcherInterference;
            this.Runs = runsCount;
            this.Fouls = foulsCount;
            this.Balls = ballsCount;
            this.Outs = outsCount;
            this.Strikes = strikesCount;
        }

        // Does the total AtBats
        public int PAtBats
            => PlateAppearances - BB - HitByPitches - Sacrifices - CatcherInterference;

        //Calculates pitch count
        public int PitchCount
            => Strikes + BB + Fouls - FoulStrikes;

        // Calculates OPS
        public double CalculateOBP()
        {
            double OBP = (double)(Singles + Doubles * 2 + Triples * 3 + HR * 4) / AtBats;
            return Math.Round(OBP, 3);
        }

        // Calculates OBP
        public double CalculateSLG()
        {
            double OBP = (double)(Hits + BB + HitByPitches) / (AtBats + BB + HitByPitches + Sacrifices);
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

            double result = (double)Hits / totalAtBats;
            return Math.Round(result, 3);
        }

        public static List<PlayerScoreKeepingVM> GetPlayers(Player[] players)
        {
            List<PlayerScoreKeepingVM> displayPlayers = new();

            foreach (Player player in players)
            {
                string playerName = $"{player.PlyrFirstName} {player.PlyrLastName}";

                PlayerScoreKeepingVM playerActions = new(playerName, player.ID);
                displayPlayers.Add(playerActions);
            }

            return displayPlayers;
        }
    }
}