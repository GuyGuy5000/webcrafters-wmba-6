﻿using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using wmbaApp.CustomControllers;
using wmbaApp.Data;
using wmbaApp.Models;
using wmbaApp.Utilities;
using wmbaApp.ViewModels;

namespace wmbaApp.Controllers
{
    public class ScoreKeepingController : ElephantController
    {
        private readonly WmbaContext _context;

        public ScoreKeepingController(WmbaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(GameScoreKeepingVM scoreKeeping)
        {
            //get game using game ID
            var game = await _context.Games
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.Innings).ThenInclude(i => i.PlayByPlays).ThenInclude(pbp => pbp.Player)
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.ID == scoreKeeping.GameID);

            //update scorekeeping to match what is in the db
            scoreKeeping.LineUp = game.HomeLineup.PlayerLineups.Select(pl => new PlayerScoreKeepingVM(pl.Player.FullName, pl.ID)).ToList();
            scoreKeeping.HomeTeamScore = game.HomeTeamScore;
            scoreKeeping.AwayTeamScore = game.AwayTeamScore;
            scoreKeeping.CurrentInning = game.CurrentInning;


            //add innings that exist in the db
            for (int i = 0; i < game.CurrentInning; i++)
            {
                scoreKeeping.Innings.Add(new InningScoreKeepingVM(scoreKeeping.LineUp));
                scoreKeeping.Innings[i].HomeTeamScore = game.Innings.ToArray()[i].HomeTeamScore;
                scoreKeeping.Innings[i].AwayTeamScore = game.Innings.ToArray()[i].AwayTeamScore;
            }
            //add any remaining innings to make 9 innings total
            if (scoreKeeping.Innings.Count < 9)
            {
                for (int i = scoreKeeping.Innings.Count; i < 9; i++)
                    scoreKeeping.Innings.Add(new InningScoreKeepingVM(scoreKeeping.LineUp));
            }


            //initialize TempData variables
            TempData["HandleFirstBase"] = false;
            TempData["HandleSecondBase"] = false;
            TempData["HandleThirdBase"] = false;
            TempData["StrikeLimit"] = 3;
            //TempData["BatterUpNextIndex"] = 0;

            //check to see if it's a 9U game
            if (game.HomeTeam.Division.DivName == "9U")
                TempData["StrikeLimit"] = 5;

            PopulateDropDownLists(scoreKeeping.Innings[scoreKeeping.CurrentInning]);

            return View(scoreKeeping);
        }

        //Returs a fresh baseball diamond with a new inning and updates the database
        public async Task<IActionResult> StartNewInning(string gameScoreKeepingJSON)
        {
            GameScoreKeepingVM gameVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameScoreKeepingJSON);
            InningScoreKeepingVM previousInningVM = gameVM.Innings[gameVM.CurrentInning - 1]; //gets the previous inning


