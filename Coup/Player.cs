using System;
using System.Collections.Generic;
using System.Threading;
using Timer = System.Timers.Timer;

namespace Coup
{
    public class Player
    {
        private int _number;
        private String _name;
        private int _coins;
        private bool _active;
        private bool _isCPU; 
        private List<Role> _hand = new List<Role>();

        /* Creates a new Player along with the correct attributes including the Player's Name */
        public Player(int playerNo, bool isCpuGame)
        {
            _number= playerNo;
            _isCPU = isCpuGame && (_number > 1);
            _coins = 2;
            _active = true;
            if (_isCPU)
            {
                _name = "CPU" + (_number - 1);
            }
            else
            {
               SetPlayerName(playerNo);
            }
        }

        /* Sets player name */
        private void SetPlayerName(int playerNo)
        {
            while (string.IsNullOrEmpty(_name))
            {
                Console.WriteLine("Hello. Player " + _number + ". What is your name?");
                _name = Console.ReadLine();

                if (string.IsNullOrEmpty(_name))
                {
                    Console.WriteLine("This is not an acceptable Name. Please enter a valid one.");
                    Console.WriteLine();
                }
            }
            
            Console.WriteLine("Player " + playerNo + " is " + _name);
            Console.WriteLine();
        }
        
        public String GetPlayerName()
        {
            return _name;
        }

        public void SetCoins(int income)
        {
            this._coins = income;
        }

        public int GetCoins()
        {
            return _coins;
        }
        
        public bool GetPlayerActive()
        {
            return _active;
        }

        public bool GetIsCpu()
        {
            return _isCPU;
        }

        public List<Role> GetHand()
        {
            return _hand;
        }
        
        /* Displays the current status of a Player */
        public void DisplayPlayerStatus()
        {
            Console.WriteLine("Player: " + _name + " | Coins: " + _coins);
            Console.Write("Roles: ");

            foreach (var role in _hand)
            {
                if (role.GetActiveness())
                {
                    Console.Write("Unknown");
                }
                else
                {
                    Console.Write(role.GetName());
                }
                
                Console.Write(" | ");
            }

            if (!_active)
            {
                Console.Write("ELIMINATED!");
            }
            
            Console.WriteLine();
            Console.WriteLine();
        }

        /* Getting a Player's Input, whether a player is a CPU and whether a timer is required.  */
        public static int GetInput(int limit, bool isCpu, bool isTimed = false)
        {
            var value = 0;
            var found = false;
            var countdown = 8;
            
            if (isCpu)
            {
                Random rand = new Random();
                value = rand.Next(1, limit + 1);
                Thread.Sleep(50);
            }
            else
            {
                if (isTimed)
                {
                    var timer = new Timer(1000);
                    timer.Elapsed += OnTimedEvent;
                    timer.AutoReset = true;
                    timer.Enabled = true;

                    void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)
                    {
                        if (found)
                        {
                            timer.Enabled = false;
                        }
                        else
                        {
                            Console.WriteLine(countdown + " second(s) to decide. ");
                            countdown--;
                        }

                        if (countdown == 0)
                        {
                            Console.WriteLine();
                            Console.WriteLine("Time's up.");
                            Console.WriteLine("There's will be no challenges.");
                            timer.Enabled = false;
                        }
                    }
                }

                while (found == false && countdown > 0)
                {
                    if (int.TryParse(Console.ReadLine(), out value)) 
                    {
                        found = true;
                    }
                    else
                    {
                        Console.WriteLine("Please enter a correct option:...");
                    }

                    if (value > limit)
                    {
                        found = false;
                        Console.WriteLine("Please enter a correct option:...");
                    }
                }

                if (countdown == 0)
                {
                    value = 2;
                }
            }

            return value;
        }

        /* This will only be used in a 2-Player Game. A player selects there Character Roles.
         A list of roles are displayed and then assigned to a Player's hand. */
        public void ChooseCharacterRole(String [] roles)
        {
            Console.WriteLine("Please select YOUR first Influence: ");
            var counter = 0;
            var chosenInfluence = "";
            
            foreach (var influenceName in roles) 
            {
                counter++;
                Console.WriteLine(counter + " - " + influenceName);
            }

            chosenInfluence = roles[GetInput(roles.Length, _isCPU) - 1];
            Console.WriteLine(_name + " chooses " + chosenInfluence + ".");
            Console.WriteLine();
            Role firstInfluence = new Role(chosenInfluence);
            _hand.Add(firstInfluence);
        } 

