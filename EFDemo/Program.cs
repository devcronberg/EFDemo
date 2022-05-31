using MCronberg.Sap.ConsoleInput.Core;
using MCronberg.Sap.ConsoleOutput.Core;
using Microsoft.EntityFrameworkCore;

using TableParser;

namespace EFDemo
{
    internal class Program
    {
        static string pathToDb = @"c:\temp\people.db";
        static Writer writer = new Writer();
        static Reader reader = new Reader();

        static void Main(string[] args)
        {

            int menuId = 0;
            do
            {
                writer.BigHeader("Simple EF demo", addNewline: true);
                menuId = reader.Menu
                    ("ShowPeople",
                    "ShowPeopleFilteredAndSorted",
                    "ShowPeopleWithCountry",
                    "ShowPeopleProjection",
                    "EditPersonFirstName",
                    "InsertNewPerson",
                    "DeleteLastPerson",
                    "Exit"
                    );
                Console.Clear();
                switch (menuId)
                {
                    case 1:
                        ShowPeople(count: 10);
                        break;
                    case 2:
                        ShowPeopleFilteredAndSorted(count: 10);
                        break;
                    case 3:
                        ShowPeopleWithCountry(count: 10);
                        break;
                    case 4:
                        ShowPeopleProjection(count: 10);
                        break;
                    case 5:
                        EditPersonFirstName(1, "**");
                        ShowPeople(count: 10);
                        break;
                    case 6:
                        var newId = InsertNewPerson("a", "b");
                        ShowPeople(); break;
                    case 7:
                        DeleteLastPerson();
                        ShowPeople();
                        break;
                    case 8:
                        return;
                }
                reader.GetConsoleKey("Press any key");
                Console.Clear();

            } while (true);
        }

        private static void ShowPeopleProjection(int? count)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People
                    .OrderBy(i => i.PersonId)
                    .Select(i => new PersonProjection { FirstName = i.FirstName, LastName = i.LastName, PersonId = i.PersonId })
                    .Take(count ?? 1000);
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName,
                    u => u.FullName
                );
                writer.Write(table);
            }
        }

        private static void EditPersonFirstName(int personId, string firstName)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = c.People.FirstOrDefault(i => i.PersonId == personId);
                person.FirstName = firstName;
                c.SaveChanges();
                writer.Write($"Firstname on person with {personId} set to {firstName}");
                writer.NewLine();
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
                writer.Write($"New person added with id {person.PersonId}");
                writer.NewLine();
                return person.PersonId;
            }
        }

        private static void DeleteLastPerson()
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var person = c.People.OrderBy(i => i.PersonId).LastOrDefault();
                c.People.Remove(person);
                c.SaveChanges();
                writer.Write($"Person with id {person.PersonId} removed");
                
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
                    .Take(count ?? 1000);
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName
                );
                writer.Write(table);
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
                writer.Write(table);
            }
        }

        private static void ShowPeopleWithCountry(int? count = null)
        {
            var MethodName = System.Reflection.MethodBase.GetCurrentMethod().Name;
            writer.SimpleHeader(MethodName, addNewline: true);
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People?.Include(i => i.Country).Take(count ?? 1000);
                var table = res?.ToStringTable(
                    u => u.PersonId,
                    u => u.FirstName,
                    u => u.LastName,
                    u => u.Country.CountryId,
                    u => u.Country.Name
                );
                writer.Write(table);
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
                writer.Write(r);
            }
            else
                writer.Write(new String('-', count));
            
        }

    }

    class PersonProjection
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
    }
}
