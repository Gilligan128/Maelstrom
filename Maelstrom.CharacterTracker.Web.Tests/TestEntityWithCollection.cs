using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maelstrom.CharacterTracker.Web.Models;

namespace Maelstrom.CharacterTracker.Web.Tests
{
    public class TestEntityWithBiCollection : Entity
    {
        public TestEntityWithBiCollection()
        {
            Children = new List<TestChildEntity>();
        }

        public virtual string Name { get; set; }
        public virtual IList<TestChildEntity> Children { get; set; }
    }

    public class TestChildEntity : Entity
    {
        public virtual TestEntityWithBiCollection Parent { get; set; }
        public virtual string Name { get; set; }
    }

}