            var game = await _context.Games
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.Innings).ThenInclude(i => i.PlayByPlays).ThenInclude(pbp => pbp.Player)
               .FirstOrDefaultAsync(g => g.ID == gameVM.GameID);


            //save the previous inning's data
            try
            {
                game.CurrentInning = gameVM.CurrentInning;
                game.HomeTeamScore = gameVM.HomeTeamScore;
                game.AwayTeamScore = gameVM.AwayTeamScore;
                game.CurrentInning = gameVM.CurrentInning;

                var previousInning = new Inning() { gameID = game.ID };

                previousInning.HomeTeamScore = previousInningVM.HomeTeamScore;
                previousInning.AwayTeamScore = previousInningVM.AwayTeamScore;
                //previousInning.PlayByPlays = previousInningVM.PlayByPlays;

                game.Innings.Add(previousInning);

                _context.Games.Update(game);
                _context.SaveChanges();
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save inning. Try again, and if the problem persists see your system administrator.");
            }

            PopulateDropDownLists(gameVM.Innings[gameVM.CurrentInning]);
            TempData["HandleFirstBase"] = false;
            TempData["HandleSecondBase"] = false;
            TempData["HandleThirdBase"] = false;

            return PartialView("_BaseballDiamond", gameVM.Innings[gameVM.CurrentInning]);
        }

        //handles a runner advancing to the next base
        public async Task<IActionResult> HandlePlayerOnBase(string inningScoreKeepingJSON, string senderID, string senderAction)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepingJSON);

            if (senderID.Contains("thirdBase")) //base that triggered the event
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnThird.ID); //get player on third base

                if (senderAction == "home")//check what action occured
                {
                    player.Runs++;
                    player.ThirdBase = false;
                }
                else if (senderAction == "stay")
                {

                }
                else
                {
                    player.Outs++;
                    player.ThirdBase = false;
                }

                TempData["HandleThirdBase"] = false;

            }
            else if (senderID.Contains("secondBase"))
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnSecond.ID);
                if (senderAction == "3rd")
                {
                    player.SecondBase = false;
                    player.ThirdBase = true;
                }
                else if (senderAction == "home")
                {
                    player.Runs++;
                    player.SecondBase = false;
                }
                else if (senderAction == "stay")
                {

                }
                else
                {
                    player.Outs++;
                    player.SecondBase = false;
                }

                TempData["HandleSecondBase"] = false;
            }
            else if (senderID.Contains("firstBase"))
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnFirst.ID);
                if (senderAction == "2nd")
                {
                    player.FirstBase = false;
                    player.SecondBase = true;
                }
                else if (senderAction == "3rd")
                {
                    player.FirstBase = false;
                    player.ThirdBase = true;
                }
                else if (senderAction == "home")
                {
                    player.Runs++;
                    player.FirstBase = false;
                }
                else if (senderAction == "stay")
                {
                }
                else
                {
                    player.Outs++;
                    player.FirstBase = false;
                }

                TempData["HandleFirstBase"] = false;
            }

            bool handleThirdBase = (bool)TempData.Peek("HandleThirdBase");
            bool handleSecondBase = (bool)TempData.Peek("HandleSecondBase");
            bool handleFirstBase = (bool)TempData.Peek("HandleFirstBase");

            if (!handleFirstBase && !handleSecondBase && !handleThirdBase)
            {
                inning.CurrentBatter++;

                while (inning.Batter.FirstBase || inning.Batter.SecondBase || inning.Batter.ThirdBase)
                    inning.CurrentBatter++;
            }

            if (inning.CurrentBatter > inning.Players.Count)
            {
                inning.CurrentBatter = 0;

                while (inning.Batter.FirstBase || inning.Batter.SecondBase || inning.Batter.ThirdBase)
                    inning.CurrentBatter++;
            }

            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        //handles the action made by the batter
        public async Task<IActionResult> HandleBatterAction(string inningScoreKeepingJSON, int actionID)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepingJSON);
            PlayerScoreKeepingVM currentBatter = inning.Players[inning.CurrentBatter];
            PlayerScoreKeepingVM playerOnFirst = inning.PlayerOnFirst;
            PlayerScoreKeepingVM playerOnSecond = inning.PlayerOnSecond;
            PlayerScoreKeepingVM playerOnThird = inning.PlayerOnThird;


            //if any players were already on base flag them as needing to be handled
            if (playerOnFirst != null)
                TempData["HandleFirstBase"] = false;
            if (playerOnSecond != null)
                TempData["HandleSecondBase"] = false;
            if (playerOnThird != null)
                TempData["HandleThirdBase"] = false;

            AtBatOutcome action = (AtBatOutcome)actionID;

            //based on the action, update the playerVM's stats and base position accordingly
            switch (action)
            {
                case AtBatOutcome.Single:
                    RecordSingle(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;

                case AtBatOutcome.Double:
                    RecordDouble(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;

                case AtBatOutcome.Triple:
                    RecordTriple(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;

                case AtBatOutcome.HomeRun:
                    RecordHomeRun(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;

                case AtBatOutcome.Ball:
                    RecordBall(inning, currentBatter);
                    break;

                case AtBatOutcome.Strike:
                    RecordStrike(inning, currentBatter);
                    break;

                case AtBatOutcome.FoulBall:
                    RecordFoul(inning, currentBatter);
                    break;

                case AtBatOutcome.FoulTip:
                    RecordFoulTip(inning, currentBatter);
                    break;

                case AtBatOutcome.HitByPitch:
                    RecordHitByPitch(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;

                case AtBatOutcome.IntentionalWalk:
                    RecordIntentionalWalk(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;
                case AtBatOutcome.CatcherInterference:
                    RecordCatcherInterference(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
                    break;
            }

            //three strikes
            if (currentBatter.Strikes >= (int)TempData.Peek("StrikeLimit"))
            {
                RecordOut(inning, currentBatter);
            }
            //four balls
            if (currentBatter.Balls == 4)
            {
                RecordWalk(inning, currentBatter, playerOnFirst, playerOnSecond, playerOnThird);
            }

            if ((bool)TempData.Peek("HandleFirstBase") == false && (bool)TempData.Peek("HandleSecondBase") == false && (bool)TempData.Peek("HandleThirdBase") == false) //if all runners were handled
                if (actionID <= 3 || actionID == 11) //Any action that is an at bat or causes the batter to advance (based on the enum)
                {
                    inning.CurrentBatter++;

                    while (inning.Batter.FirstBase || inning.Batter.SecondBase || inning.Batter.ThirdBase)
                        inning.CurrentBatter++;
                }

            if (inning.CurrentBatter > inning.Players.Count)
            {
                inning.CurrentBatter = 0;

                while (inning.Batter.FirstBase || inning.Batter.SecondBase || inning.Batter.ThirdBase)
                    inning.CurrentBatter++;
            }


            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult HandleSteal(string inningScoreKeepingJSON, string senderID)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepingJSON);

            if (senderID == "stealSecond")
            {
                inning.PlayerOnFirst.SecondBase = true;
                inning.PlayerOnFirst.FirstBase = false;
            }
            else if (senderID == "stealThird")
            {
                inning.PlayerOnSecond.ThirdBase = true;
                inning.PlayerOnSecond.SecondBase = false;
            }
            else if (senderID == "stealHome")
            {
                inning.PlayerOnThird.Runs++;
                inning.PlayerOnThird.ThirdBase = false;
            }

            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult SkipBatter(string inningScoreKeepingJSON)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepingJSON);

            inning.CurrentBatter++;

            while (inning.Batter.FirstBase || inning.Batter.SecondBase || inning.Batter.ThirdBase)
                inning.CurrentBatter++;

            if (inning.CurrentBatter > inning.Players.Count())
                inning.CurrentBatter = 0;

            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult IncrementAwayTeamScore(string inningScoreKeepingJSON)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepingJSON);

            inning.AwayTeamScore++;

            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult UpdateScorePartial(string gameJSON)
        {
            GameScoreKeepingVM game = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);

            game.HomeTeamScore = game.Innings.Sum(i => i.HomeTeamScore);
            game.AwayTeamScore = game.Innings.Sum(i => i.AwayTeamScore);
            return PartialView("_ScoreBar", game);
        }

        public IActionResult UpdateInningsTablePartial(string gameJSON)
        {
            GameScoreKeepingVM game = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);
            return PartialView("_InningsTable", game);
        }

        public string UpdateGameScoreKeeping(string gameJSON, string currentInningJSON)
        {
            GameScoreKeepingVM gameVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(currentInningJSON);

            inning.HomeTeamScore = inning.TotalRunsThisInning;
            if (gameVM.CurrentInning < gameVM.Innings.Count)
            {
                gameVM.HomeTeamScore = gameVM.Innings.Sum(i => i.HomeTeamScore);
                gameVM.AwayTeamScore = gameVM.Innings.Sum(i => i.AwayTeamScore);
                gameVM.Innings[gameVM.CurrentInning] = inning;
            }

            if (inning.TotalOutsThisInning >= 3)
            {
                gameVM.CurrentInning++;
                if (gameVM.CurrentInning >= gameVM.Innings.Count)
                    gameVM.Innings.Add(new InningScoreKeepingVM(gameVM.LineUp));
            }

            return JsonConvert.SerializeObject(gameVM, settings: new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

        public async Task<IActionResult> CancelGame(int gameID)
        {
            var game = await _context.Games
                .Include(p => p.HomeTeam).ThenInclude(p => p.Players)
                .Include(p => p.AwayTeam).ThenInclude(p => p.Players)
                .Include(p => p.HomeTeam).ThenInclude(p => p.Division)
                .Include(g => g.HomeLineup).ThenInclude(hl => hl.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.AwayLineup).ThenInclude(al => al.PlayerLineups).ThenInclude(pl => pl.Player)
                .Include(g => g.Innings).ThenInclude(i => i.PlayByPlays).ThenInclude(pbp => pbp.Player)
               .FirstOrDefaultAsync(g => g.ID == gameID);

            try
            {
                foreach (Inning i in game.Innings)
                {
                    _context.Innings.Remove(i);
                }

                game.Innings.Clear();
                game.CurrentInning = 0;
                game.HomeTeamScore = 0;
                game.AwayTeamScore = 0;

                _context.Games.Update(game);
                _context.SaveChanges();
            }
            catch (DbUpdateException dex)
            {
                ModelState.AddModelError("", "Unable to save inning. Try again, and if the problem persists see your system administrator.");
            }

            return RedirectToAction("Index", "Games");
        }

        //public PlayerAction ConvertAtBatToPlayerAction(AtBatOutcome action)
        //{
        //    return _context.PlayerActions.FirstOrDefault(a => a.Name == action.Humanize);
        //}

        #region SelectLists
        private SelectList PlayerActionSelectList()
        {
            var actionList = from AtBatOutcome outcome in Enum.GetValues(typeof(AtBatOutcome))
                             select new { ID = (int)outcome, Name = outcome.Humanize() };
            return new SelectList(actionList, "ID", "Name", 0);
        }

        //private SelectList BatterUpNextSelectList(InningScoreKeepingVM inning)
        //{
        //    var batterList = inning.Players.Select(p => new { p.ID, p.Name }).ToList();

        //    for (int i = inning.CurrentBatter; i >= 0; i--)
        //    {
        //        if (i >= inning.Players.Count)
        //        {
        //            batterList.RemoveAt(0);
        //            break;
        //        }
        //        batterList.RemoveAt(i);
        //    }

        //    if (batterList.Count == 0)
        //        batterList = inning.Players.Select(p => new { p.ID, p.Name }).ToList();

        //    return new SelectList(batterList, "ID", "Name", TempData.Peek("BatterUpNextIndex"));
        //}

        private void PopulateDropDownLists(InningScoreKeepingVM inning)
        {
            ViewData["BatterActionList"] = PlayerActionSelectList();
            //ViewData["BatterUpNext"] = BatterUpNextSelectList(inning);
        }
        #endregion


        #region ActionMethods
        private async void RecordSingle(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "single"));

            currentBatter.FirstBase = true;
            currentBatter.Singles++;
            currentBatter.Hits++;
            currentBatter.AtBats++;
            if (playerOnFirst != null)
                TempData["HandleFirstBase"] = true;
            if (playerOnSecond != null)
                TempData["HandleSecondBase"] = true;
            if (playerOnThird != null)
                TempData["HandleThirdBase"] = true;
        }

        private async void RecordDouble(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "double"));

            currentBatter.SecondBase = true;
            currentBatter.Doubles++;
            currentBatter.Hits++;
            currentBatter.AtBats++;
            if (playerOnFirst != null)
                TempData["HandleFirstBase"] = true;
            if (playerOnSecond != null)
                TempData["HandleSecondBase"] = true;
            if (playerOnThird != null)
                TempData["HandleThirdBase"] = true;
        }

        private async void RecordTriple(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "triple"));

            currentBatter.ThirdBase = true;
            currentBatter.Triples++;
            currentBatter.Hits++;
            currentBatter.AtBats++;
            if (playerOnFirst != null)
                TempData["HandleFirstBase"] = true;
            if (playerOnSecond != null)
                TempData["HandleSecondBase"] = true;
            if (playerOnThird != null)
                TempData["HandleThirdBase"] = true;
        }

        private async void RecordHomeRun(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "home run"));

            currentBatter.Runs++;
            currentBatter.HR++;
            currentBatter.Hits++;
            currentBatter.AtBats++;
            //if any runners are on base, add a run and remove them
            if (playerOnFirst != null)
            {
                TempData["HandleFirstBase"] = false;
                playerOnFirst.Runs++;
                playerOnFirst.FirstBase = false;
                currentBatter.RBI++;
            }
            if (playerOnSecond != null)
            {
                TempData["HandleSecondBase"] = false;
                playerOnSecond.Runs++;
                playerOnSecond.SecondBase = false;
                currentBatter.RBI++;

            }
            if (playerOnThird != null)
            {
                TempData["HandleThirdBase"] = false;
                playerOnThird.Runs++;
                playerOnThird.ThirdBase = false;
                currentBatter.RBI++;
            }
        }

        private async void RecordBall(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "ball"));

            currentBatter.Balls++;
        }

        private async void RecordStrike(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "strike"));

            currentBatter.Strikes++;
        }

        private async void RecordFoul(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "foul ball"));

            if (currentBatter.Strikes < 2) //in the case of less that two strikes, the foul must be counted as a special foul
            {
                currentBatter.Strikes++;
                currentBatter.Fouls++;
                currentBatter.FoulStrikes++; //special foul
                currentBatter.Hits++;
            }
            else
            {
                currentBatter.Fouls++;
                currentBatter.Hits++;
            }
        }

        private async void RecordFoulTip(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "foul tip"));

            currentBatter.Strikes++;
        }

        private async void RecordHitByPitch(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "hit by pitch"));

            //advance any runners ahead of the batter
            if (playerOnFirst != null)
            {
                playerOnFirst.SecondBase = true;
                playerOnFirst.FirstBase = false;

                if (playerOnSecond != null)
                {
                    playerOnSecond.ThirdBase = true;
                    playerOnSecond.SecondBase = false;

                    if (playerOnThird != null)
                    {
                        playerOnThird.ThirdBase = false;
                        playerOnThird.Runs++;
                        currentBatter.RBI++;
                    }
                }
            }
            currentBatter.FirstBase = true;
            currentBatter.AtBats++;
            inning.CurrentBatter++;
        }

        private async void RecordIntentionalWalk(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "intentional walk"));

            //advance any runners ahead of the batter
            if (playerOnFirst != null)
            {
                playerOnFirst.SecondBase = true;
                playerOnFirst.FirstBase = false;

                if (playerOnSecond != null)
                {
                    playerOnSecond.ThirdBase = true;
                    playerOnSecond.SecondBase = false;

                    if (playerOnThird != null)
                    {
                        playerOnThird.ThirdBase = false;
                        playerOnThird.Runs++;
                        currentBatter.RBI++;
                    }
                }
            }
            currentBatter.FirstBase = true;
            currentBatter.AtBats++;
            inning.CurrentBatter++;
        }

        private async void RecordCatcherInterference(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "catcher interference"));

            //advance any runners ahead of the batter
            if (playerOnFirst != null)
            {
                playerOnFirst.SecondBase = true;
                playerOnFirst.FirstBase = false;

                if (playerOnSecond != null)
                {
                    playerOnSecond.ThirdBase = true;
                    playerOnSecond.SecondBase = false;

                    if (playerOnThird != null)
                    {
                        playerOnThird.ThirdBase = false;
                        playerOnThird.Runs++;
                        currentBatter.RBI++;
                    }
                }
            }
            currentBatter.FirstBase = true;
            currentBatter.AtBats++;
            inning.CurrentBatter++;
        }

        private async void RecordOut(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter)
        {
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "out"));

            currentBatter.Outs++;
            inning.CurrentBatter++;
        }

        private async void RecordWalk(InningScoreKeepingVM inning, PlayerScoreKeepingVM currentBatter, PlayerScoreKeepingVM playerOnFirst, PlayerScoreKeepingVM playerOnSecond, PlayerScoreKeepingVM playerOnThird)
        {
            //add a play to the playbyplay
            inning.PlayByPlays.Add(GetPlayByPlay(currentBatter.ID, "walk"));

            //advance any runners ahead of the batter
            if (playerOnFirst != null)
            {
                playerOnFirst.SecondBase = true;
                playerOnFirst.FirstBase = false;

                if (playerOnSecond != null)
                {
                    playerOnSecond.ThirdBase = true;
                    playerOnSecond.SecondBase = false;

                    if (playerOnThird != null)
                    {
                        playerOnThird.ThirdBase = false;
                        playerOnThird.Runs++;
                        currentBatter.RBI++;
                    }
                }
            }
            currentBatter.FirstBase = true;
            currentBatter.AtBats++;
            inning.CurrentBatter++;
        }

        #endregion

        #region PlayByPlayMethods
        //returns a PlayByPlay object based on the actionName
        public PlayByPlay GetPlayByPlay(int playerID, string actionName)
        {
            //get action and player from db
            var playerAction = _context.PlayerActions.FirstOrDefault(pa => pa.PlayerActionName.ToLower() == actionName);
            var player = _context.Players.FirstOrDefault(p => p.ID == playerID);

            //return a PlayByPlay with player and playerAction that aren't null
            return new PlayByPlay { PlayerID = playerID, Player = player, PlayerActionID = playerAction.PlayerActionID, PlayerAction = playerAction };
        }
        #endregion
    }
}
