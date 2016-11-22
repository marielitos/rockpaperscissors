using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using RockPaperScissors.Models;
using Newtonsoft.Json;

namespace RockPaperScissors.Controllers
{
    /// <summary>
    /// <c>TournamentController</c> control a tournament. Check top players, store the first and second place
    /// and reset the table with a click.
    /// </summary>
    public class TournamentController : Controller
    {
        private List<Players> players = new List<Players>();
        private const string quotationMarks = "\"";

        /// <summary>
        /// <c>Top</c> list the best players accordion their scores.
        /// </summary>
        /// <returns>Top view with data or an error message.</returns>
        [HttpGet]
        public ActionResult Top()
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://rockpaperscissors.somee.com/");
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            HttpResponseMessage response = client.GetAsync("api/championship/result").Result;
            if (response.IsSuccessStatusCode)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                ViewBag.List = JsonConvert.DeserializeObject<IEnumerable<Champion>>(json);
            } else
            {
                ViewBag.Message = "Sorry! We have a problem, try later!";
            }
            return View();
        }

        /// <summary>
        /// <c>LoadFile</c> render a new page for file uploads.
        /// </summary>
        /// <returns>View's name</returns>
        [HttpGet]
        public ActionResult LoadFile()
        {
            return View("LoadFile");
        }

        /// <summary>
        /// <c>LoadFile</c> check the file and select the winners.
        /// </summary>
        /// <param name="file">File uploaded on the form</param>
        /// <returns>A page with the first and second place or an error message</returns>
        [HttpPost]
        public ActionResult LoadFile(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                if (file.ContentType.ToLower() == "text/plain")
                {
                    try
                    {
                        string result = new StreamReader(file.InputStream).ReadToEnd();
                        return AnalizeFile(result);
                    }
                    catch (Exception ex)
                    {
                        ViewBag.Message = "ERROR: " + ex.Message.ToString();
                    }
                }
                else
                {
                    ViewBag.Message = "This game accept only text files (.txt)";
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View("LoadFile");
        }

        /// <summary>
        /// <c>AnalizeFile</c> analize the file and select the winners.
        /// </summary>
        /// <param name="file">File is the text inside the file.</param>
        /// <returns>A page with the first and second place or an error message</returns>
        private ActionResult AnalizeFile(string file)
        {
            file = file.ToUpper().Replace(" ", "");
            
            bool playerfounded = false;
            bool secondPlayer = false;
            string line = "";

            for (int i = 0; i < file.Length - 1; i++)
            {
                string readedChar = file.Substring(i, 1);
 
                if (playerfounded)
                {
                    if (readedChar == "]")
                    {
                        if (!secondPlayer && file.Substring(i + 3, 1) != quotationMarks)
                        {
                            throw new HttpException(400, "Alone player, check the file format!" + file.Substring(i + 3, 1));
                        }

                        if (!CreatePlayer(line))
                        {
                            throw new HttpException(400, "One or more movements are invalid!");
                        }
                        secondPlayer = !secondPlayer;
                        playerfounded = false;
                        line = "";
                    }
                    else
                    {
                        line += readedChar;
                    }
                }
                else if (readedChar == quotationMarks)
                {
                    playerfounded = true;
                }
            }
            players = GetWinners(players);
            SaveWinners(players[0].name, players[1].name);
            ViewBag.Winners = "Winner: " + players[0].name + ", second place: " + players[1].name;
            return View("LoadFile");
        }

        /// <summary>
        /// <c>DeleteChampions</c> clear the database data about top players.
        /// </summary>
        /// <returns>Render the play game page</returns>
        [HttpPost]
        public ActionResult CleanDataBase()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string url = "http://rockpaperscissors.somee.com/api/championship/deletechampions";
            client.DeleteAsync(url);

            return View("LoadFile");
        }

        /// <summary>
        /// <c>CreatePlayer</c> get a line with the player information and create a new <c>Players</c> object,
        /// that will be on the tournament.
        /// </summary>
        /// <paramref name="line">The line contains the name and movement for a player</paramref>
        /// <returns>The return is true if the player did a movement valid.</returns>
        private bool CreatePlayer(string line)
        {
            line = line.Replace(quotationMarks, "");

            string[] playerInfo = line.Split(',');

            Players player = new Players();
            player.name = playerInfo[0];
            player.movement = playerInfo[1];

            players.Add(player);

            return (player.movement == "R" || player.movement == "P" || player.movement == "S");
        }

        /// <summary>
        /// <c>GetWinners</c> is a recursive method that receives a <c>Players</c> list and run the different 
        /// fights between the players.
        /// </summary>
        /// <param name="list">Players list</param>
        /// <returns>A list with the first and second places</returns>
        private List<Players> GetWinners(List<Players> list)
        {
            List<Players> temp = new List<Players>();
            if (list.Count == 2)
            {
                if (list[0].movement == list[1].movement)
                {
                    temp.Add(list[0]);
                    temp.Add(list[1]);
                }
                else if ((list[0].movement == "R") && list[1].movement == "P")
                {
                    temp.Add(list[1]);
                    temp.Add(list[0]);
                }
                else if ((list[0].movement == "R") && list[1].movement == "S")
                {
                    temp.Add(list[0]);
                    temp.Add(list[1]);
                }
                else if ((list[0].movement == "P") && list[1].movement == "R")
                {
                    temp.Add(list[0]);
                    temp.Add(list[1]);
                }
                else if ((list[0].movement == "P") && list[1].movement == "S")
                {
                    temp.Add(list[1]);
                    temp.Add(list[0]);
                }
                else if ((list[0].movement == "S") && list[1].movement == "P")
                {
                    temp.Add(list[0]);
                    temp.Add(list[1]);
                }
                else if ((list[0].movement == "S") && list[1].movement == "R")
                {
                    temp.Add(list[1]);
                    temp.Add(list[0]);
                }
                return temp;
            } else
            {
                for (int i = 0; i < list.Count - 1;)
                {
                    if (list[i].movement == list[i + 1].movement)
                    {
                        temp.Add(list[i]);
                    }
                    else if ((list[i].movement == "R") && list[i + 1].movement == "P")
                    {
                        temp.Add(list[i + 1]);
                    }
                    else if ((list[i].movement == "R") && list[i + 1].movement == "S")
                    {
                        temp.Add(list[i]);
                    }
                    else if ((list[i].movement == "P") && list[i + 1].movement == "R")
                    {
                        temp.Add(list[i]);
                    }
                    else if ((list[i].movement == "P") && list[i + 1].movement == "S")
                    {
                        temp.Add(list[i + 1]);
                    }
                    else if ((list[i].movement == "S") && list[i + 1].movement == "P")
                    {
                        temp.Add(list[i]);
                    }
                    else if ((list[i].movement == "S") && list[i + 1].movement == "R")
                    {
                        temp.Add(list[i + 1]);
                    }
                    i += 2;
                }
                return GetWinners(temp);
            }
        }

        /// <summary>
        /// <c>SaveWinner</c> save the first and second place on database.
        /// </summary>
        /// <param name="player1">Winner</param>
        /// <param name="player2">Second place</param>
        private void SaveWinners(string player1, string player2)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Add(
                new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            string url = "http://rockpaperscissors.somee.com/api/championship/top?champion1=" + player1 + "&champion2=" + player2;
            string postBody = "";
            client.PostAsync(url, 
                new StringContent(postBody, System.Text.Encoding.UTF8, "application/json"));
 
        }

    }
}
