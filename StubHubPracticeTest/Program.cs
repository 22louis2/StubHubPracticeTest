using System;
using System.Collections.Generic;
using System.Linq;

/*
 
Let's say we're running a small entertainment business as a start-up. This means we're selling tickets to live events on a website. An email campaign service is what we are going to make here. We're building a marketing engine that will send notifications (emails, text messages) directly to the client and we'll add more features as we go.
 
Please, instead of debuging with breakpoints, debug with "Console.Writeline();" for each task because the Interview will be in Coderpad and in that platform you cant do Breakpoints.
 
*/

namespace TicketsConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*

          1. You can see here a list of events, a customer object. Try to understand the code, make it compile. 

          2. The goal is to create a MarketingEngine class sending all events through the constructor as parameter and make it print the events that are happening in the same city as the customer. To do that, inside this class, create a SendCustomerNotifications method which will receive a customer as parameter and an Event parameter and will mock the the Notification Service API. DON’T MODIFY THIS METHOD, unless you want to add the price to the console.writeline for task 7. Add this ConsoleWriteLine inside the Method to mock the service. Inside this method you can add the code you need to run this task correctly but you cant modify the console writeline: Console.WriteLine($"{customer.Name} from {customer.City} event {e.Name} at {e.Date}");

          3. As part of a new campaign, we need to be able to let customers know about events that are coming up close to their next birthday. You can make a guess and add it to the MarketingEngine class if you want to. So we still want to keep how things work now, which is that we email customers about events in their city or the event closest to next customer's birthday, and then we email them again at some point during the year. The current customer, his birthday is on may. So it's already in the past. So we want to find the next one, which is 23. How would you like the code to be built? We don't just want functionality; we want more than that. We want to know how you plan to make that work. Please code it.

          4. The next requirement is to extend the solution to be able to send notifications for the five closest events to the customer. The interviewer here can paste a method to help you, or ask you to search it. We will attach a way to calculate the distance.

            public record City(string Name, int X, int Y);
            |public static readonly IDictionary<string, City> Cities = new Dictionary<string, City>()
                  {
                      { "New York", new City("New York", 3572, 1455) },
                      { "Los Angeles", new City("Los Angeles", 462, 975) },
                      { "San Francisco", new City("San Francisco", 183, 1233) },
                      { "Boston", new City("Boston", 3778, 1566) },
                      { "Chicago", new City("Chicago", 2608, 1525) },
                      { "Washington", new City("Washington", 3358, 1320) },
                  };
            var customerCityInfo = Cities.Where(c => c.Key == city).Single().Value;
            var distance = Math.Abs(customerCityInfo.X - eventCityInfo.X) + Math.Abs(customerCityInfo.Y - eventCityInfo.Y);

          5. If the calculation of the distances is an API call which could fail or is too expensive, how will you improve the code written in 4? Think in caching the data which could be code it as a dictionary. You need to store the distances between two cities. Example:

          New York - Boston => 400 
          Boston - Washington => 540

          Boston - New York => Should not exist because "New York - Boston" is already stored and the distance is the same. 
 
            6. If the calculation of the distances is an API call which could fail, what can be done to avoid the failure? Think in HTTPResponse Answers: Timeoute, 404, 403. How can you handle that exceptions? Code it.
 
            7.  If we also want to sort the resulting events by other fields like price, etc. to determine whichones to send to the customer, how would you implement it? Code it.
            */

            var events = new List<Event>{
                new Event(1, "Phantom of the Opera", "New York", new DateTime(2023,12,23)),
                new Event(2, "Metallica", "Los Angeles", new DateTime(2023,12,02)),
                new Event(3, "Metallica", "New York", new DateTime(2023,12,06)),
                new Event(4, "Metallica", "Boston", new DateTime(2023,10,23)),
                new Event(5, "LadyGaGa", "New York", new DateTime(2023,09,20)),
                new Event(6, "LadyGaGa", "Boston", new DateTime(2023,08,01)),
                new Event(7, "LadyGaGa", "Chicago", new DateTime(2023,07,04)),
                new Event(8, "LadyGaGa", "San Francisco", new DateTime(2023,07,07)),
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

            var marketingEngine = new MarketingEngine(events);

            /* 
             * QUESTION 1. find out all events that are in cities of customer
             * then add to email.
             * we want you to send an email to this customer with all events in their city
             * Just call AddToEmail(customer, event) for each event you think they should get
             */
            events.Where(x => x.City.Contains(customer.City))?.ToList()?.ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));

            /*
             *QUESTION 2. closest event to birthday 1st of August, 1st of Sept
             */
            events.OrderBy(x => Math.Abs(DateTime.Compare(customer.BirthDate, x.Date))).Take(5)?
                .ToList()?.ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));

            /*
             * QUESTION 3. find out the top 5 closest cities to the customer
             * and send an email to the customer(s)
             */
            events.OrderBy(x => marketingEngine.GetDistance(customer.City, x.City))?.Take(5)?.ToList()?
                .ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));

            /*
             * QUESTION 4. to solve the problem of the GetDistance method, if it is an API call and could fail or is too expensive
             * how would you improve the code in question 2. Best possible way is to cache the closest cities result and first of all, 
             * search if the data, exists in the cached storage, before making the request to the GetDistance method
             * then send the email, to the customer(s)
             */
            events.OrderBy(x => marketingEngine.GetDistancePreCall(customer.City, x.City))?.Take(5)?.ToList()?
                .ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));

            /*
             * QUESTION 5. to solve the problem of the GetDistance method, failing, and for which we don't want the process to fail.
             * we can cache the data in a store, by using the from and to city as key, then before making a request to the 
             * GetDistance method, search if it exists as a key in the cache store.
             */
            events.OrderBy(x => marketingEngine.GetDistancePreCallNotToFail(customer.City, x.City))?.Take(5)?.ToList()?
                .ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));

            // 2nd option
            events.Where(x => marketingEngine.GetDistancePreCallNotToFail(customer.City, x.City).Item1 == true)
                .Select(x => x)
                .OrderBy(x => marketingEngine.GetDistancePreCallNotToFail(customer.City, x.City).Item2)
                .Take(5)?.ToList()?.ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));
            /*   
             * QUESTION 5. Tackling this in respect to sorting, I would order it by price, which would help determine
             * the ones to be sent to the customer, from my cached closest city event list
             */
            events.OrderBy(x => marketingEngine.GetDistancePreCallNotToFail(customer.City, x.City))?.ThenBy(x => marketingEngine.GetPrice(x))?
                .Take(5)?.ToList()?.ForEach(x => marketingEngine.SendCustomerNotifications(customer, x));
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

        public class MarketingEngine
        {
            static Dictionary<string, int> _cachedDistanceStore = new Dictionary<string, int>();
            public List<Event> _events { get; set; }
            public MarketingEngine(List<Event> events)
            {
                _events = events;
            }
            // You do not need to know how these methods work
            public void SendCustomerNotifications(Customer c, Event e, int? price = null)
            {
                var distance = GetDistance(c.City, e.City);
                Console.Out.WriteLine($"{c.Name}: {e.Name} in {e.City}"
                + (distance > 0 ? $" ({distance} miles away)" : "")
                + (price.HasValue ? $" for ${price}" : ""));
            }

            public int GetDistancePreCallNotToFailBySorting(Tuple<bool, int> tuple)
            {
                return tuple.Item1 == true ? tuple.Item2 : 0;
            }

            public (bool, int) GetDistancePreCallNotToFail(string fromCity, string toCity)
            {
                try
                {
                    string queryKey = fromCity + toCity;
                    if (fromCity == toCity)
                        return (true, 0);
                    if (_cachedDistanceStore.ContainsKey(queryKey))
                        return (true, _cachedDistanceStore[queryKey]);
                    var distance = GetDistance(fromCity, toCity);
                    _cachedDistanceStore.Add(queryKey, distance);
                    return (true, distance);
                }
                catch (Exception)
                {
                    return (false, 0);
                }
            }
            public int GetDistancePreCall(string fromCity, string toCity)
            {
                string queryKey = fromCity + toCity;
                if (fromCity == toCity)
                    return 0;
                if (_cachedDistanceStore.ContainsKey(queryKey))
                    return _cachedDistanceStore[queryKey]; //.FirstOrDefault(x => x.Key == queryKey).Value;
                var distance = GetDistance(fromCity, toCity);
                _cachedDistanceStore.Add(queryKey, distance);
                return distance;
            }

            public int GetPrice(Event e)
            {
                return (AlphebiticalDistance(e.City, "") + AlphebiticalDistance(e.Name, "")) / 10;
            }
            public int GetDistance(string fromCity, string toCity)
            {
                return AlphebiticalDistance(fromCity, toCity);
            }
            private static int AlphebiticalDistance(string s, string t)
            {
                var result = 0;
                var i = 0;
                for (i = 0; i < Math.Min(s.Length, t.Length); i++)
                {
                    // Console.Out.WriteLine($"loop 1 i={i} {s.Length} {t.Length}");
                    result += Math.Abs(s[i] - t[i]);
                }
                for (; i < Math.Max(s.Length, t.Length); i++)
                {
                    // Console.Out.WriteLine($"loop 2 i={i} {s.Length} {t.Length}");
                    result += s.Length > t.Length ? s[i] : t[i];
                }
                return result;
            }
        }

        /*-------------------------------------
        Coordinates are roughly to scale with miles in the USA
        2000 +----------------------+
        | |
        | |
        Y | |
        | |
        | |
        | |
        | |
        0 +----------------------+
        0 X 4000
        ---------------------------------------*/

    }
}