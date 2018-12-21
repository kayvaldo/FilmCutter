using System.Collections.Generic;
using MediaToolkit;
using MediaToolkit.Model;
using MediaToolkit.Options;
using System;
using System.IO;
using System.Linq;


namespace FilmCutter
{
    public class Program
    {
        static void Main()
        {
            const string videofilename = @"E:\Matches\Nigeria\Nigeria vs Bulgaria\Nigeria vs Bulgaria (1998).m4v";
            const string periodsFilename = @"C:\Users\kayvaldo\Documents\Soccer\Kanu\Text\Nigeria\Kanu vs Bulgaria.txt";
            // GetTimeSpan("01:02:14");
            var periods = GetPeriodsFromFile(periodsFilename);

            foreach (var period in periods)
            {
                Console.WriteLine("Abt to cut period {0}", period);
                CutFile(videofilename, period, periods.ToList().IndexOf(period));
            }

            Console.WriteLine("Finished Cutting Periods");
            Console.ReadLine();

        }

        private static void CutFile(string filename, string period, int index)
        {
            try
            {
                //period: 00:12:23 - 01:45:45
                var inputFile = new MediaFile { Filename = filename };
                var outputname = filename.Split(new[] { ".mp4" }, StringSplitOptions.None)[0];
                var outputFile = new MediaFile { Filename = string.Format("{0}{1}{2}", outputname, "~" + (index + 8), ".mp4") };
                var t1 = period.Split('-')[0];
                Console.WriteLine("Time t1 : {0}", t1);

                var t2 = period.Split('-')[1];
                Console.WriteLine("Time t2 : {0}", t2);

                TimeSpan span1 = GetTimeSpan(t1.Trim());
                TimeSpan span2 = GetTimeSpan(t2.Trim());

                using (var engine = new Engine())
                {
                    engine.GetMetadata(inputFile);

                    var options = new ConversionOptions();

                    // This example will create a 25 second video, starting from the 
                    // 30th second of the original video.
                    //// First parameter requests the starting frame to cut the media from.
                    //// Second parameter requests how long to cut the video.
                    var span3 = span2 - span1;
                    options.CutMedia(span1, span3);
                    options.VideoSize = VideoSize.Hd720;
                    options.VideoAspectRatio = VideoAspectRatio.R16_9;
                    engine.CustomCommand("-deinterlace"); 
                    Console.WriteLine("Cutting and converting period {0}", period);
                    engine.Convert(inputFile, outputFile, options);
                    Console.WriteLine("Done cutting period {0}", period);

                }
            }
            catch (IndexOutOfRangeException ind)
            {
                Console.WriteLine(ind.Message + ":" + ind.StackTrace);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + ":" + e.StackTrace);
            }
        }

        private static TimeSpan GetTimeSpan(string t1)
        {
            var x = t1.Split(':');
            var s = x.Reverse();
            var enumerable = s as IList<string> ?? s.ToList();
            string seconds = enumerable.ElementAtOrDefault(0);
            int sec = GetNumber(seconds);
            string minutes = enumerable.ElementAtOrDefault(1);
            int min = GetNumber(minutes);
            string hour = enumerable.ElementAtOrDefault(2);
            int hr = GetNumber(hour);


            var time = new TimeSpan(hr, min, sec);
            return time;
        }

        private static int GetNumber(string seconds)
        {
            int sec = 0;
            if (!string.IsNullOrEmpty(seconds))
            {
                try
                {
                    sec = Convert.ToInt32(seconds);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + '-' + ex.StackTrace);
                }
            }

            return sec;
        }

        private static string[] GetPeriodsFromFile(string filename)
        {
            //00:12:34 - 01:23:13
            var reader = new StreamReader(filename);
            var fileContent = reader.ReadToEnd();
            Console.WriteLine("File content: {0}", fileContent);
            var periods = fileContent.Split(';');

            foreach (var item in periods)
            {
                Console.WriteLine("Period: {0}", item);
            }

            return periods;
        }
    }
}
