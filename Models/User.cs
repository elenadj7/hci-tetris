using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Models
{
    public class User
    {
        private string username;
        private int score;

        public String Username { get { return username; } set { username = value; } }
        public int Score { get { return score; } set { score = value; } }

        public User()
        {

        }

        public User(string username)
        {
            this.username = username;
        }

        public User(string username, int score)
        {
            this.username = username;
            this.score = score;
        }
    }
}
