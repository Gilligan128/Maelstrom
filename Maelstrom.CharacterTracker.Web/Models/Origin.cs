using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Maelstrom.CharacterTracker.Web.Models
{
    public class Origin : Entity
    {
        [Required]
        public string Name { get; set; }
        public virtual string Description { get; set; }
        [Range(0,100)]
        public int BonusHP { get; set; }
        [Range(0,100)]
        public int BonusAP { get; set; }

    }
}