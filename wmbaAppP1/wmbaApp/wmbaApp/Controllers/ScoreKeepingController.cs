using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
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

            scoreKeeping.Innings[0].Players[3].FirstBase = true;
            scoreKeeping.Innings[0].Players[2].SecondBase = true;
            scoreKeeping.Innings[0].Players[1].ThirdBase = true;

            TempData["HandleFirstBase"] = true;
            TempData["HandleSecondBase"] = true;
            TempData["HandleThirdBase"] = true;

            return View(scoreKeeping);
        }

        [HttpPost]
        public async Task<IActionResult> StartInning(string gameScoreKeepningJSON)
        {
            GameScoreKeepingVM gameScoreKeepingVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameScoreKeepningJSON);
            InningScoreKeepingVM inning = gameScoreKeepingVM.Innings[gameScoreKeepingVM.CurrentInning];

            // move this code to the hanlde batter action when it is done
            TempData["HandleFirstBase"] = true;
            TempData["HandleSecondBase"] = true;
            TempData["HandleThirdBase"] = true;

            return PartialView("_BaseballDiamond", inning);
        }
        public async Task<IActionResult> HandlePlayerOnBase(string gameScoreKeepningJSON, string senderID, string senderAction)
        {
            GameScoreKeepingVM gameScoreKeepingVM = JsonConvert.DeserializeObject<GameScoreKeepingVM>(gameScoreKeepningJSON);
            InningScoreKeepingVM inning = gameScoreKeepingVM.Innings[gameScoreKeepingVM.CurrentInning];

            //if third base triggered the event
            if (senderID.Contains("thirdBase"))
            {
                PlayerScoreKeepingVM player = inning.Players.FirstOrDefault(p => p.ID == inning.PlayerOnThird.ID); //get player based on third base player ID
                //check what action occured
                if (senderAction == "home")
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

            if (inning.TotalOutsThisInning == 3)
            {
                InningScoreKeepingVM newInning = gameScoreKeepingVM.Innings[gameScoreKeepingVM.CurrentInning + 1]; 
                return PartialView("_BaseballDiamond", newInning);
            }

            return PartialView("_BaseballDiamond", inning);
        }

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


        #region SelectLists
        private SelectList PlayerActionSelectList()
        {
            var actionList = from AtBatOutcome outcome in Enum.GetValues(typeof(AtBatOutcome))
                             select new { ID = (int)outcome, Name = outcome.Humanize() };
            return new SelectList(actionList, "ID", "Name", 0);
        }
        private void PopulateDropDownLists()
        {
            ViewData["PlayerActionList"] = PlayerActionSelectList();
        }
        #endregion
    }
}
