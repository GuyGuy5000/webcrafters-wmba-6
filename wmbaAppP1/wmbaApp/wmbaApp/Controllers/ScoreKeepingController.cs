using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
<<<<<<< HEAD
using Newtonsoft.Json.Serialization;
=======
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
using OfficeOpenXml.Drawing.Chart;
using System;
using System.Linq;
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

        public IActionResult Index(GameScoreKeepingVM scoreKeeping)
        {
            scoreKeeping.Innings = new InningScoreKeepingVM[9]
                                {
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                    new InningScoreKeepingVM(""),
                                };

<<<<<<< HEAD
<<<<<<< HEAD


            TempData["HandleFirstBase"] = false;
            TempData["HandleSecondBase"] = false;
            TempData["HandleThirdBase"] = false;
            //TempData["BatterUpNextIndex"] = 0;

            PopulateDropDownLists(scoreKeeping.Innings[scoreKeeping.CurrentInning]);
=======
            scoreKeeping.Innings[0].Players[3].FirstBase = true;
=======
            scoreKeeping.Innings[0].Players[1].FirstBase = true;
>>>>>>> c4ae6ca (Added pitch button and batter actions select list for scorekeeping.)
            scoreKeeping.Innings[0].Players[2].SecondBase = true;
            scoreKeeping.Innings[0].Players[3].ThirdBase = true;

            TempData["HandleFirstBase"] = true;
            TempData["HandleSecondBase"] = true;
            TempData["HandleThirdBase"] = true;
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)

            //TempData["HandleFirstBase"] = false;
            //TempData["HandleSecondBase"] = false;
            //TempData["HandleThirdBase"] = false;

            PopulateDropDownLists();

            return View(scoreKeeping);
        }

