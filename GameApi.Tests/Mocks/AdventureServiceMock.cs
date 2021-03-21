using System.Collections.Generic;
using System;

using Moq;

using GameApi.Entities;
using GameApi.Repositories;
using GameApi.Services;

namespace GameApi.Tests.Mocks {
    public class AdventureServiceMock {
        public static AdventureService MockAdventureService() {
            var adventures = new List<Adventure>();
            adventures.Add(new Adventure { 
                Id = new System.Guid("d4e1de74-7271-4ed8-8e86-35dbb3cd6b3e"),
                Name = "Demo"
            });
            var mockAdvRepo = new Mock<IAdventureRepository>();
            mockAdvRepo.Setup(repo => repo.GetAdventureByName(It.IsAny<string>()))
                .ReturnsAsync((string name) => adventures.Find(adv => adv.Name == name));
            mockAdvRepo.Setup(repo => repo.GetAdventureById(It.IsAny<Guid>()))
                .ReturnsAsync((Guid id) => adventures.Find(adv => adv.Id == id));
            return new AdventureService(mockAdvRepo.Object);
        }
    }
}