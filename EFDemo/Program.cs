﻿using Microsoft.EntityFrameworkCore;

using TableParser;

namespace EFDemo
{
    internal class Program
    {
        static string pathToDb = @"c:\temp\people.db";
        static void Main(string[] args)
        {
            Console.WriteLine("EFDemo - view sql/ef log in efdemo.log");
            Console.WriteLine();

            ShowAllPeople();
            ShowPeopleFilteredAndSorted(count: 10);
            ShowPeopleWithCountry(count: 10);
            
            EditPersonFirstName(1, "**");
            ShowAllPeople(count: 10);

            var newId = InsertNewPerson("a", "b");
            
            DeletePerson(201);
            

        }

        private static void EditPersonFirstName(int personId, string firstName)
        {
            WriteHeader("EditPersonFirstName");
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
            WriteHeader("InsertNewPerson");
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

        private static void DeletePerson(int personId)
        {
            WriteHeader("DeletePerson");
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = c.People.FirstOrDefault(i => i.PersonId == personId);
                c.People.Remove(person);
                c.SaveChanges();
                Console.WriteLine($"Person with id {person.PersonId} removed");
                Console.WriteLine();
            }
        }

        private static void ShowPeopleFilteredAndSorted(int? count = null)
        {
            WriteHeader("ShowPeople");
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

        private static void ShowAllPeople(int? count = null)
        {
            WriteHeader("ShowAllPeople");
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
            WriteHeader("ShowPeopleWithCountry");
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

        static void WriteNewLine() {
            Console.WriteLine();
        }

    }
}
