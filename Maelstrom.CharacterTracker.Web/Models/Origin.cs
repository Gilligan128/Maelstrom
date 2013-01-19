using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Maelstrom.CharacterTracker.Web.Models
{
    public class Origin
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }
        [Range(0,100)]
        public int BonusHP { get; set; }
        [Range(0,100)]
        public int BonusAP { get; set; }

    }
}