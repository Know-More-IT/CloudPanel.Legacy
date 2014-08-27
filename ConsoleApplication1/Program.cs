using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            string dn = "CN=tetststtststs_APP0,OU=Exchange,OU=APP0,OU=APP,OU=Hosting,dc=appliwave,dc=local";
            dn = dn.ToUpper();

            // Now set the canonical name
            string[] originalArray = dn.Split(',');
            string[] reversedArray = dn.Split(',');
            Array.Reverse(reversedArray);

            string canonicalName = string.Empty;
            foreach (string s in reversedArray)
            {
                if (s.StartsWith("CN="))
                    canonicalName += s.Replace("CN=", string.Empty) + "/";
                else if (s.StartsWith("OU="))
                    canonicalName += s.Replace("OU=", string.Empty) + "/";
            }

            // Remove the ending slash
            canonicalName = canonicalName.Substring(0, canonicalName.Length - 1);

            // Now our canonical name should be formatted except for the DC=
            // Lets do the DC= now
            string domain = string.Empty;
            foreach (string s in originalArray)
            {
                if (s.StartsWith("DC="))
                    domain += s.Replace("DC=", string.Empty) + ".";
            }

            // Remove the ending period
            if (domain.EndsWith("."))
                domain = domain.Substring(0, domain.Length - 1);

            // Now finally set it
            Console.WriteLine(string.Format("{0}/{1}", domain, canonicalName));
            Console.ReadKey();
        }
    }
}
