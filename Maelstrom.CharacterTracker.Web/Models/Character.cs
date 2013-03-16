using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Maelstrom.CharacterTracker.Web.Models
{
    public class Character : Entity
    {
        private int StartingHP = 10;
        public virtual Origin Origin { get; set; }

        public virtual int calculateHP()
        {
            return StartingHP;
        }

    }
}