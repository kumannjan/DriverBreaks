using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace DriverBreaks
{
    class Program
    {
        static List<DriverBreak> ListOfBreaks = new List<DriverBreak>() { };
        static List<BreakTime> ListOfMinutes = new List<BreakTime>() { };

        // Shift minute and number of drivers on break on that particular minute
        // 
        public class BreakTime
        {
            public DateTime ShiftMinute { get; set; }
            public UInt32 DriversOnBreak { get; set; }
        }

        // Break start and end times
        public class DriverBreak
        {
            public DateTime BreakStartTime { get; set; }
            public DateTime BreakEndtime { get; set; }
        }

        static int Main(string[] args)
        {
            System.Console.WriteLine("DriverBreaks v1.00" + System.Environment.NewLine);

            if (args.Length >= 2 && args[0].ToLower() == "filename")
            {
                System.Console.WriteLine("Input file: " + args[1]);

                var BreaksLines = File.ReadLines(args[1]);
                foreach (var BreakTimeLine in BreaksLines)
                {
                    //System.Console.WriteLine(BreakTimeLine);
                    AddBreakToList(BreakTimeLine);
                }
            }

            DateTime CM = Convert.ToDateTime("00:00");  // CM - Current Minute, working name
            for (int i = 0; i < 24 * 60; i++)
            {
                ListOfMinutes.Add(new BreakTime { ShiftMinute = CM, DriversOnBreak = 0 });
                CM = CM.AddMinutes(1);
            }

            CM = Convert.ToDateTime("00:00");
            for (int i = 0; i < 24 * 60; i++)
            {
                foreach (var B in ListOfBreaks)
                {
                    if (CM >= B.BreakStartTime && CM <= B.BreakEndtime)
                    {
                        ListOfMinutes.ElementAt(i).DriversOnBreak++;
                    }
                }
                CM = CM.AddMinutes(1);
            }

            if (ListOfBreaks.Count > 0)
            {
                var bm = from element in ListOfMinutes orderby element.DriversOnBreak descending select element; // Now, sorted list 1st element has a peak number of drivers on break on particular time period
                int i = 0;
                var PeakDriversOnBreak = bm.ElementAt(i).DriversOnBreak;
                DateTime PeakBreakTimeStart = bm.ElementAt(i).ShiftMinute;
                DateTime PeakBreakTimeEnd = bm.ElementAt(i).ShiftMinute;
                while (bm.ElementAt(i).DriversOnBreak == PeakDriversOnBreak)
                {
                    PeakBreakTimeEnd = bm.ElementAt(i).ShiftMinute;
                    i++;
                }

                while (true)
                {
                    DisplayBreaks();
                    System.Console.Write(">");
                    string InputLine = System.Console.ReadLine();
                    if (InputLine.Length == 0)
                    {
                        break;
                    }

                    if (InputLine.Length == 11 && InputLine.Substring(2, 1) == ":" && InputLine.Substring(5, 1) == "-" && InputLine.Substring(8, 1) == ":")
                    {
                        try
                        {
                            var BreakStartTime = Convert.ToDateTime(InputLine.Substring(0, 5));
                            var BreakEndTime = Convert.ToDateTime(InputLine.Substring(6, 5));
                            if (BreakEndTime < BreakStartTime)
                            {
                                BreakEndTime=BreakEndTime.AddDays(1);
                            }
                            ListOfBreaks.Add(new DriverBreak { BreakStartTime = BreakStartTime, BreakEndtime = BreakEndTime });
                        }
                        catch
                        {
                            System.Console.WriteLine("!!! Invalid time...");
                        }
                    }
                    else
                    {
                        System.Console.WriteLine("!!! Invalid input, use <StartTime>-<EndTime> for example 10:00-10:30...");
                    }
                }
            }
            return 0;
        }

        static void AddBreakToList(string LineWithTime)
        {
            if (LineWithTime.Length == 11 && LineWithTime.Substring(2, 1) == ":" && LineWithTime.Substring(5, 1) == "-" && LineWithTime.Substring(8, 1) == ":")
            {
                try
                {
                    var BreakStartTime = Convert.ToDateTime(LineWithTime.Substring(0, 5));
                    var BreakEndTime = Convert.ToDateTime(LineWithTime.Substring(6, 5));
                    if (BreakEndTime < BreakStartTime)
                    {
                        BreakEndTime = BreakEndTime.AddDays(1);
                    }
                    ListOfBreaks.Add(new DriverBreak { BreakStartTime = BreakStartTime, BreakEndtime = BreakEndTime });
                }
                catch
                {
                    System.Console.WriteLine("!!! Invalid time...");
                }
            }
            else
            {
                System.Console.WriteLine("!!! Invalid input, use <StartTime>-<EndTime>, for example 06:15-11:30");
            }
        }
        static void DisplayBreaks()
        {
            if (ListOfBreaks.Count > 0)
            {
                foreach (var B in ListOfBreaks)
                {
                    System.Console.WriteLine(B.BreakStartTime.ToString("HH:mm") + "-" + B.BreakEndtime.ToString("HH:mm"));
                }

                var bm = from element in ListOfMinutes orderby element.DriversOnBreak descending select element; // Now, sorted list has a peak number of drivers on break in first element

                int i = 0;
                var PeakDriversOnBreak = bm.ElementAt(i).DriversOnBreak;
                System.Console.WriteLine("--- Total break times in the list = " + ListOfBreaks.Count);

                if (PeakDriversOnBreak > 0)
                {
                    DateTime PeakBreakTimeStart = bm.ElementAt(i).ShiftMinute;
                    DateTime PeakBreakTimeEnd = bm.ElementAt(i).ShiftMinute;
                    while (bm.ElementAt(i).DriversOnBreak == PeakDriversOnBreak)
                    {
                        PeakBreakTimeEnd = bm.ElementAt(i).ShiftMinute;
                        i++;
                    }
                    System.Console.WriteLine("Most drivers on break at: " + PeakBreakTimeStart.ToString("HH:mm") + "-" + PeakBreakTimeEnd.ToString("HH:mm") + ", " + PeakDriversOnBreak + " drivers" + Environment.NewLine);
                }
            }
        }
    }
}
