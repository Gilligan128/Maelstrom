﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Maelstrom.CharacterTracker.Web;
using Maelstrom.CharacterTracker.Web.Controllers;
using Xunit;

namespace Maelstrom.CharacterTracker.Web.Tests.Controllers
{
    
    public class HomeControllerTest
    {
        [Fact]
        public void Index()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Index() as ViewResult;

            // Assert
            Assert.Equal("Modify this template to jump-start your ASP.NET MVC application.", result.ViewBag.Message);
        }

        [Fact]
        public void About()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.About() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void Contact()
        {
            // Arrange
            HomeController controller = new HomeController();

            // Act
            ViewResult result = controller.Contact() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }
    }
}