<<<<<<< HEAD
        //Returs a fresh baseball diamond with a new inning and pushes updates to the database
        public async Task<IActionResult> StartNewInning(string gameScoreKeepingJSON)
        {
            GameScoreKeepingVM gameScoreKeepingVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameScoreKeepingJSON);
            InningScoreKeepingVM inning = gameScoreKeepingVM.Innings[gameScoreKeepingVM.CurrentInning];

            PopulateDropDownLists(inning);

            return PartialView("_BaseballDiamond", inning);
        }

        //handles a runner advancing to the next base
        public async Task<IActionResult> HandlePlayerOnBase(string inningScoreKeepningJSON, string senderID, string senderAction)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);

            if (senderID.Contains("thirdBase")) //base that triggered the event
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnThird.ID); //get player on third base

                if (senderAction == "home")//check what action occured
                {
                    player.AwardRun();
                    player.ThirdBase = false;
=======
        [HttpPost]
        public async Task<IActionResult> StartInning(string gameScoreKeepningJSON)
        {
            GameScoreKeepingVM gameScoreKeepingVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameScoreKeepningJSON);
            InningScoreKeepingVM inning = gameScoreKeepingVM.Innings[gameScoreKeepingVM.CurrentInning];

            return PartialView("_BaseballDiamond", inning);
        }
        public async Task<IActionResult> HandlePlayerOnBase(string inningScoreKeepningJSON, string senderID, string senderAction)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);


            if (senderID.Contains("thirdBase")) //if third base triggered the event
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnThird.ID); //get player on third base
                //check what action occured
                if (senderAction == "home")
                {
                    player.AwardRun();
                    player.ThirdBase = false;
<<<<<<< HEAD

>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
>>>>>>> c4ae6ca (Added pitch button and batter actions select list for scorekeeping.)
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
<<<<<<< HEAD
<<<<<<< HEAD
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
                    player.AwardRun();
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
                    player.AwardRun();
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
                inning.CurrentBatter++;

            if (inning.CurrentBatter > inning.Players.Count)
                inning.CurrentBatter = 0;



            PopulateDropDownLists(inning);
            return PartialView("_BaseballDiamond", inning);
        }

        //handles the action made by the batter
        public async Task<IActionResult> HandleBatterAction(string inningScoreKeepningJSON, int actionID)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);
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

            //based on the action, update the player's stats and base position accordingly
            switch (action)
            {
                case AtBatOutcome.Single:
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
                    break;
                case AtBatOutcome.Double:
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
                    break;
                case AtBatOutcome.Triple:
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
                    break;
                case AtBatOutcome.HomeRun:
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
                    break;
                case AtBatOutcome.Ball:
                    currentBatter.Balls++;
                    break;
                case AtBatOutcome.Strike:
                    currentBatter.Strikes++;
                    break;
                case AtBatOutcome.FoulBall:
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
                    break;
                case AtBatOutcome.FoulTipOut:
                    if (currentBatter.Strikes < 2)
                    {
                        currentBatter.Strikes++;
                        currentBatter.Fouls++;
                        currentBatter.FoulStrikes++;
                        currentBatter.Hits++;
                    }
                    else
                    {
                        currentBatter.Fouls++;
                        currentBatter.Hits++;
                    }
                    break;
                case AtBatOutcome.HitByPitch:
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
                    break;
                case AtBatOutcome.IntentionalWalk:
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
                    break;
                case AtBatOutcome.CatcherInterference:
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
                    break;
            }

            //three strikes
            if (currentBatter.Strikes >= 3)
            {
                currentBatter.Outs++;
                inning.CurrentBatter++;
            }
            //four balls
            if (currentBatter.Balls == 4)
            {
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

            if ((bool)TempData.Peek("HandleFirstBase") == false && (bool)TempData.Peek("HandleSecondBase") == false && (bool)TempData.Peek("HandleThirdBase") == false) //if all runners were handled
                if (actionID <= 3 || actionID == 11) //Any action that is an at bat or causes the batter to advance (based on the enum)
                    inning.CurrentBatter++;

            if (inning.CurrentBatter > inning.Players.Count())
                inning.CurrentBatter = 0;

            PopulateDropDownLists(inning);
=======

            if (inning.TotalOutsThisInning == 3)
=======
            else if (senderID.Contains("secondBase")) //if second base triggered the event
>>>>>>> c4ae6ca (Added pitch button and batter actions select list for scorekeeping.)
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnSecond.ID); //get player on second base
                if (senderAction == "3rd")
                {
                    player.SecondBase = false;
                    player.ThirdBase = true;
                }
                else if (senderAction == "home")
                {
                    player.AwardRun();
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
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnFirst.ID); //get player on second base
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
                    player.AwardRun();
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
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)

            PopulateDropDownLists();
            return PartialView("_BaseballDiamond", inning);
        }

<<<<<<< HEAD
        public IActionResult HandleSteal(string inningScoreKeepningJSON, string stolenBase)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);



            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult UpdateScorePartial(string gameJSON)
        {
            GameScoreKeepingVM game = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);

            if (game.Innings[game.CurrentInning].TotalOutsThisInning >= 3)
                game.CurrentInning++;
            if (game.CurrentInning >= 10)
                return RedirectToAction("Index", "Games");

            game.HomeTeamScore = Convert.ToInt32(game.Score);
            return PartialView("_ScoreBar", game);
        }

        public IActionResult UpdateInningsTablePartial(string gameJSON)
        {
            GameScoreKeepingVM game = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);
            return PartialView("_InningsTable", game);
        }

        public string UpdateGameScoreKeeping(string gameJSON, string currentInningJSON)
        {
            GameScoreKeepingVM game = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameJSON);
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(currentInningJSON);

            game.Innings[game.CurrentInning] = inning;

            if (inning.TotalOutsThisInning >= 3)
                game.CurrentInning++;

            return JsonConvert.SerializeObject(game, settings: new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }

