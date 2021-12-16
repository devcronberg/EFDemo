using Microsoft.EntityFrameworkCore;

namespace EFDemo
{
    internal class Program
    {
        static string pathToDb = @"c:\temp\people.db";
        static void Main(string[] args)
        {
            ShowPeople();
            ShowPeopleWithCountry();
        }

        private static void ShowPeople()
        {
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People?.Where(i => 
                    i.Height > 170 && 
                    i.IsHealthy)
                    .OrderBy(i => i.LastName);
                WriteExpression(res);
                WriteSql(res);
                WriteSeparator("Result");
                res?.ToList().ForEach(i => Console.WriteLine(i));
            }
        }

        private static void ShowPeopleWithCountry(int count = 10)
        {
            using (PeopleContext c = new PeopleContext(pathToDb))
            {
                var res = c.People?.Include(i=>i.Country).Take(count);
                WriteExpression(res);
                WriteSql(res);
                WriteSeparator("Result");
                res?.ToList().ForEach(i =>
                    Console.WriteLine($"{i.PersonId} {i.FirstName} {i.LastName} {i?.Country?.Name}"));
            }
        }

        static void WriteSeparator(string txt = "", int count = 25)
        {
            if (txt != "")
            {
                int l = txt.Length;
                string s = new String('-', (count - l - 2) / 2);
                string r = $"{s} {txt} {s}";
                if (r.Length < 25)
                    r += "-";
                Console.WriteLine(r);
            }
            else
                Console.WriteLine(new String('-', count));
        }

        static void WriteNewLine() {
            Console.WriteLine();
        }
        static void WriteSql(IQueryable? q)
        {
            WriteSeparator("SQL");
            Console.WriteLine(q?.ToQueryString());
            WriteNewLine();
        }

        static void WriteExpression(IQueryable? q)
        {
            WriteSeparator("Expression");
            Console.WriteLine(q?.Expression.ToString().Replace("Microsoft.EntityFrameworkCore.Query.",""));
            WriteNewLine();
        }
    }
}
