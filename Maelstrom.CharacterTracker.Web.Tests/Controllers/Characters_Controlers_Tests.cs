using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using Maelstrom.CharacterTracker.Web.Controllers;
using Maelstrom.CharacterTracker.Web.Models;
using NHibernate.Linq;

namespace Maelstrom.CharacterTracker.Web.Tests.Controllers
{
    public class Characters_Controlers_Tests : ControllerTests
    {
        [Fact]
        public void Successfully_Creates_A_New_Character()
        {
            var character = new Character();
            ExecuteAction<CharactersController>(c => c.Create(character));
            Assert.Equal(character, currentSession.Query<Character>().Single());
        }

    }
}
