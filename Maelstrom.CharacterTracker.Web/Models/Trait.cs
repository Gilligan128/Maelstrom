using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Maelstrom.CharacterTracker.Web.Models
{
	public class Trait : Entity
	{
 
        [Required]
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        [Range(0,10)]
        public virtual int UsesPerDay { get; set; }
        [Range(0,10)]
        public virtual int HpBonus { get; set; }
        [Range(0,10)]
        public virtual int ApBonus { get; set; }
	}
}