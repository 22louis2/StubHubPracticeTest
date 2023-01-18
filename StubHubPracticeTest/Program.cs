using System;
using System.Collections.Generic;
using System.Linq;

namespace TicketsConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var events = new List<Event>{
            new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23)),
            new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02)),
            new Event(3, "Metallica", "New York", new DateTime(2023,12,06)),
            new Event(4, "Metallica", "Boston", new DateTime(2023,10,23)),
            new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20)),
            new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01)),
            new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04)),
            new Event(8, "LadyGaGa","San Francisco", new DateTime(2023,07,07)),
            new Event(9, "LadyGaGa", "Washington", new DateTime(2023,05,22)),
            new Event(10, "Metallica", "Chicago", new DateTime(2023,01,01)),
            new Event(11, "Phantom of the Opera", "San Francisco", new DateTime(2023,07,04)),
            new Event(12, "Phantom of the Opera", "Chicago", new DateTime(2024,05,15))
            };
            var customer = new Customer()
            {
                Id = 1,
                Name = "John",
                City = "New York",
                BirthDate = new DateTime(1995, 05, 10)
            };

            var marketEngine = new MarketEngine(events);
            marketEngine.SendEventBasedOnLocation(customer);
            marketEngine.SendEventBasedOnBirthday(customer, 1);
            marketEngine.SendEventBasedOnPrice(customer, 1);
        }

        public class Event
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime Date { get; set; }

            public Event(int id, string name, string city, DateTime date)
            {
                this.Id = id;
                this.Name = name;
                this.City = city;
                this.Date = date;
            }
        }

        public class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string City { get; set; }
            public DateTime BirthDate { get; set; }
        }

        public class MarketEngine
        {
            private readonly List<Event> _events;
            public static Dictionary<(string, string), int> CachedDistances = new Dictionary<(string, string), int>();

            public MarketEngine(List<Event> events)
            {
                _events = events;
            }

            private static void SendCustomerNotifications(Customer customer, Event e)
            {
                Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} in {e.City} at {e.Date}");
            }

            public void SendEventBasedOnLocation(Customer customer)
            {
                _events.Where(x => x.City.Contains(customer.City))?.ToList()
                .ForEach(x => SendCustomerNotifications(customer, x));
            }

            public void SendEventBasedOnBirthday(Customer customer, int noEvents = 5)
            {
                DateTime nextBirthDate = customer.BirthDate.AddYears(DateTime.Today.Year
                    - customer.BirthDate.Year);

                if (nextBirthDate < DateTime.Today)
                    nextBirthDate = nextBirthDate.AddYears(1);

                _events.OrderBy(x => nextBirthDate - x.Date).Take(noEvents).ToList()?.ForEach(x => SendCustomerNotifications(customer, x));
            }

            public void SendEventBasedOnClosestEvents(Customer customer, int noEvents = 5)
            {
                _events.OrderBy(x => GetDistance(customer.City, x.City))?.Take(noEvents).ToList()?.ForEach(x => SendCustomerNotifications(customer, x));
            }

            public void SendEventBasedOnPrice(Customer customer, int noEvents = 5)
            {
                _events.OrderBy(x => GetDistance(customer.City, x.City))?
                .ThenBy(x => GetPrice(x))?.Take(noEvents)?.ToList()?
                .ForEach(x => SendCustomerNotifications(customer, x));
            }

            private static int GetPrice(Event e)
            {
                return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
            }

            private static int GetDistance(string cityA, string cityB, int tries = 1)
            {
                if (CachedDistances.TryGetValue((cityA, cityB), out var distance))
                    return distance;

                while (tries > 0)
                {
                    try
                    {
                        distance = AlphebiticalDistance(cityA, cityB);
                        break;
                    }
                    catch (TimeoutException e)
                    {
                        Console.WriteLine($"The exception throw when trying to get distance between {cityA} and {cityB} : {e}");
                        if (tries < 2)
                            throw;
                    }
                }
                CachedDistances[(cityA, cityB)] = distance;
                CachedDistances[(cityB, cityA)] = distance;

                return distance;
            }

            private static int AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for (i = 0; i < Math.Min(s.Length, t.Length); i++)
                {
                    result += Math.Abs(s[i] - t[i]);
                }
                for (; i < Math.Max(s.Length, t.Length); i++)
                {
                    result += s.Length > t.Length ? s[i] : t[i];
                }

                return result;
            }
        }
    }
}