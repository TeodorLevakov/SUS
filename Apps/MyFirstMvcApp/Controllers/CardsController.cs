using BattleCards.Data;
using BattleCards.Services;
using BattleCards.ViewModels;
using BattleCards.ViewModels.Cards;
using SUS.HTTP;
using SUS.MvcFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCards.Controllers
{
    public class CardsController : Controller
    {
        private readonly ICardsService cardsService;

        public CardsController(ICardsService cardsService)
        {
            this.cardsService = cardsService;
        }

        public HttpResponse Add() 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }
            return this.View();
        }

        [HttpPost]
        public HttpResponse Add(AddCardInputModel model) 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            if (string.IsNullOrEmpty(model.Name) || model.Name.Length < 5 || model.Name.Length > 15)
            {
                return this.Error("Name should be between 5 and 15 ch long");
            }
            if (string.IsNullOrWhiteSpace(model.Image))
            {
                return this.Error("Image is required!");
            }
            if (!Uri.TryCreate(model.Image, UriKind.Absolute, out _))
            {
                return this.Error("Url should be valid!");
            }
            if (string.IsNullOrWhiteSpace(model.Keyword))
            {
                return this.Error("Keyword is required.");
            }
            if (model.Attack < 0)
            {
                return this.Error("Attack should be non-negativ integer.");
            }
            if (model.Health < 0)
            {
                return this.Error("Health should be non-negativ integer.");
            }
            if (string.IsNullOrWhiteSpace(model.Description) || model.Description.Length > 200)
            {
                return this.Error("Description is not valid.");
            }

            var cardId = this.cardsService.AddCard(model);

            var userId = this.GetUserId();

            this.cardsService.AddCardToUserCollection(userId, cardId);

            return this.Redirect("/Cards/All");
        }

        public HttpResponse All()
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var cardsViewModel = this.cardsService.GetAll();

            return this.View(cardsViewModel);
        }

        public HttpResponse Collection()
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var userId = this.GetUserId();
            var viewModel = this.cardsService.GetByUserId(userId);

            return this.View(viewModel);
        }

        public HttpResponse AddToCollection(int cardId) 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var userId = this.GetUserId();
            this.cardsService.AddCardToUserCollection(userId, cardId);
            return this.Redirect("/Cards/All");
        }

        public HttpResponse RemoveFromCollection(int cardId) 
        {
            if (!this.IsUserSignedIn())
            {
                return this.Redirect("/Users/Login");
            }

            var userId = this.GetUserId();
            this.cardsService.RemoveCardFromCollection(userId, cardId);
            return this.Redirect("/Cards/Collection");
        }
    }
}
