using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;

namespace Coup
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            /* Sets up Players within the game */
            List<Player> playerList = new List<Player>();
            Deck deck = new Deck();
            var isCpuGame = false;
            String[,] actions =
            {
                {"Income", "Foreign Aid", "Coup", "Tax", "Steal", "Exchange", "Assassinate", "Check Influences"},
                {null, null, null, "Duke", "Captain", "Ambassador", "Assassin", null},
                {null, "block", null, null, "block", null, "block", null}
            };
            
            GameSetup();
            GameLoop();

            /* Sets up the game */
            void GameSetup()
            {
                /* First we set up the 5 possible Characters Roles.
                 * Then we set up the all possible Player Actions
                 */
                String[] titles = {"Duke", "Captain", "Ambassador", "Assassin", "Contessa"};

                /* Determines Single or Multi-Player Game */
                Console.WriteLine("Against players or CPU?");
                Console.WriteLine("1 - Players");
                Console.WriteLine("2 - CPU");
                isCpuGame = Player.GetInput(2, false) == 2;
                
                /* Determines Number of Players */
                Console.WriteLine("How many Players in this game? (Only 2 - 6 Players Allowed).");
                var noOfPlayers = Player.GetInput(6, false);
                
                deck.ProduceDeck(titles, noOfPlayers);
                deck.ShuffleDeck();

                for (int i = 0; i < noOfPlayers; i++)
                {
                    Player player = new Player(i + 1, isCpuGame);
                    playerList.Add(player);

                    /* If 2-Players Game then each Player chooses their first Influences */
                    if (noOfPlayers == 2)
                    {
                        player.ChooseCharacterRole(titles);
                    }
                }

                /* Each Player collect two Random Influence(s)
                 * If 2 Player Game collect one Random Influence.
                 */

                var cardsToCollect = 2;

                if (playerList.Count == 2)
                {
                    cardsToCollect = 1;
                }

                for (var i = 0; i < cardsToCollect; i++)
                {
                    foreach (var player in playerList)
                    {
                        player.GetNewCharacterRoleFromDeck(deck);
                    }
                }
            }
            
            /* Now the the game set up the game can now begin. This is the made Game Loop */
            void GameLoop()
            {
                /* Vital game statistics */
                var actionNumber = 0;
                var isActionDone = false;
                var gameOver = false;
                
                /* This will loop until somebody wins and the end is over. */
                while (gameOver == false)
                {
                    /* Looping around every player */
                    foreach (var player in playerList)
                    {
                        isActionDone = false;
                    
                        /*This trigger the end of the game */
                        if (gameOver)
                        {
                            break;
                        }

                        /* This ensures that a player has completed their turn */
                        while (isActionDone == false)
                        {
                            /* Checks if the game is over*/
                            if (GameOverCheck())
                            {
                                gameOver = true;
                                Console.WriteLine("The Game is Over.");
                                Player winner = FindWinner();
                                Console.WriteLine(winner.GetPlayerName() + " WINS!");
                                break;
                            }
                            
                            /* If a Player is no longer active then the game skipped to the next player */
                            if (!player.GetPlayerActive())
                            {
                                isActionDone = true;
                                Console.WriteLine(player.GetPlayerName() + " is out of the game.");
                            }
                            else
                            {
                                /* Display Each Player's Stats */
                                
                                Console.WriteLine("PLAYER STATS");
                                DisplayMenuHeading();
                                
                                foreach (var p in playerList)
                                {
                                    p.DisplayPlayerStatus();
                                }

                                Console.ReadLine();

                                /* Now the player can choose their action */ 
                                actionNumber = ChooseAction(player);
                                isActionDone = GetAction(player, actionNumber);
                                Console.WriteLine();
                            }
                        }
                    }
                }  
            }
            
            /* This method determine whether or not only one player is alive and therefore the game is over.
             The methdd only returns a 'true' value if the noActivePlayer variable is 1*/
            bool GameOverCheck()
            {
                var noActivePlayers = 0;

                foreach (var player in playerList)
                {
                    if (player.GetPlayerActive())
                    {
                        noActivePlayers++;
                    }
                }

                return noActivePlayers == 1; 
            }

            /* This method search for the last active Player in the game. It will immedaliately return to first active
             player that it finds. */
            Player FindWinner()
            {
                foreach (var player in playerList)
                {
                    if (player.GetPlayerActive())
                    {
                        return player;
                    }
                }

                return null; //If this happens we have problems. 
            }

            /* This method configures how a player selects their action */
            int ChooseAction(Player player)
            {
                int chosenAction;
                var counter = 0;
                var playerName = player.GetPlayerName();
                var menuLength = 8;
                const int rowOfActions = 0;

                /* The CPU doesn't need to check Influences */
                if (player.GetIsCpu())
                {
                    menuLength = 7;
                }

                /* Indicates this Player's turn */
                Console.WriteLine(playerName + "'s Turn");
                DisplayMenuHeading();
                Console.Write(playerName.ToUpper() + ". ");;


                /* RULE: In Coup, if a player has starts their turn if MORE than 10 coins then that player MUST use the
                 Coup action */
                if (player.GetCoins() > 10)
                {
                    Console.WriteLine(player.GetPlayerName() + " has more than 10 coins and therefore has to Coup.");
                    chosenAction = 3;
                }
                /* Otherwise, here the Player chooses the action they would like to take */
                else
                {
                    Console.WriteLine("Choose an action: ");

                    for (var i = 0; i < menuLength; i++)
                    {
                        counter++;
                        Console.WriteLine(counter + " - " + actions[rowOfActions, i]);
                    }

                    chosenAction = Player.GetInput(menuLength, player.GetIsCpu());
                    Console.WriteLine();
                    Console.WriteLine(playerName + " plays " + actions[rowOfActions, chosenAction - 1]);
                    Console.ReadLine();
                }

                return chosenAction;
            }

            bool GetAction(Player player, int actionNo)
            {
                Player opponent = null;
                var isEndTurn = false;
                var playerCoins = player.GetCoins();
                var cardName = actions[1, actionNo - 1];
                var actionName = actions[0, actionNo - 1];
                int coinsNeeded;
                

                switch (actionNo)
                {
                    /* -- The Income Action --
                     The player collects only one coin. The action doesn't require any character role.
                     The action also CANNOT be blocked by another player.
                     */ 
                    
                    case 1:
                        player.SetCoins(Action.IncomeAction(playerCoins));
                        Console.WriteLine(player.GetPlayerName() + " collects 1 coin.");
                        isEndTurn = ConfirmActionEnd();
                        
                            break;
                    
                    case 2:
                        /* -- The Foreign Aid Action --
                         The player collects two coins. This action doesn't require any character role.
                         However this action CAN be blocked by a player claiming to be "Duke". */
                         
                        Player blocker = GetChallenger(player, null, actionName); 
                        
                        /* If "Blocker exists when the Blocker's character role (Duke) must be determined */
                        if (blocker != null)
                        {
                            isEndTurn = GetBlockAction(blocker, player, actionName);
                        }
                        
                        /* If not blocking was attempted or if blocking attempt failed we proceed with the Foreign Aid
                         action* */
                        if (!isEndTurn)
                        {
                            player.SetCoins(Action.ForeignAidAction(playerCoins));
                            Console.WriteLine(player.GetPlayerName() + " collects 2 coins.");
                            isEndTurn = ConfirmActionEnd();
                        }
                        break;
                    
                    case 3:
                        /* --The Coup Action --
                         This action required 7 coins. This action is to be used on another player. This is doesn't not 
                         require any character role. The action removes another Player's character role. One activated 
                         this cannot be blocked.
                         */
                        coinsNeeded = 7;
                        if (playerCoins >= coinsNeeded) 
                        {
                            opponent = player.SelectTarget(playerList);
                            Action.Coup(player, opponent);
                            isEndTurn = ConfirmActionEnd();
                        }
                        else
                        {
                            DisplayNotEnoughCoins(coinsNeeded);
                        }
                        break;
                    
                    case 4:
                        /* --The Tax Action --
                         * The player collects 3 coins. This action requires the player to claim the "Duke" role which
                         * means this action CAN be challenged. */
                        
                        if (!IsActionChallenged(player, cardName, actionName, null)) 
                        {
                            player.SetCoins(Action.DukeAction(playerCoins));
                            Console.WriteLine(player.GetPlayerName() + " claims Duke and collect 3 coins.");
                        }
                        isEndTurn = ConfirmActionEnd();
                        break;
                    
                    case 5:
                        /* --The Steal Action --
                         * The Player steals 2 coins from another player. This action requires the Player to claim the
                         * "Captain" role which means this action CAN be challenged. The action can also be blocked by
                         * the targeted player. 
                         */
                        
                        /*The player choose their target */
                        opponent = player.SelectTarget(playerList);
                        Console.WriteLine(player.GetPlayerName() + " wishes to steal from " + opponent.GetPlayerName());
                        Console.WriteLine();
                        
                        if (opponent.GetCoins() >= 2)
                        {
                            /* If someone Challenge the player's Role ("Captain") */
                            if (!IsActionChallenged(player, cardName, actionName, opponent))
                            {
                                Action.CaptainAction(player, opponent);
                                Console.WriteLine(player.GetPlayerName() + " stole 2 coins from " + opponent.GetPlayerName());
                            }
                            isEndTurn = ConfirmActionEnd();
                        }
                        else
                        {
                            Console.WriteLine(opponent.GetPlayerName() +  " doesn't have enough coins. Try again.");
                        }
                        break;
                    
                    case 6:
                        /* This is the exchange action. The player can view 2 characters roles from the deck and exchange them with
                         their current active roles. Using this action the player must claim the role of 'Ambassador' 
                         meaning that they can be challenged by another player. */
                        
                        /* If someone Challenge the player's Role ("Ambassador") */
                        if (!IsActionChallenged(player, cardName, actionName, null))
                        {
                            Console.WriteLine(player.GetPlayerName() + " plays " + actionName + " and Exchanges their cards");
                            Action.ExchangeAction(player, player.GetHand(), deck);
                        }
                        isEndTurn = ConfirmActionEnd();

                        break;
                    case 7:
                        /* This is the Assassinate Action. This require 3 coins to use. A player can target another
                         player and eliminate a target's character role. The target can choose the blocked this attack
                         with a "Contessa". Another player can also challenge this action as this player is claming
                         to be an "Assassin" when doing this action.*/

                        /* This player must have at least 3 coins to perform this action */
                        coinsNeeded = 3;
                        if (playerCoins >= coinsNeeded)
                        {
                            opponent = player.SelectTarget(playerList);
                            Console.WriteLine(player.GetPlayerName() + " wishes to assassinate " + opponent.GetPlayerName());
                            Console.WriteLine();
                            player.SetCoins(playerCoins - coinsNeeded);

                            if (!IsActionChallenged(player, cardName, actionName, opponent))
                            {
                                Console.WriteLine(player.GetPlayerName() + " assassinates " + opponent.GetPlayerName());
                                Action.AssassinAction(player, opponent);
                            }
                            isEndTurn = ConfirmActionEnd();
                        }
                        else
                        {
                            DisplayNotEnoughCoins(coinsNeeded);
                        }
                        break;
                    case 8:
                        player.DisplayHand();
                        break;
                }

                return isEndTurn;
            }

            /* Determines whether of not a challenge will be made */
            Boolean IsActionChallenged(Player challengedPlayer, String card, String action, Player opponent)
            {
                Boolean endAction = false;
                var challenger = GetChallenger(challengedPlayer, card, action);

                if (challenger != null)
                {
                    endAction = GetChallengeOrBlockAction(challenger, challengedPlayer, opponent, card, action);
                }

                return endAction;
            }
            
            /* Provides the option for a player to challenge an Action as well as determining where the action can be
             * blocked and/or challenged. This is then display the correct message. */
            Player GetChallenger(Player challengedPlayer, String card, String action)
            {
                var playerInput = 0;
                var onlyBlock = card == null;
                var isCpuChallenger = !challengedPlayer.GetIsCpu() && isCpuGame; //Needs Changing
                Player playerToReturn = null;
                var text = "";
                var canBlock = CanBeBlocked(action);
                
                if (onlyBlock)
                {
                    text = "block ";
                    card = "Foreign Aid";
                }
                else if (canBlock)
                {
                    text = "challenge/block ";
                }
                else
                {
                    text = "challenge ";
                }
                
                Console.WriteLine("Would anyone like to " + text + challengedPlayer.GetPlayerName() + "'s " + card);
                Console.WriteLine("1 - Yes");
                Console.WriteLine("2 - No");
                playerInput = Player.GetInput(2, isCpuChallenger, true);
                
                /* When a Challenger has been confirmed select the challenger by providing a list of other players. */
                
                if (playerInput == 1)
                {
                    Player[] opponentPlayers = new Player [playerList.Count - 1];
                    var counter = 0;
                    var selectedOppNo = 0;
                    
                    Console.WriteLine();
                    Console.WriteLine("Who wants to " + text + "?");

                    foreach (var challenger in playerList)
                    {
                        if (challenger.GetPlayerActive() && challenger != challengedPlayer)
                        {
                            opponentPlayers[counter] = challenger;
                            counter++;
                            Console.WriteLine(counter + " - " + challenger.GetPlayerName());
                        }
                    }

                    selectedOppNo = Player.GetInput(counter, isCpuChallenger);

                    playerToReturn = opponentPlayers[selectedOppNo - 1];
                }

                return playerToReturn;
            }

            /* This methods checked whether a certain action can be blocked */
            bool CanBeBlocked(String action)
            {
                var canBeBlocked = action == "Steal" || action == "Foreign Aid" || action == "Assassinate";

                return canBeBlocked;
            }
            
            /* Determines Block or Challenge Action */
            bool GetChallengeOrBlockAction(Player challenger, Player player, Player target, String cardName, String actionName)
            {
                var isEndTurn = false;
                var isBlock = false;

                if (CanBeBlocked(actionName) && challenger == target)
                {
                    /*The target Player can choose whether to block or challenge. Determines the choice being made. */
                    isBlock = challenger.IsBlock();

                    if (isBlock)
                    {
                        isEndTurn = GetBlockAction(challenger, player, actionName);
                    }
                }

                /* If it's not a Block then it must be a Challenge */
                if (!isEndTurn && !isBlock)
                {
                    isEndTurn = Action.Challenge(challenger, player, cardName, deck);
                }
                
                return isEndTurn;
            } 

            /* Get the player performing the Blocking action and the character Role that claim to be
             * blocking with. The blocking action can also be challenged.
             * */
            bool GetBlockAction(Player blocker, Player initialPlayer, String actionName)
            {
                var isSuccessfulBlock = false;
                var blockingRole = Action.GetBlockingRole(blocker, actionName, initialPlayer);
                var blockerChallenger = GetChallenger(blocker, blockingRole, "block");    
                
                if (blockerChallenger != null) 
                {
                    isSuccessfulBlock = !Action.Challenge(blockerChallenger, blocker, blockingRole, deck);
                }
                
                if(isSuccessfulBlock)
                {
                    Console.WriteLine(blocker.GetPlayerName() + " blocks " + actionName + " with " + blockingRole);
                    Console.WriteLine();
                }

                return isSuccessfulBlock;
            } 

            /* This function displays the messages that more coins are needed than the player currently has */
            void DisplayNotEnoughCoins(int coinsNeeded)
            {
                Console.WriteLine("You need at least " + coinsNeeded + " coins for this action.");
                Console.WriteLine("Please choose another action.");
                ConfirmActionEnd();
            }

            void DisplayMenuHeading()
            {
                Console.WriteLine("______________________");
            }
            
            bool ConfirmActionEnd()
            {
                Console.ReadLine();
                return true;
            }
                
        }
    }
}