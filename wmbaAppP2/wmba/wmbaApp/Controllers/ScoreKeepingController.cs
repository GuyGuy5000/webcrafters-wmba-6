using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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



            TempData["HandleFirstBase"] = false;
            TempData["HandleSecondBase"] = false;
            TempData["HandleThirdBase"] = false;
            //TempData["BatterUpNextIndex"] = 0;

            PopulateDropDownLists(scoreKeeping.Innings[scoreKeeping.CurrentInning]);

            return View(scoreKeeping);
        }

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

            return PartialView("_BaseballDiamond", inning);
        }

        public IActionResult HandleSteal(string inningScoreKeepningJSON, string senderID)
        {
            InningScoreKeepingVM inning = JsonConvert.DeserializeObject<InningScoreKeepingVM>(inningScoreKeepningJSON);

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
    }
}
