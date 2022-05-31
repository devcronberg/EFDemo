﻿using MCronberg.Sap.ConsoleOutput.Core;
using Microsoft.EntityFrameworkCore;

using TableParser;

namespace EFDemo
{
    internal class Program
    {
        static string pathToDb = @"c:\temp\people.db";
        static Writer writer = new Writer();

        static void Main(string[] args)
        {
            writer.BigHeader("EFDemo - view sql/ef log in efdemo.log", addNewline: true);

            #region Show
            ShowPeople(count: 10);
            ShowPeopleFilteredAndSorted(count: 10);
            ShowPeopleWithCountry(count: 10);
            ShowPeopleProjection(count: 10);
            #endregion

            #region Edit            
            EditPersonFirstName(1, "**");
            ShowPeople(count: 10);
            #endregion

            #region Insert
            var newId = InsertNewPerson("a", "b");
            ShowPeople();
            #endregion

            #region Delete
            DeleteLastPerson();
            ShowPeople();
            #endregion

        }

        private static void ShowPeopleProjection(int? count)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);            
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People
                    .OrderBy(i => i.PersonId)
                    .Select(i=>new PersonProjection { FirstName = i.FirstName, LastName = i.LastName, PersonId = i.PersonId })
                    .Take(count ?? 1000);
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName,
                    u => u.FullName
                );
                Console.WriteLine(table);
            }
        }

        private static void EditPersonFirstName(int personId, string firstName)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = c.People.FirstOrDefault(i=>i.PersonId == personId);
                person.FirstName = firstName;
                c.SaveChanges();
                Console.WriteLine($"Firstname on person with {personId} set to {firstName}");
                Console.WriteLine();
            }
        }

        private static int InsertNewPerson(string firstName, string lastName)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = new Person();
                person.FirstName = firstName;
                person.LastName = lastName;
                person.CountryId = 1;   // default
                c.People.Add(person);
                c.SaveChanges();
                Console.WriteLine($"New person added with id {person.PersonId}");
                Console.WriteLine();
                return person.PersonId;
            }
        }

        private static void DeleteLastPerson()
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = c.People.OrderBy(i=>i.PersonId).LastOrDefault();
                c.People.Remove(person);
                c.SaveChanges();
                Console.WriteLine($"Person with id {person.PersonId} removed");
                Console.WriteLine();
            }
        }

        private static void ShowPeopleFilteredAndSorted(int? count = null)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People?.Where(i => 
                    i.Height > 170 && 
                    i.IsHealthy)
                    .OrderBy(i => i.PersonId)
                    .Take(count??1000);                                              
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName
                );
                Console.WriteLine(table);
            }
        }

        private static void ShowPeople(int? count = null)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People
                    .OrderBy(i => i.PersonId)
                    .Take(count ?? 1000);
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName
                );
                Console.WriteLine(table);
            }
        }

        private static void ShowPeopleWithCountry(int? count = null)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People?.Include(i=>i.Country).Take(count??1000);                      
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName,
                    u => u.Country.CountryId,
                    u => u.Country.Name
                );
                Console.WriteLine(table);
            }
        }

        static void WriteHeader(string txt = "", int count = 50)
        {
            if (txt != "")
            {
                int l = txt.Length;
                string s = new String('-', (count - l - 2) / 2);
                string r = $"{s} {txt} {s}";
                if (r.Length < count)
                    r += "-";
                Console.WriteLine(r);
            }
            else
                Console.WriteLine(new String('-', count));
            Console.WriteLine();
        }

    }

    class PersonProjection {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get { 
                return FirstName + " " + LastName;
            }
        }
    }
}
