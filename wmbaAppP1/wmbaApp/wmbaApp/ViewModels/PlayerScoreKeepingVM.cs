﻿//Done By: Emmanuel James, Nadav Hilu

using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wmbaApp.Models;

namespace wmbaApp.ViewModels
{
    public enum AtBatOutcome
    {
        Ball,
        CalledStrike,
        SwingAndMiss,
        FoulBall,
        FoulTipOut,
        HitByPitch,
        IntentionalBall,
        IntentionalWalk,
        CatcherInterference,
        IllegalPitch,
        Walk,
        Strikeout,
        Hit,
        Single,
        Double,
        Triple,
        HomeRun
    }

    public class PlayerScoreKeepingVM
    {
        // Fields and Properties
        public int AtBats = 0;
        public int Hits = 0;
        public int Singles = 0;
        public int Doubles = 0;
        public int Triples = 0;
        public int HomeRuns = 0;
        public int RBI = 0;
        public int PlateAppearances = 0;
        public int Walks = 0;
        public int HitByPitches = 0;
        public int Sacrifices = 0;
        public int CatcherInterference = 0;
        public int Balls = 0;
        public int Strikes = 0;
        public int Fouls = 0;
        public int Runs = 0;
        public int Outs = 0;
        public bool FirstBase { get; set; }
        public bool SecondBase { get; set; }
        public bool ThirdBase { get; set; }
        public bool BasesLoaded => FirstBase && SecondBase && ThirdBase;
        public string Name { get; set; }
        public int ID {  get; set; }
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
            this.HomeRuns = homeRuns;
            this.PlateAppearances = plateAppearances;
            this.Walks = walks;
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
            => PlateAppearances - Walks - HitByPitches - Sacrifices - CatcherInterference;

        // Calculates OPS
        public double CalculateOBP()
        {
            double OPS = (double)(Singles + Doubles * 2 + Triples * 3 + HomeRuns * 4) / AtBats;
            return Math.Round(OPS, 3);
        }

        // Calculates OBP
        public double CalculateSLG()
        {
            double OBP = (double)(Hits + Walks + HitByPitches) / (AtBats + Walks + HitByPitches + Sacrifices);
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

        // PLAYER ACTIONS

        private void RecordHit(AtBatOutcome outcome)
        {
            if (outcome == AtBatOutcome.Single)
            {
                FirstBase = true;
                SecondBase = false;
                ThirdBase = false;

                Singles++;
            }
            else if (outcome == AtBatOutcome.Double)
            {
                FirstBase = false;
                SecondBase = true;
                ThirdBase = false;
                Doubles++;
            }
            else if (outcome == AtBatOutcome.Triple)
            {
                FirstBase = false;
                SecondBase = false;
                ThirdBase = true;
                Triples++;
            }
            else
            {
                FirstBase = false;
                SecondBase = false;
                ThirdBase = false;
                Runs++;
                HomeRuns++;
            }
        }

        public void RecordBall() => Balls++;
        public void RecordFoul() => Fouls++;
        public void AwardRun() => Runs++;
        private void RecordOut()
        {
            Outs++;
            ResetCount();
        }
        private void ResetCount()
        {
            Balls = 0;
            Strikes = 0;
        }
        public void RecordStrike()
        {
            Strikes++;
            if (Strikes >= 3)
            {
                RecordOut();
            }
        }
        private void RecordFoulTipOut()
        {
            RecordOut();
        }
        private void RecordHitByPitch()
        {
            HitByPitches++;

            if (IsBasesLoaded())
            {
                AwardRun();
            }
        }
        private bool IsBasesLoaded()
            => BasesLoaded;
        private void RecordIntentionalBall()
            => RecordBall();
        private void RecordWalk()
            => Walks++;
        private void RecordCatcherInterference()
            => CatcherInterference++;
        private void RecordSwingAndMiss()
        {
            Strikes++;
            if (Strikes >= 3)
                RecordOut();
        }
        private void RecordIntentionalWalk()
            => ResetCount();
        private void RecordIllegalPitch()
            => RecordBall();

        public AtBatOutcome AnalyzeAtBat()
        {
            if (Balls >= 4)
            {
                Balls = 0;
                return AtBatOutcome.Walk;
            }
            else if (Strikes >= 3)
                return AtBatOutcome.Strikeout;
            else
            {
                ///////Still need to add Logic for fouls
                return AtBatOutcome.Hit;
            }
        }

        public void AnalyzePlayerActions(List<AtBatOutcome> playerActions)
        {
            foreach (var action in playerActions)
            {
                switch (action)
                {
                    case AtBatOutcome.Single:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Double:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Triple:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.HomeRun:
                        RecordHit(action);
                        break;
                    case AtBatOutcome.Ball:
                        RecordBall();
                        break;
                    case AtBatOutcome.CalledStrike:
                        RecordStrike();
                        break;
                    case AtBatOutcome.FoulBall:
                        RecordFoul();
                        break;
                    case AtBatOutcome.SwingAndMiss:
                        RecordSwingAndMiss();
                        break;
                    case AtBatOutcome.FoulTipOut:
                        RecordFoulTipOut();
                        break;
                    case AtBatOutcome.HitByPitch:
                        RecordHitByPitch();
                        break;
                    case AtBatOutcome.IntentionalBall:
                        RecordIntentionalBall();
                        break;
                    case AtBatOutcome.IntentionalWalk:
                        RecordIntentionalWalk();
                        break;
                    case AtBatOutcome.CatcherInterference:
                        RecordCatcherInterference();
                        break;
                    case AtBatOutcome.IllegalPitch:
                        RecordIllegalPitch();
                        break;
                }
            }
        }

        public void AddAction(AtBatOutcome action)
            => AtBatActions.Add(action);

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