using wmbaApp.Data;
using wmbaApp.Models;

namespace wmbaApp.Utilities
{
    /// <summary>
    /// A class for creating a matchup with both teams, based on a GameTeam object
    /// - Nadav Hilu
    /// </summary>
    public class GameMatchup
    {
        private readonly WmbaContext _context;
        public Game game;
        public Team teamOne;
        public Team teamTwo;
        public string teamOneLineUp;
        public string teamTwoLineUp;
        public int? teamOneScore;
        public int? teamTwoScore;


        public string Versus
        {
            get => $"{teamOne.TmName} VS. {teamTwo.TmName}";
        }

        public string Score
        {
            get => $"{teamOne.TmName}: {teamOneScore} - {teamTwo.TmName}: {teamTwoScore}";
        }

        public GameMatchup(WmbaContext context, GameTeam gameTeam)
        {
            _context = context;
            int gameID = gameTeam.GameID; //gameID
            int teamID = gameTeam.TeamID; //teamID
            GameTeam[] gameTeams = _context.GameTeams.Where(gt => gt.GameID == gameID).ToArray(); //Get both records that match the gameID


            //if both records were found that means there are two teams assigned to one game
            if (gameTeams.Length == 2)
            {
                //get both team's info using gameTeam.teamID
                game = _context.Games.FirstOrDefault(g => g.ID == gameID);
                teamOne = _context.Teams.FirstOrDefault(t => t.ID == gameTeams[0].TeamID);
                teamTwo = _context.Teams.FirstOrDefault(t => t.ID == gameTeams[1].TeamID);
                teamOneLineUp = gameTeams[0].GmtmLineup;
                teamTwoLineUp = gameTeams[1].GmtmLineup;
                teamOneScore = gameTeams[0].GmtmScore;
                teamTwoScore = gameTeams[1].GmtmScore;
            }
            else
                throw new Exception("Could not find a matchup using the given gameID");
        }

        public Team[] GetTeams()
            => new Team[] { teamOne, teamTwo };

        public static List<GameMatchup> GetMatchups(WmbaContext context,Team[] teams)
        {
            List<GameMatchup> gameMatchups = new();
            foreach (Team team in teams) //each team has a list of games
                if (team.GameTeams.Count > 0) //if a team has games:
                {
                    GameMatchup gameMatchup = new(context, team.GameTeams.Last()); //gets the most recent gameTeam record
                    gameMatchups.Add(gameMatchup);
                }

            return gameMatchups;
        }

        public void UpdateMatchUp()
        {
            if (teamOne != null && teamTwo != null && game != null) //if both teams and a game exist
            {
                GameTeam[] gameTeams = _context.GameTeams.Where(gt => gt.GameID == game.ID).ToArray(); //gets both GameTeam records
                GameTeam gameOne = gameTeams[0];
                GameTeam gameTwo = gameTeams[1];

                //updates both records
                gameOne.GmtmLineup = teamOneLineUp;
                gameTwo.GmtmLineup = teamTwoLineUp;
                gameOne.GmtmScore = teamOneScore;
                gameTwo.GmtmScore = teamTwoScore;

                //updates the database and saves 
                _context.GameTeams.Update(gameOne);
                _context.GameTeams.Update(gameTwo);
                _context.SaveChanges();
            }
        }
    }
}
