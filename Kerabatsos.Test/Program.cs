using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kerabatsos.EBML;

namespace Kerabatsos.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            FileStream ice = File.Open("ice_fall.webm", FileMode.Open);
            EBMLReader reader = new EBMLReader(ice);
        }
    }
}
