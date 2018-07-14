using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    ///Class discriptions
    //      Program

    //      PngClass
    //          -Constructor
    //              -PngClass() , will prompt user to choose a png.
    //          -Accessors
    //              - string name
    //              - string path
    //              - string designator
    //              - int length
    //              - int count
    //              - int value
    //              - byte[] data
    //              - List <CoinClass> listOfCoins
    //              - List <CoinClass> listOfStagedCoins
    //              - bool hasCoins
    //              - bool hasStagedCoins
    //
    //
    //
    //      CoinClass
    //          -Constructors
    //              -CoinClass(string pathToCloudCoin)
    //              -CoinClass(byte[] cloudCoinsByteFile)
    //          -Accessors
    //              - PngChunk .pngChunk = PngChunk(cloudcoin) easy access to the needed pngChunk.
    //              - string .name = The name of this coin. ie 250.CloudCoin.1.11111111.tag.Stack
    //              - string .path = Coins that are not in a png will have a path.
    //              - string .tag  = Used for creating the Stack name.
    //              - string .val  = Value of this Coin. 
    //              - string .sn   = This coins Serial number.
    //              - string .nn   = This coins Network number.
    //              - int .length = The same as PngChunk.chunkLength 
    //
    //
    //      PngChunk
    //          -Constructors
    //              -PngChunk(string pathToCloudCoin)
    //              -PngChunk(byte[] cloudCoinsByteFile)
    //          -Accessor
    //              -byte[] .chunk = the cloudCoinData ready to be inserted as a png chunk.
    //              -int .chunkLength = numeric representation of the chunk size.
    //

    class Program
    {
        Encoding FileEncoding = Encoding.ASCII;
        private string[] status_;
        public string [] status {    get{ return status_; }
                                    set { status_ = value; }
                                }//end
        //Track activity
        static void Main(string[] args)
        {
            Program prog = new Program();
            prog.runProgram();
        }
        void runProgram()
        {
            
            Utils util = new Utils();
            PngClass png = new PngClass(true);
            bool makingChanges = true;
            while (makingChanges)
            {
                int choice = 0;
                try
                {
                    setStatus(png);
                    choice = Utils.printOptions(); //Display user choices.
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: " + e);
                }
                
                switch (choice)
                {
                    case 1://add new png.
                        png = new PngClass();
                        break;
                    case 2://select a png file.
                        png = new PngClass(true);
                        break;
                    case 3://select cloudcoins
                        png.stageCoins();
                        break;
                    case 4://insert cloudcoins to png ([byte[] data][string Names][int length])
                        if(png.hasStagedCoins)
                            png.SaveCoins();
                        png.updatePNG();
                        break;
                    case 5://retrieve cloudcoins from png
                        png.removeCoins();
                        break;
                    case 6://quit
                        makingChanges = false;//Select the png file.
                        break;
                }
                if(png.hasCoins){
                    
                }
            }
        }// end runProgram
        private void setStatus(PngClass png){ 

            //For saved coins.
            string sCount = "0";
            string sVal = "0";

            if(png.hasCoins)
            {
                List<CoinClass> coinList = png.listOfCoins;               
                string[] hasCoinsStatus = new string[coinList.Count()];
                int n = 0;
                foreach(CoinClass coin in coinList){
                    hasCoinsStatus[n] = "Name:       " + coin.name + "\r\n" + "Value:     " + coin.strVal;
                    n++;
                }//end foreach
            }

            //For staged coins.
            string[] updateStagedCoins;
            if(png.hasStagedCoins)
            {
                int stagedVal = 0;
                int i = 0;
                updateStagedCoins = new string[png.listOfStagedCoins.Count()];
                foreach(CoinClass coin in png.listOfStagedCoins){
                    updateStagedCoins[i] = coin.name + ": ";
                    int temp = 0;
                    Int32.TryParse(coin.strVal, out temp);
                    stagedVal += temp;
                    i++;
                }
                
                sVal = stagedVal.ToString();
                sCount = i.ToString();
                // Utils.consolePrintList(updateStagedCoins, false, "Staged coins: ", false);
                
                Console.WriteLine("                     -----                       ");

               
                
            }else{
                updateStagedCoins = new string[0];
            }

            status = new string[] {
                  "File:                    " + png.name +"."+png.tag,
                  "Coins found:             " + png.count, 
                  "Value of png:            " + png.storedVal,
                  "Coins staged:            " + sCount,
                  "Value of staged Coins:   " + sVal
                };  
            Utils.consolePrintList(status_, false, "Updates: ", false);
        }//end status()
    }
}
