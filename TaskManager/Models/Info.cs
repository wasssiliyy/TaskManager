using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManager.Models
{
    public class Info
    {
        public string Name { get; set; }
        public double CPU { get; set; }
        public int Memory { get; set; }
        public float Disk { get; set; }
        public float Network { get; set; }
    }
}
