using SQLite.Net.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1
{
    public class Sites
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [MaxLength(64)]
        public string Name { get; set; }

        public string Url { get; set; }
    }
}
