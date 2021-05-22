using System;
using System.Collections.Generic;

namespace Coup

{
    public class Deck
    {
        private List <Role> _courtDeck = new List <Role>();

        
        /*This produces a deck of cards. Three of each character Roles. 15 Cards each. If they're after only two players
         there is a deck only consisted on 5 cards each a single character role each. */
        public void ProduceDeck(String [] titles, int noOfPlayers)
        {
            Role roles;
            var noOfRoleAppereances = 0;
            
            if (noOfPlayers > 2)
            {
                noOfRoleAppereances = 3;
            }
            else
            {
                noOfRoleAppereances = 1;
            }

            /*Create a card for each chaaracter role a certain number of times and add them to the deck*/
            foreach (var t in titles)
            {
                for (int i = 0; i < noOfRoleAppereances; i++)
                {
                    roles = new Role(t);
                    _courtDeck.Add(roles);
                }
            }
        }

        public List<Role> GetDeck()
        {
            return _courtDeck;
        }
        
        
        /* Gets Character Role and removes it from the deck */
        public Role GiveCharacterRole()
        {
            Role selectedInfluence = _courtDeck[0];
            _courtDeck.Remove(selectedInfluence);
            return selectedInfluence;
        }
        
        /* Shuffles the Court Deck of the game */
        public void ShuffleDeck()
        {
            var rand = new Random();

            for (var i = _courtDeck.Count; i > 0; i--)
            {
                var firstPos = i - 1;
                var lastPos = rand.Next(0 , firstPos);
                var temp = _courtDeck[firstPos];
                _courtDeck[firstPos] = _courtDeck[lastPos];
                _courtDeck[lastPos] = temp;
            }
        }
        
        
    }
}