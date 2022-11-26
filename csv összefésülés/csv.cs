using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
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
        public string csv1;
        public string csv2;
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
        public termék beolvas(string s)
        {
            
            string[] darab = s.Split(";");
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
            
            return olvas;

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

        public csvfésü()
        {
            csv1 = "C:\\csv\\procurement list.csv";
            while (!System.IO.File.Exists(csv1))
            {
                Console.WriteLine("Add meg az első lista pontos helyét!\nPl.:C:\\\\dokumentumok\\\\procurement list.csv");
                csv1 = Console.ReadLine()!;
            }
            csv2 = "C:\\csv\\procurement list (1).csv";
            while (!System.IO.File.Exists(csv2))
            {
                Console.WriteLine("Add meg az első lista pontos helyét!\nPl.:C:\\\\dokumentumok\\\\procurement list.csv");
                csv2 = Console.ReadLine()!;
            }
            StreamReader streamReader1 = new (csv1, Encoding.Latin1);
            StreamReader streamReader2 = new (csv2, Encoding.Latin1);
            List<termék> elso = new ();
            List<termék> masodik = new ();
            string? headerLine = streamReader2.ReadLine();
            streamReader1.ReadLine();
            /*CSV beolvasása listába*/
            while (!streamReader1.EndOfStream)
            {
                string? s1 = streamReader1.ReadLine();
                termék row1 = beolvas(s1!);
                elso.Add(row1);
            }
            streamReader1.Close();
            while (!streamReader2.EndOfStream)
            {
                string? s2 = streamReader2.ReadLine();
                termék row2 = beolvas(s2!);
                masodik.Add(row2);
            }
            streamReader2.Close();

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

            using (var writer = new StreamWriter("C:\\csv\\összegzett.csv", false, Encoding.Latin1))
            using (var csv = new CsvWriter(writer, CultureInfo.CurrentCulture))
            {
                csv.NextRecord();
                foreach (var record in össze)
                {
                    csv.WriteRecord(record);
                    csv.NextRecord();
                }
                writer.Flush();
            }
        }
    }
}
