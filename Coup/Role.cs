using System;

namespace Coup
{
    public class Role
    {
        private String _name;
        private bool _isActive;
        String[] titles = {"Duke", "Captain", "Ambassador", "Assassin", "Contessa"};

        public Role(String title)
        {
            _name = title;
            _isActive = true;
        }

        public String GetName()
        {
            return _name;
        }

        public bool GetActiveness()
        {
            return _isActive;
        }

        /* Ultimately this is 'Kill Influence' method. This deactivates a card. */
        public void SetInfluenceActive(Player player)
        {
            Console.WriteLine(player.GetPlayerName() + "'s " + _name + " is longer active.");
            _isActive = false;
        }
    }
}