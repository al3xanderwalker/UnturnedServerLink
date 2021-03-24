using System;
using System.Linq;

namespace USLModule.Models
{
    public class Link
    {
        public string Steam;
        public string Discord;
        public string Code;

        public Link(string steam)
        {
            Steam = steam;
            Discord = "";
            Code = RandomString(16);
        }
        
        private static readonly Random Random = new Random();

        private static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }
    }
}