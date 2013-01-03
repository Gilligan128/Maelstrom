using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Maelstrom.CharacterTracker.Web.Models;

namespace Maelstrom.CharacterTracker.Web.Controllers
{
    public class OriginController : LookupController<Origin>
    {

        static OriginController()
        {
            Add(new Origin {  Name = "Orlo" }, new Origin {  Name = "Gay Telbri" });
        }


    }


}
