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
            PngClass png = new PngClass();
            bool makingChanges = true;
            while (makingChanges)
            {
                setStatus(png);
                int choice = Utils.printOptions(); //Display user choices.
                switch (choice)
                {
                    case 1://select new png file.
                        png = new PngClass();
                        break;
                    case 2://select cloudcoins
                        png.stageCoins();
                        break;
                    case 3://insert cloudcoins to png ([byte[] data][string Names][int length])
                        if(png.hasStagedCoins)
                            png.SaveCoins();
                        break;
                    case 4://retrieve cloudcoins from png
                        png.removeCoins();
                        break;
                    case 5://quit
                        makingChanges = false;//Select the png file.
                        break;
                }
                if(png.hasCoins){
                    
                }
            }
        }// end runProgram
        private void setStatus(PngClass png){ 
            if(png.hasCoins)
            {
                List<CoinClass> coinList = png.listOfCoins;               
                status = new string[] {
                  "File: " + png.name,
                  "Coins found: " + png.count, 
                  "Value of png: " + png.storedVal
                };
                foreach(CoinClass coin in coinList){
                    Console.WriteLine("--");
                    Console.WriteLine(  "Name:       " + coin.name + "\r\n" +
                                        "Tag:       " + coin.tag + "\r\n" +
                                        "Sn:       " + coin.sn + "\r\n" +
                                        "Value:     " + coin.strVal + "              "
                    );
                    Console.WriteLine("--");
                }
            }
            if(png.hasStagedCoins)
            {
                string names = "Name:       ";
                int stagedVal = 0;
                string stagedStrVal = "";
                int i = 0;
                string[] updateStagedCoins = new string[png.listOfStagedCoins.Count()];
                foreach(CoinClass coin in png.listOfStagedCoins){
                    updateStagedCoins[i] = coin.name + ": Staged            ";
                    stagedVal += coin.intVal;
                    i++;
                }
                stagedStrVal = stagedVal.ToString();
                Utils.consolePrintList(updateStagedCoins, false, "Staged coins: ", false);
                Console.WriteLine("                     -----                       ");
                Console.WriteLine("--Staged value: " + png.stagedVal);
                Console.WriteLine("--Staged count: " + png.listOfStagedCoins.Count());
                
            }
            
            Utils.consolePrintList(status_, false, "Updates: ", false);
        }//end status()
    }
}
