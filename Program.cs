using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    class Program
    {
        Encoding FileEncoding = Encoding.ASCII;

        //Storage for png
        byte[] pngbyteFile;     
        int pngLength = 0; 
        string[] pngInfo = new string[2];// [0]FilePath [1]FileName => stores png file information.


        //Storage for CloudCoin
        byte[] coinBytes;
        int coinLength = 0;
        List <string[]> coinInfo = new List<string[]>(); //includes string[Fp, Fn] => [0]FilePath, [1]FileName => to store the CloudCoins before insertion. 

        //Track activity
        public string[] printUpdate = new string[] { "PNG staged: ", "Coins staged: ", "Status: ", "Additional info: none" };

        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.runProgram();
        }
        void runProgram()
        {
            PngMethod pngManip = new PngMethod();
            Utils util = new Utils();
            bool makingChanges = true;
            while (makingChanges)
            {
                int choice = Utils.printOptions(); //Display user choices.
                switch (choice)
                {
                    case 1://select new png file.
                        pngInfo = pngManip.SelectNewPNG();//Select the png file.
                        pngbyteFile = System.IO.File.ReadAllBytes(pngInfo[0]);
                        pngLength = pngbyteFile.Length;
                        Console.WriteLine("FL: "+pngLength);
                
                        util.printUpdate[0] = "PNG staged: " + pngInfo[1]; // add name to updates.
                        util.printUpdate[1] = "Coins staged: none";  // add name to updates.
                        util.printUpdate[2] = "Status: ";  // add name to updates.
                        util.printUpdate[3] = "Additional info: none";  // add name to updates.

                        if(pngManip.checkForCoin(pngbyteFile, pngLength))
                        {
                                util.printUpdate[3] = "Status: coins found"; // add name to updates.
                        }
                        else
                        {
                                util.printUpdate[3] = "Status: no coins found"; // add name to updates.
                        }

                        break;
                    case 2://select cloudcoins
                        coinInfo = pngManip.getStack();//Select the stack to insert.

                        util.printUpdate[1] = "Coins staged: " + coinInfo.Count; // add name to updates.
                        break;
                    case 3://insert cloudcoins to png ([byte[] data][string Names][int length])
                        foreach(string[] coin in coinInfo)
                        {
                            coinBytes = System.IO.File.ReadAllBytes(coin[0]);
                            coinLength = coinBytes.Length;
                            pngbyteFile = pngManip.SaveCoinsToPNG(pngbyteFile, coinBytes, pngInfo[1], coin[1], pngLength, coinLength);//Select the png file.
                            pngLength = pngbyteFile.Length;

                            coinBytes = null;
                            coinLength = 0;
                            util.printUpdate[2] += coin[1]+"\n"; // add name to updates.
                        }
                        util.printUpdate[1] = "Coins staged: none" ; // add name to updates.
                        break;
                    case 4://retrieve cloudcoins from png
                        int modifier = 0;
                        bool cLDcExists = pngManip.checkForCoin(pngbyteFile, pngLength);
                        while(cLDcExists)
                        {
                            pngbyteFile = System.IO.File.ReadAllBytes(pngInfo[0]);
                            pngLength = pngbyteFile.Length;

                            byte[] savedCoins = pngManip.getFromPNG(pngbyteFile, pngLength, pngInfo[1], modifier);//Select the png file.
                            util.printUpdate[2] = "Status: Saved to " + pngInfo[1]+".stack"; // add name to updates.

                            pngManip.deleteCoinFromPNG(pngbyteFile, pngLength, pngInfo[0], pngInfo[1]);
                            pngbyteFile = System.IO.File.ReadAllBytes(pngInfo[0]);
                            pngLength = pngbyteFile.Length;
                            modifier++;
                            cLDcExists = pngManip.checkForCoin(pngbyteFile, pngLength);
                            if(modifier > 2000)
                            {
                                cLDcExists = false;
                            }
                        }

                        util.printUpdate[3] = "Status: no coins found: "; // add name to updates.
                        break;
                    case 5://quit
                        makingChanges = false;//Select the png file.
                        break;
                }
                Utils.consolePrintList(util.printUpdate, false, "Updates: ", false);
            }
        }
    }
}
