using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Maelstrom.CharacterTracker.Web.Models;
using Xunit;

namespace Maelstrom.CharacterTracker.Web.Tests
{
    class HPCalculatorTests
    {
        [Fact]
        public void test_Racial_HP_at_Level_One()
        {
           // Starting, floating points, Origns, Traits, Tiers

            // arange the data
            var character = new Character();
            // Act
            var hp = character.Origin.BonusHP;
            // Assert
            
        }

        [Fact]
        public void character_Starts_With_10_HP()
        {
            var character = new Character();
            
            var currentHP = character.calculateHP();

            Assert.Equal(10, currentHP);
        }


    }
}
