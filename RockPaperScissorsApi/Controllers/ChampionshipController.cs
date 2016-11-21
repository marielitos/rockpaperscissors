﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using RockPaperScissorsApi.Models;

namespace RockPaperScissorsApi.Controllers
{
    ///<summary>
    /// Use <c>ChampionshipController</c> to control the scores about tournament.
    ///</summary>
    /// <permission cref="ChampionshipController">ChampionshipController is a public class.</permission>
    public class ChampionshipController : ApiController
    {
        private RockPaperScissorsEntities db = new RockPaperScissorsEntities();

        ///<summary>
        /// Use <c>GetChampion</c> for list the top players of all championships.
        /// The <param name="number">number property</param> set the quantity of the elements for the return.
        /// <returns>List of champions. Every champion has a name and score.</returns>
        ///</summary>
        [Route("Championship/Result")]
        public List<Champion> GetChampion(int number = 10)
        {
            return db.Champion.OrderByDescending(query => query.Score).Take(number).ToList();
        }


        ///<summary>
        /// Use <c>PostChampion</c> for set the first and second place of a tournament.
        /// <param name="champion1">champion1</param> set the name of the winner of the championship.
        /// <param name="champion2">champion2</param> set the name of the second place of the championship.
        /// <returns>Response accordion the success of the operation</returns>
        ///</summary>
        [Route("Championship/Top")]
        public IHttpActionResult PostChampion(string champion1, string champion2)
        {
            champion1 = champion1.ToUpper();
            champion2 = champion2.ToUpper();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            AddChampion(champion1);
            AddChampion(champion2);

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateException)
            {
                if (ChampionExists(champion1) && ChampionExists(champion2))
                {
                    //Update the champion
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(true);
        }

        ///<summary>
        /// <c>AddChampion</c> determine if this user won a tournament, after create or update the score for this player.
        /// <param name="name">name</param> is the player's name.
        ///</summary>
        private void AddChampion(string name)
        {
            if (ChampionExists(name))
            {
                //Update the champion
                Champion founding = db.Champion.FirstOrDefault(query => query.Name.Equals(name));
                founding.Score = founding.Score + 100;
            }
            else
            {
                //Insert the champion
                Champion champion = new Champion();
                champion.Name = name;
                champion.Score = 100;
                db.Champion.Add(champion);
            }
        }

        ///<summary>
        /// <c>ChampionExists</c> determine if this user won a tournament.
        /// <param name="name">name</param> is the player's name.
        /// <returns>Return true if the user won a tournament.</returns>
        ///</summary>
        private bool ChampionExists(string name)
        {
            return db.Champion.Count(e => e.Name == name) > 0;
        }
    }
}