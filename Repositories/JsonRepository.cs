using hci_tetris.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hci_tetris.Repositories
{
    internal class JsonRepository : IRepository
    {
        private readonly string jsonPath = "..\\..\\..\\Data\\Users.json";

        public void Add(string username, int score)
        {
            List<User> users = [];

            if (File.Exists(jsonPath))
            {
                string text = File.ReadAllText(jsonPath);

                JObject jsonObject = JObject.Parse(text);

                if (jsonObject["users"] is JArray usersArray)
                {
                    users = usersArray.ToObject<List<User>>();
                }
            }

            users?.Add(new User(username, score));

            string newJson = JsonConvert.SerializeObject(new { users }, Formatting.Indented);
            File.WriteAllText(jsonPath, newJson);
        }


        public bool CheckIfExists(string username)
        {
            bool exists = false;
            string text = File.ReadAllText(jsonPath);

            JObject? jsonObject = JsonConvert.DeserializeObject<JObject>(text);
            if (jsonObject != null && jsonObject.TryGetValue("users", out JToken? usersToken))
            {
                JArray? usersArray = usersToken?.ToObject<JArray>();
                if (usersArray != null)
                {
                    List<User>? users = usersArray.ToObject<List<User>>();
                    if (users != null)
                    {
                        exists = users.Any(u => u.Username == username);
                    }
                }
            }

            return exists;
        }

        public List<User> GetAll()
        {
            List<User> users = [];

            if (File.Exists(jsonPath))
            {
                string text = File.ReadAllText(jsonPath);

                JObject jsonObject = JObject.Parse(text);

                if (jsonObject["users"] is JArray usersArray)
                {
                    users = usersArray.ToObject<List<User>>();
                }
            }

            return users;
        }

        public void Update(string username, int score)
        {
            List<User> users = [];

            if(File.Exists(jsonPath))
            {
                string text = File.ReadAllText(jsonPath);

                JObject jsonObject = JObject.Parse(text);

                if (jsonObject["users"] is JArray usersArray)
                {
                    users = usersArray.ToObject<List<User>>();
                    User? userToUpdate = users.FirstOrDefault(u => u.Username == username);

                    if (userToUpdate != null)
                    {
                        userToUpdate.Score = score;
                        string newJson = JsonConvert.SerializeObject(new { users }, Formatting.Indented);
                        File.WriteAllText(jsonPath, newJson);
                    }
                }
            }
        }
    }
}
