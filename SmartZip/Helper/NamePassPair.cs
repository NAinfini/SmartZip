using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartZip.Helper
{
    public class NamePassPair
    {
        public int ID { get; set; }
        public string Password { get; set; }

        public NamePassPair(int id, string password)
        {
            ID = id;
            Password = password;
        }
    }
}