using hci_tetris.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Repositories
{
    internal interface IRepository
    {
        bool CheckIfExists(string username);
        void Add(string username, int score);
        void Update(string username, int score);
        List<User> GetAll();
        User Get(string username);
    }
}