        /* Gets a new Character Role from deck and then shuffles the deck */
        public void GetNewCharacterRoleFromDeck(Deck deck)
        {
            Role newRole = deck.GiveCharacterRole();
            _hand.Add(newRole);
            deck.ShuffleDeck();
            if (!_isCPU)
            {
                Console.WriteLine(_name + " got " + newRole.GetName());
                Console.ReadLine();
            }
        }

        /* Checks whether a player has any active roles in their hand. */
        private bool ChecksActiveRoles()
        {
            var playerActive = false;
            
            foreach (var card in _hand)
            {
                if (card.GetActiveness())
                {
                    playerActive = true;
                    break;
                }
            }

            if (!playerActive)
            {
                Console.WriteLine(_name + " has no more Character Roles and is out of the game.");
            }
            return playerActive;
        }

        /* This method deactivates one of a player's active Character Roles. First check the number of active character
         Roles that the target has. If the target has multiple Characters roles then they choose which role to 
         deactivate. */
        public void DeactivateCharacterRole()
        {
            var noOfActiveCards = 0;
            
            /* Count active cards */
            foreach (var influence in _hand)
            {
                if (influence.GetActiveness())
                {
                    noOfActiveCards++;
                }
            }
            
            if (noOfActiveCards > 0)
            {
                Console.WriteLine();

                if (noOfActiveCards == 1)
                {
                    foreach (var influence in _hand)
                    {
                        if (influence.GetActiveness())
                        {
                            influence.SetInfluenceActive(this);
                        }
                    }
                }
                else
                {
                    /* When multiple roles are available the player must selected a role to be removed. */
                    Console.WriteLine(_name + " selects Role to deactivate...");
                    var i = 1;

                    if (_isCPU)
                    {
                        Console.ReadLine();
                    }
                    else
                    {
                        foreach (var role in _hand)
                        {
                            Console.WriteLine(i + " - " + role.GetName());
                            i++;
                        }
                    }

                    /* Removes the selected character roles from Hand */
                    if (GetInput(noOfActiveCards, _isCPU) == 1)
                    {
                        _hand[0].SetInfluenceActive(this);
                    }
                    else
                    {
                        _hand[1].SetInfluenceActive(this);
                    }
                }
            }

            /* Re-assess whether the player is still active */
            _active = ChecksActiveRoles();
        }

        /* This method allows a player to select a target for their action. */
        public Player SelectTarget(List<Player> playerList)
        {
            var counter = 0;
            var selectedOppNo = 0;
            Player[] opponentPlayers = new Player[playerList.Count];
            
            /* Provides a list of targets */
            Console.WriteLine("Choose an Opponent:");
            foreach (var player in playerList)
            {
                if (player._active && player != this)
                {
                    opponentPlayers[counter] = player;
                    counter++;
                    Console.WriteLine(counter + " - " + player.GetPlayerName());
                }
            }
            
            selectedOppNo = GetInput(counter, _isCPU);
            return opponentPlayers[selectedOppNo - 1];
        }

        /* The Player's character role in question should be publicly displayed. This method checks whether the role is
         in the character's hand. */
        public Role RevealCorrectCharacterRole(String card)
        {
            foreach (var role in _hand)
            {
                if (role.GetName() == card && role.GetActiveness())
                {
                    return role;
                }
            }
            return null;
        }

        /* Replaces one of player's character roles with a new one. */
        public void ReplaceCharacterRole(Deck deck, String card)
        {
            foreach (var influence in _hand)
            {
                /* We first have to find the correct card to replace. */ 
                if (influence.GetName() == card && influence.GetActiveness())
                {
                    /* Removes card from the player and adds it to the deck. */
                    Console.WriteLine(_name + " has to replace Character Role.");
                    _hand.Remove(influence);
                    deck.GetDeck().Add(influence);
                    Console.WriteLine(_name + " gives their Character role back to Court Deck");
                    Console.ReadLine();
                    /* Adds new Character Role to players Hand */
                    GetNewCharacterRoleFromDeck(deck);
                    break;
                }   
            }
        }
        
        /* Determines whether a player wishes to block or challenge? */
        public bool IsBlock()
        {
            var isBlock = false;
            var playerInput = 0;
            
            Console.WriteLine(_name + ". Do you wish to block or challenge?");
            Console.WriteLine("1 - Block");
            Console.WriteLine("2 - Challenge");
            playerInput = GetInput(2, _isCPU);
            Console.WriteLine();
            Console.ReadLine();

            if (playerInput == 1)
            {
                isBlock = true;
            }

            return isBlock;
        }

        /* Displays a Player's Hand */
        public void DisplayHand()
        {
            foreach (var influence in _hand)
            {
                if (influence.GetActiveness())
                {
                    Console.WriteLine(influence.GetName());
                }
            }

            Console.ReadLine();
        }
    }
    
}