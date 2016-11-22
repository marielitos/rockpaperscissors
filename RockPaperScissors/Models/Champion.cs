using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RockPaperScissors.Models
{
    /// <summary>
    /// <c>Champion</c> represent a player that won at least 1 time.
    /// </summary>
    public class Champion
    {
        /// <summary>
        /// Get/Set the value for <c>Name</c> property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/Set the value for <c>Score</c> property
        /// </summary>
        public int Score { get; set; }
    }
}