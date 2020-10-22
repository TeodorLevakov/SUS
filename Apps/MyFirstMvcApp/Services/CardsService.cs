using BattleCards.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleCards.Services
{
    public class CardsService : ICardsService
    {
        private readonly ApplicationDbContext db;

        public CardsService(ApplicationDbContext db)
        {
            this.db = db;
        }
        public void AddCard()
        {
            throw new NotImplementedException();
        }
    }
}
