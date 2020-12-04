using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Remoting.Contexts;
using System.Text;
using Numeral;

namespace Hex
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            
            // Pocetni HEX
            // Ovo mozes mijenjati sa podacima iz base
            string bigHex = "5cfa6c050f0000000036000000000000";
            
            //Pravimo objekat LortiotDecodeMessage koji ce sadrzavati obradjene podatke
            var loriotDecodedMessage = new LoriotDecodedMessage();


            // Petlja koja se okrece do duzine hexa i prolazi kroz svako slovo.
            // Posto je format slijedeci:
            // 5cfa6c05 --> TimeStamp           obrcemo for petlju do 8
            // 0f       --> CyclePeriod         obrcemo for petlju do 10
            // 0000     --> DailyEventsCoun     obrcemo for petlju do 14
            // 0000     --> CycleEventsCount    obrcemo for petlju do 18
            // 3600     --> DailyCyclesCount    obrcemo for petlju do 22
            // 0000000000 --> rezervisano ne raspakuje se po tabeli.
            for (int i = 1; i <= bigHex.Length; i++)
            {
                if (i == 8)
                {
                    // Spremamo rezultat metode u byte[]
                    // Koristim BitConverter.ToUInt32 da se dobije timestamp
                    // Onda timestamp pomocu DateTimeOffset pretvaram u datum 
                    byte[] timeStampResult = ConvertHexStringToByteArray(bigHex.Substring(0, 8));
                    loriotDecodedMessage.TimeStamp = BitConverter.ToUInt32(timeStampResult, 0) + 1514764800;
                    loriotDecodedMessage.ToDate = DateTimeOffset.FromUnixTimeSeconds(loriotDecodedMessage.TimeStamp);
                }
                else if (i == 10)
                {
                    // Spremamo rezultat metode u byte[]
                    byte[] cyclePeriod = ConvertHexStringToByteArray(bigHex.Substring(8, 2));
                    // Foreach petlja -> isto for petlja samo jednostavnija.
                    foreach (var b in cyclePeriod)
                    {
                        //Cast-am byte b u sbyte b i spremam u Objekat
                        loriotDecodedMessage.CyclePeriod = (sbyte) b;
                    }
                    //loriotDecodedMessage.CyclePeriod = (sbyte) BitConverter.ToChar(cyclePeriod, 0);
                }
                else if (i == 14)
                {
                    // Spremamo rezultat metode u byte[]
                    byte[] dailyEventCounts = ConvertHexStringToByteArray(bigHex.Substring(10, 4));
                    // Po tabeli ToUInt16 pretvaramo u brojac
                    loriotDecodedMessage.DailyEventsCounts = BitConverter.ToUInt16(dailyEventCounts, 0);
                }
                else if (i == 18)
                {
                    // Spremamo rezultat metode u byte[]
                    byte[] cycleEventCounts = ConvertHexStringToByteArray(bigHex.Substring(14, 4));
                    // Po tabeli ToUInt16 pretvaramo u brojac
                    loriotDecodedMessage.CycleEventsCounts = BitConverter.ToUInt16(cycleEventCounts, 0);
                }
                else if (i == 22)
                {
                    // Spremamo rezultat metode u byte[]
                    byte[] dailyCycleCount = ConvertHexStringToByteArray(bigHex.Substring(18, 4));
                    // Po tabeli ToUInt16 pretvarmo u brojac
                    loriotDecodedMessage.DailyCycleCounts = BitConverter.ToUInt16(dailyCycleCount, 0);
                }
            }

            // Ispis na konzoli
            Console.WriteLine($" Time Stamp: {loriotDecodedMessage.ToDate}, \n Cycle Period: {loriotDecodedMessage.CyclePeriod} \n Daily Events Count: {loriotDecodedMessage.DailyEventsCounts} \n Cycle Events Count: {loriotDecodedMessage.CycleEventsCounts} \n Daily Cycle Count: {loriotDecodedMessage.DailyCycleCounts}");
            Console.ReadKey();
        }
        

        // Metoda koja pretvara HexString u niz byte-ova (ByteArray -> byte[])
        public static byte[] ConvertHexStringToByteArray(string hexString)
        {
            // Provjera da li je HEX djeljiv sa 2
            // Ako jeste HEX je vazeci
            // Ako nije baci error jer hex mora biti paran uvijek
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            // Pretvaranje string HEX-a u byte data
            byte[] data = new byte[hexString.Length / 2];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring(index * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            // Metoda vraca byte[]
            return data;
        }
    }
    public class LoriotDecodedMessage
    {
        public UInt16 DailyCycleCounts { get; set; }
        public UInt16 CycleEventsCounts { get; set; }
        public UInt16 DailyEventsCounts { get; set; }
        public sbyte CyclePeriod { get; set; }
        public UInt32 TimeStamp { get; set; }
        public DateTimeOffset ToDate { get; set; }
    }
}