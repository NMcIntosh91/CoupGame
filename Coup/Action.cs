using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;

namespace Coup
{
    public class Action
    {
        
        String[] actions = {"Income", "Foreign Aid", "Coup", "Tax", "Steal", "Exchange", "Assassinate", "Check Influences"};
        
        /* Increments the variable 'coins' by 1 */
        public static int IncomeAction(int coins)
        {
            return coins += 1;
        }

        /* Increments the variable 'coins' by 2 */
        public static int ForeignAidAction(int coins)
        {
            return coins += 2;
        }
        
        /* Removes 7 coins from the player and active the Deactivated Role method*/
        public static void Coup(Player player, Player opponent)
        {
            //Player plays 7 Coins
            player.SetCoins(player.GetCoins() - 7);
            
            //Opponent Kills Card
            opponent.DeactivateCharacterRole();

        }
        
        /* Increments the variable 'coins' by 3 */
        public static int DukeAction(int coins)
        {
            return coins += 3;
        }

        /* Increments the Player's variable 'coins' by 2. Remove the target's coints by 2 or 1.
         By this point a check should have already been made that target DOES have some coins
         to steal. */
        public static void CaptainAction(Player player, Player target)
        {

            var coinsToSteal = 2;
            
            if (target.GetCoins() < 2)
            {
                coinsToSteal = 1;
            }
            player.SetCoins(player.GetCoins() + coinsToSteal);
            target.SetCoins(target.GetCoins() - coinsToSteal);
        }
        
        public static void ExchangeAction(Player player, List<Role> hand, Deck deck)
        {
            //IEnumerable<Influence> myQuery = hand.AsQueryable().Where(influence.GetActiveness());
            
            List<Role> exchangeList = new List<Role>();
            List<Role> newHand = new List<Role>();
            var cardsToRetrieve = 0;
            var counter = 0;
            var inputNo = 0;
            var indexSelection = 0;
            var isInactiveInfluences = false;
            Role newRole = null;
            List<Role> courtDeck = deck.GetDeck();
            
            
            /* Retrieve two cards from the deck */
            for (var i = 0; i < 2; i++)          
            {
                exchangeList.Add(courtDeck[0]);
                courtDeck.Remove(courtDeck[0]);
            }

            /* Retrieves ACTIVE roles from Player's hand*/
            foreach (var role in hand)
            {
                if (role.GetActiveness())
                {
                    cardsToRetrieve++;
                    exchangeList.Add(role); 
                }
                else
                {
                    isInactiveInfluences = true;
                }
            }
            
            Console.WriteLine();

            /* If Player is not CPU then player must manually choose their new cards. */
            for (var i = 0; i < cardsToRetrieve; i++) 
            {
                if (!player.GetIsCpu())
                {
                    counter = 0;
                    Console.WriteLine(player.GetPlayerName() + " choose an Influence for your hand:");   
                
                    foreach (var role in exchangeList) 
                    {
                        counter++;
                        Console.WriteLine(counter + " - " + role.GetName());
                    }
                }
                    
                inputNo = Player.GetInput(exchangeList.Count, player.GetIsCpu());
                indexSelection = inputNo - 1;
                var selectedRole = exchangeList[indexSelection];
                newHand.Add(selectedRole);
                exchangeList.Remove(selectedRole);
            }
            
            /* Return the un-chosen roles in the 'Exchange List' back into the deck and then shuffle the deck */
            foreach (Role influence in exchangeList)  
            {
                if (influence != null)
                {
                    courtDeck.Add(influence);
                }
            }

            deck.ShuffleDeck();
            
            /* Returns Player's previous inactive roles to their hand */
            if (isInactiveInfluences)
            {
                foreach (var influence in hand)
                {
                    if (!influence.GetActiveness())
                    {
                        newHand.Add(influence);
                    }
                }
            }
            
            //Reset Players hand -- NO IDEA WHAT THIS DOES!?!?
            hand.RemoveRange(0,2);
            
            /* Places newly selected roles into Player's Hand */
            foreach (var role in newHand)   
            {
                hand.Add(role);
            }
        }

        /* Target must choose one of their roles to eliminate */
        public static void AssassinAction(Player player, Player target)
        {
            target.DeactivateCharacterRole();
        }

        /* A player has challenged another player claiming to be a certain character Role. Determines whether challenged
         * Player has the correct role. Is fo the challenge is lost and the CHALLENGER has to deactivate a card. If the
         * challenged is won, the CHALLENGED player must deactivated a card. */
        public static bool Challenge(Player challenger, Player challengedPlayer, String card, Deck deck)
        {

            Console.WriteLine(challenger.GetPlayerName() + " challenges " + challengedPlayer.GetPlayerName() + "'s claim to be " + card);
            Console.ReadLine();
            
            var isCorrectCard = challengedPlayer.RevealCorrectCharacterRole(card) != null;
            var isSuccessfulChallenge = ! isCorrectCard;

            if (isCorrectCard)
            {
                Console.WriteLine(challengedPlayer.GetPlayerName() + " reveals " + card + "!");
                Console.ReadLine();
                Console.WriteLine(challenger.GetPlayerName()  + " must now lose a Role card.");
                challenger.DeactivateCharacterRole();
                Console.ReadLine();
                challengedPlayer.ReplaceCharacterRole(deck, card);
            }
            else
            {
                Console.WriteLine(challengedPlayer.GetPlayerName() + " does not have a " + card);
                Console.WriteLine(challengedPlayer.GetPlayerName() + " loses an influence");
                challengedPlayer.DeactivateCharacterRole();
                Console.ReadLine();
            }

            return isSuccessfulChallenge;
        }
        
        /* Targeted player block an action because they claim to be a particular Character Role. */  
        public static string GetBlockingRole (Player blocker, String action, Player opponent)
        {
            var blockingCard = "";

            switch (action)
            {
                case "Foreign Aid":
                    blockingCard = "Duke";
                    break;
                case "Assassinate":
                    blockingCard = "Contessa";
                    break;
                default:
                    Console.WriteLine(blocker.GetPlayerName() + ". You are blocking " + opponent.GetPlayerName() + " from stealing because you claim to be a...");
                    Console.WriteLine("1 - Captain");
                    Console.WriteLine("2 - Ambassador");
                    var playerInput = Player.GetInput(2, blocker.GetIsCpu(), false);

                    if (playerInput == 1)
                    {
                        blockingCard = "Captain";
                    }
                    else
                    {
                        blockingCard = "Ambassador";
                    }
                    break;
            }
            
            Console.WriteLine(blocker.GetPlayerName() + " claims " + blockingCard + " to block " + action);
            Console.ReadLine();
            
            return blockingCard;
        }

    }
}