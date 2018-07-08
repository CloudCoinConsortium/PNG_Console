using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace AddToPng
{
    class PngMethod {
        Utils util = new Utils();
        
        public bool checkForCoin(byte[] png, int length){
            byte[] cLDc = new byte[] {99, 76, 68, 99};
            List<int> location = returnPos(png, cLDc, length);

            if(location.Any())
            { 
                Console.WriteLine(location[0]);
                return true;
            }
            return false;
        }
        public byte[] getFromPNG(byte[] pngBytes, int length, string pngName, int mod)
        {   //Returns a byte array containing one instance of cLDc.
                string coinName;
                //Byte patterns to match against.
                byte[] endOfCoin = new byte[] {123,63,36,125};
                byte[] cLDc = new byte[] {99, 76, 68, 99};
                byte[] iEnd = new byte[] {73,69,78,68};

                //Check for the possitions.
                List<int> startCoin = returnPos(pngBytes, cLDc, length); //marks first "cLDc" tag in file.
                List<int> endCoin = returnPos(pngBytes, endOfCoin, length);
                List<int> endFile = returnPos(pngBytes, iEnd, length);

                // Console.WriteLine("startCoin: " + startCoin[0]);
                // Console.WriteLine("endCoin: " + endCoin[0]);
                // Console.WriteLine("endFile: " + endFile[0]);

                if(startCoin.Any())
                {
                    int startC = startCoin[0] + 4; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                    int endC = endCoin[0]; //Marks the first instace of the end sequence "}]}" + 3 for the matching bytes.
                    int diff = (endC - startC);
                    byte[] coinData = new byte[diff]; //the difference between the numbers will be the length of the coin.
                    for(int i = 0; i<=coinData.Length-1; i++)
                    {
                        coinData[i] = pngBytes[startC + i];
                    }//end for
                    string file = System.Text.Encoding.UTF8.GetString(coinData);
                    Console.WriteLine("file: " + file);
                    try//save the png.
                    {
                        CoinClass cc = new CoinClass(coinData);
                        Console.WriteLine("");
                        Console.WriteLine("With data");
                        Console.WriteLine("");
                        Console.WriteLine("data: " + cc.data);
                        Console.WriteLine("name: " + cc.name);
                        Console.WriteLine("path: " + cc.path);
                        Console.WriteLine("tag: " + cc.tag);
                        Console.WriteLine("val: " + cc.val);
                        Console.WriteLine("sn: " + cc.sn);
                        Console.WriteLine("nn: " + cc.nn);
                        Console.WriteLine("length: " + cc.length);

                        coinName = returnCoinName(coinData);
                        using (var fs = new FileStream("./Printouts/"+coinName, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(coinData, 0, coinData.Length);
                        }
                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch
                    return coinData;
                }//end if startcoin
                else
                    return null;
            return null;//no coins in file.
        }//end getFromPNG();

        //inserts coins into the selected png file. Overwrites initial file.
        public byte[] SaveCoinsToPNG(byte[] png, byte[] cc, string pngName, string ccName, int pngLength, int ccLength)
        { 
            
            //Separate the png into two files 
            //Upto and not including 'IEND chunk' -firstHalf
            //The cloudCoin data being inserted -coinBytes
            //Location, type, and crc of IEND) -secondHalf

            //cloudcoin chunk
            PngChunk chunk = new PngChunk(); //Start creating a cLDc chunk to insert in png.
            byte[] coin = chunk.setChunk(cc); //return the cloud coin file as a png chunk.
            ccLength = coin.Length;
            //first section (header / image body)Int32.Parse(TextBoxD1.Text);
            byte[] iEnd = new byte[] {73,69,78,68};

            Console.WriteLine("L: "+pngLength);
            List<int> iendPosition = returnPos(png,iEnd, pngLength);//Location of the chunk type 'iend'
            int firstBitLocation = iendPosition[0] - 4; //iend possition minus 4 bytes for the length. also first byte of coinfile.

            //Concatonate the chunks.

            int coinChunkIndex = firstBitLocation;
            int endChunkIndex = firstBitLocation + ccLength;
            int totalLength = pngLength + ccLength;

            byte[] finalFile = new byte[totalLength];

            for(int i = 0; i<totalLength; i++)
            {
                if( i < coinChunkIndex)
                {
                    finalFile[i] = png[i];
                }//end if header
                if(i >= coinChunkIndex && i < endChunkIndex)
                {
                    finalFile[i] = coin[i-coinChunkIndex];
                }//end if cloudcoin
                if(i >= endChunkIndex)
                {
                    finalFile[i] = png[i-coin.Length];
                }//end if iend.
            }//end for

            try//save the png.
            {
                using (var fs = new FileStream("./png/"+pngName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(finalFile, 0, finalFile.Length);
                }
                return finalFile;
            }//end try
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return finalFile;
            }//end catch
        }//end saveCoinsToPng()


        //selects the cloudcoin file to be inserted in the png. 
        public List<string[]> getStack()
        {
            List<string[]> coinList = new List<string[]>();//allow for multiple coins.

            string[] myStack = new String[2];
            PngChunk chunk = new PngChunk();
            bool addCoins = true;

            try
            {
                string[] ccPaths = Directory.GetFiles("./Bank", "*.stack");
                string message = "Coins found in bank folder: "; //notify the user cloudcoins have been found.
                Utils.consolePrintList(ccPaths, true, message, true); //print out a list of the available coins.

                if(Utils.getEnter("Press enter to select all files. or any other key to continue.")){
                    foreach(string coin in ccPaths)
                    {
                        CoinClass cc = new CoinClass(coin);
                        Console.WriteLine("");
                        Console.WriteLine("with fp");
                        Console.WriteLine("");
                        Console.WriteLine("data: " + cc.data);
                        Console.WriteLine("name: " + cc.name);
                        Console.WriteLine("path: " + cc.path);
                        Console.WriteLine("tag: " + cc.tag);
                        Console.WriteLine("val: " + cc.val);
                        Console.WriteLine("sn: " + cc.sn);
                        Console.WriteLine("nn: " + cc.nn);
                        Console.WriteLine("length: " + cc.length);

                        myStack[0] = coin; //Choose the cloudcoin to be added to the png.
                        myStack[1] = System.IO.Path.GetFileName(coin);//save the stacks name.
                        coinList.Add(myStack);
                    }
                    return coinList;
                }

                string note = "Select the file you wish to use."; //Input prompt.
                while(addCoins)
                {
                    int choice = Utils.getUserInput(ccPaths.Length, note) - 1;
                    if (choice > -1)
                    {
                        myStack[0] = ccPaths[choice]; //Choose the cloudcoin to be added to the png.
                        myStack[1] = System.IO.Path.GetFileName(myStack[0]);//save the stacks name.
                    }//end if
                    else
                    {
                        myStack[0] = "null";
                        myStack[1] = "";
                    }
                    if(Utils.getEnter("Press enter to accept your choices, press space to add more coins.")){
                        addCoins = false;
                    }
                    coinList.Add(myStack);
                }//end while
                return coinList;
            }//end try
            catch (Exception e)
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                myStack[0] = e.ToString();
                return coinList;
            }//end catch
        }// end getStack();


        //Methods to select and store the png file.
        public string[] SelectNewPNG()
        {
            string message = "PNG files found: ";
            string note = "Select the file you wish to use.";
            //png[] => [1]Filepath [2]Name
            string[] pngReturnPack = new string[] {"no file path", "no name"};
            try
            {
                string[] pngFilePaths = Directory.GetFiles("./png", "*.png");
                Utils.consolePrintList(pngFilePaths, true, message, true);
                int selection = Utils.getUserInput(pngFilePaths.Length, note) - 1;
                if (selection > -1)
                {
                    pngReturnPack[0] = pngFilePaths[selection];//File path.
                    string[] png = pngReturnPack[0].Split('/'); //Split path.
                    pngReturnPack[1] = png[png.Length - 1];//Name.
                    return pngReturnPack;
                }
                else
                {
                    return pngReturnPack;
                }
            }
            catch (Exception e)
            {
                util.printError[0] = e.ToString();
                Utils.consolePrintList(util.printError, false, "ERROR: ", false);
                return util.printError;
            }
        }// end selectNewPNG()
        public byte[] deleteCoinFromPNG(byte[] pngBytes, int pngLength, string pngPath, string pngName)
        {   //Returns a byte array containing one instance of cLDc.
            PngMethod method = new PngMethod();

            //Byte patterns to match against.
            byte[] endOfCoin = new byte[] {123,63,36,125}; //end file marker.
            byte[] cLDc = new byte[] {99, 76, 68, 99};
            byte[] iEnd = new byte[] {73,69,78,68};

            //Check for the possitions. if startCoin exists there is a coin in the file.
            List<int> startCoin = returnPos(pngBytes, cLDc, pngLength); //marks first "cLDc" tag in file.
            List<int> endCoin = returnPos(pngBytes, endOfCoin, pngLength);

            if(startCoin.Any())
            {
                int startC = startCoin[0] - 4 ; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                int endC = endCoin[0] + 7; //Marks the first instace of the end sequence "}]}" + 3 for the matching bytes + 4 for crc.
                int diff = (endC - startC);
                int n = 0;
                byte[] newFile = new byte[pngBytes.Length - diff - 1]; //the size of the new file. 
                for(int i = 0; i < pngBytes.Length; i++)
                {
                    if(i<startC || i > endC){
                        newFile[n] = pngBytes[i];
                        n++;
                    }
                }//end for
                try//save the png.
                {
                    using (var fs = new FileStream(pngPath, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(newFile, 0, newFile.Length);
                    }
                    Console.WriteLine("A coin was removed from ");
                }//end try
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);
                }//end catch
                return newFile;
            }//end if startCoin
            else
                return null;
        }//end deleteCoinFromPNG();
        static public List<int> returnPos(byte[] subject, byte[] seque, int length) 
        {   //Return the starting position for the chunk given by 'seque'.
            List<int> positions = new List<int>();
            string name = System.Text.Encoding.Default.GetString(seque);
            int chunkLength = seque.Length;

            byte firstMatchByte = seque[0];

            for (int i = 0; i < length; i++)
            {
                if (firstMatchByte == subject[i] && length - i >= chunkLength)
                {
                    byte[] match = new byte[chunkLength];
                    Array.Copy(subject, i, match, 0, chunkLength);
                    if (match.SequenceEqual<byte>(seque))
                    {
                        positions.Add(i);
                        i += chunkLength - 1;
                    }
                }
            }
            return positions;
        }// end returnPos() 
        static public string returnCoinName(byte[] coin)
        {

            //Get the serial number from the coin.
            int length = coin.Length;
            //The byte patterns to search for. ,"an":
            byte[] snPattern = new byte[] {(byte)'"',(byte)'s',(byte)'n',(byte)'"',(byte)':'};
            byte[] snStopPattern= new byte[] {(byte)',',(byte)'"',(byte)'a',(byte)'n',(byte)'"'};
            byte[] nnPattern = new byte[] {(byte)'"',(byte)'n',(byte)'n',(byte)'"',(byte)':'};
            
            //Use the byte patterns to get the locations.
            List<int> snList = returnPos(coin, snPattern, length);
            int snLoc = snList[0] + snPattern.Length;

            List<int> snStopList = returnPos(coin, snStopPattern, length);
            int snLength = snLoc - snStopList[0];

            List<int> nnList = returnPos(coin, nnPattern, length);
            int nnLoc = nnList[0] + nnPattern.Length;

            //Start building names.
            string val =        "0"; // First piece of the name, will be 1,5,25,100,250
            string cc =         ".CloudCoin"; //Second piece of coins name.
            string nn =         "."+System.Text.Encoding.UTF8.GetString(coin.Skip(nnLoc).Take(1).ToArray())+".";
            string sn =         System.Text.Encoding.UTF8.GetString(coin.Skip(snLoc).Take(snLength).ToArray()); //The coins serial number. 3rd part.
            string userTag =    ".userTag";
            string fileDes =    ".png";

            //Convert the sn into an integer.
            int intSn = 0;
            Int32.TryParse(sn, out intSn);
            Console.WriteLine("string serial number: " + sn);
            Console.WriteLine("integer serial number: " + intSn);

            //Get the denomination of the coin.
            if(intSn >= 1 && intSn < 2097153)
                val = "1";
            else if(intSn >= 2097153 && intSn < 4194305)
                val = "5";
            else if(intSn >= 4194305 && intSn < 6291475)
                val = "25";
            else if(intSn >= 6291475 && intSn < 14680065)
                val = "100";
            else if(intSn >= 14680065 && intSn < 16777217)
                val = "250";

            string filename = val+cc+nn+sn+userTag+fileDes;
            Console.WriteLine("Stackname: " + filename);
            return filename;
        }
    }
}