=======
        public async Task<IActionResult> InputPlayerAction(string inningScoreKeepningJSON, int outcomeID)
        {
            InningScoreKeepingVM inningScoreKeepingVM = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);
            PlayerScoreKeepingVM currentBatter = inningScoreKeepingVM.Players[inningScoreKeepingVM.CurrentBatter];

            AtBatOutcome outcome = (AtBatOutcome)outcomeID;


            inningScoreKeepingVM.CurrentBatter++;
            if (inningScoreKeepingVM.CurrentBatter > inningScoreKeepingVM.Players.Count())
                inningScoreKeepingVM.CurrentBatter = 1;

            return PartialView("_BaseballDiamond", inningScoreKeepingVM);
        }











        [HttpPost]
        public IActionResult UpdateScore(GameScoreKeepingVM scoreKeeping)
        {
            GameTeam gt = _context.GameTeams.FirstOrDefault(gt => gt.GameID == scoreKeeping.GameID);
            //gt.GmtmScore = scoreKeeping.HomeTeamScore;
            //_context.GameTeams.Update(gt);
            //_context.SaveChanges();

            return PartialView("_ScoreBar", scoreKeeping);
        }

        [HttpPost]
        public IActionResult UpdateInningTable(GameScoreKeepingVM scoreKeeping)
        {
            return PartialView("_InningsTable", scoreKeeping);
        }

        [HttpPost]
        public JsonResult UpdateGameScoreKeeping(GameScoreKeepingVM scoreKeeping)
            => Json(scoreKeeping);


        [HttpPost]
        public IActionResult AddPlayerAction(string playerScoreKeepingJSON, int playerActionID)
        {
            PlayerScoreKeepingVM playerScoreKeeping = JsonConvert.DeserializeObject<PlayerScoreKeepingVM>(playerScoreKeepingJSON);

            switch (playerActionID)
            {
                case 0:
                    playerScoreKeeping.AddAction(AtBatOutcome.Ball);
                    break;
                case 1:
                    playerScoreKeeping.AddAction(AtBatOutcome.CalledStrike);
                    break;
                case 2:
                    playerScoreKeeping.AddAction(AtBatOutcome.SwingAndMiss);
                    break;
                case 3:
                    playerScoreKeeping.AddAction(AtBatOutcome.FoulBall);
                    break;
                case 4:
                    playerScoreKeeping.AddAction(AtBatOutcome.FoulTipOut);
                    break;
                case 5:
                    playerScoreKeeping.AddAction(AtBatOutcome.HitByPitch);
                    break;
                case 6:
                    playerScoreKeeping.AddAction(AtBatOutcome.IntentionalBall);
                    break;
                case 7:
                    playerScoreKeeping.AddAction(AtBatOutcome.IntentionalWalk);
                    break;
                case 8:
                    playerScoreKeeping.AddAction(AtBatOutcome.CatcherInterference);
                    break;
                case 9:
                    playerScoreKeeping.AddAction(AtBatOutcome.IllegalPitch);
                    break;
                case 12:
                    playerScoreKeeping.AddAction(AtBatOutcome.Hit);
                    break;
            }

            playerScoreKeeping.AnalyzePlayerActions(playerScoreKeeping.AtBatActions);

            return PartialView("_BaseballDiamond", playerScoreKeeping);
        }


>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
        #region SelectLists
        private SelectList PlayerActionSelectList()
        {
            var actionList = from AtBatOutcome outcome in Enum.GetValues(typeof(AtBatOutcome))
                             select new { ID = (int)outcome, Name = outcome.Humanize() };
            return new SelectList(actionList, "ID", "Name", 0);
        }
<<<<<<< HEAD

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
=======
        private void PopulateDropDownLists()
        {
<<<<<<< HEAD
            ViewData["PlayerActionList"] = PlayerActionSelectList();
>>>>>>> 2403aa7 (Updated scorekeeping view and ViewModels. Scorekeeping view not yet completed)
=======
            ViewData["BatterActionList"] = PlayerActionSelectList();
>>>>>>> c4ae6ca (Added pitch button and batter actions select list for scorekeeping.)
        }
        #endregion
    }
}
