namespace csv_összefésülés
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string csv1 = "C:\\csv\\procurement_list.csv";
            string csv2 = "C:\\csv\\procurement_list (1).csv";
            /*Fájlellenörzés, létezik-e a megadott helyen, megadott nevű fájl*/
            while (!File.Exists(csv1))
            {
                Console.WriteLine("Az első fájl helye, vagy neve nem megfelelő!\n" +
                    "Pontos hely megadása:\nPl.:C:\\\\csv\\\\procurement list.csv");
                csv1 = Console.ReadLine()!;
            }
            while (!File.Exists(csv2))
            {
                Console.WriteLine("A második fájl helye, vagy neve nem megfelelő!\n" +
                    "Pontos hely megadása:\nPl.:C:\\\\csv\\\\procurement list (1).csv");
                csv2 = Console.ReadLine()!;
            }
            csvfésü asd = new(csv1, csv2);

        }
    }
}