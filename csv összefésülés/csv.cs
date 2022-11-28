using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.VisualBasic.FileIO;

namespace csv_összefésülés
{
    


    public class csvfésü
    {
        public string csv1 = "C:\\csv\\procurement_list.csv";
        public string csv2 = "C:\\csv\\procurement_list (1).csv";
        public struct termék
        {
            public string cikkszám { get; set; }
            public string név { get; set; }
            public int darab { get; set; }
            public int nettó { get; set; }
            public int bruttó { get; set; }
            public string gyártó { get; set; }
            public string az { get; set; }
        }
        
        public class termékMap : ClassMap<termék>
        {
            public termékMap()
            {
                Map(m => m.cikkszám).Index(0).Name("Cikkszám");
                Map(m => m.név).Index(1).Name("Név");
                Map(m => m.darab).Index(2).Name("Darabszám");
                Map(m => m.nettó).Index(3).Name("Nettó");
                Map(m => m.bruttó).Index(4).Name("Bruttó");
                Map(m => m.gyártó).Index(5).Name("Gyártó");
                Map(m => m.az).Index(6).Name("Rendelés azonosító(k)");
            }
        }
        public List<termék> beolvas(string csv)
        {
            List<termék> lista = new();
            StreamReader streamReader = new(csv, Encoding.Latin1);
            string? headerLine = streamReader.ReadLine();
            while (!streamReader.EndOfStream)
            {
                string? s1 = streamReader.ReadLine();
                string[] darab = s1.Split(";");
                for (int i = 0; i < darab.Length; i++)
                {
                    darab[i] = Regex.Replace(darab[i], "\"", String.Empty);
                }
                termék olvas = new()
                {
                    cikkszám = darab[0],
                    név = darab[1],
                    darab = int.Parse(darab[2]),
                    nettó = int.Parse(darab[3]),
                    bruttó = int.Parse(darab[4]),
                    gyártó = darab[5],
                    az = darab[6]
                };
                lista.Add(olvas);
            }
            streamReader.Close();
            return lista;
        }
        public string rendezettAz(string az)
        {
            var darab = az.Split(",");
            Array.Sort(darab);
            az = $"{darab[0]}, ";
            for (int i = 0; i < darab.Length - 2; i++)
            {
                az += $"{darab[i + 1]}, ";
            }
            az += $"{darab[darab.Length - 1]}";
            return az;
        }
        public csvfésü()
        {
            /*Fájlellenörzés, létezik-e a megadott helyen, megadott nevű fájl*/
            while (!System.IO.File.Exists(csv1))
            {
                Console.WriteLine("Az első fájl helye, vagy neve nem megfelelő!\n" +
                    "Pontos hely megadása:\nPl.:C:\\\\csv\\\\procurement list.csv");
                csv1 = Console.ReadLine()!;
            }
            while (!System.IO.File.Exists(csv2))
            {
                Console.WriteLine("A második fájl helye, vagy neve nem megfelelő!\n" +
                    "Pontos hely megadása:\nPl.:C:\\\\csv\\\\procurement list.csv");
                csv2 = Console.ReadLine()!;
            }
            /*CSV beolvasása listába*/
            List<termék> elso = beolvas(csv1);
            List<termék> masodik = beolvas(csv2);
            List<termék> össze = new ();
            bool lefutott = false;

            for (int i = 0; i < elso.Count; i++)
            {
                lefutott = false;
                for (int j = 0; j < masodik.Count; j++)
                {
                    if (elso[i].cikkszám == masodik[j].cikkszám)
                    {
                        termék jelen = new();
                        jelen = elso[i];
                        jelen.darab += masodik[j].darab;
                        jelen.az += $", {masodik[j].az}";
                        jelen.az = rendezettAz(jelen.az);
                        össze.Add(jelen);
                        masodik.RemoveAt(j);
                        j++;
                        lefutott = true;
                    }
                }
                if (lefutott == false)
                {
                    össze.Add(elso[i]);
                }
            }
            
            össze.AddRange(masodik);
            List<termék> sorted = össze.OrderBy(t => t.az.Substring(0,6)).ToList();

            using (var writer = new StreamWriter("C:\\csv\\összegzett.csv", false, Encoding.Latin1))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                csv.NextRecord();
                foreach (var record in sorted)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
                writer.Flush();
            }
        }
    }
}
