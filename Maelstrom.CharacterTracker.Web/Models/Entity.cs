using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Maelstrom.CharacterTracker.Web.Models
{
    public class Entity
    {
       [HiddenInput(DisplayValue= false)]
        public virtual int Id { get; protected set; }
    }
}