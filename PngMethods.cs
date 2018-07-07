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
                //Byte patterns to match against.
                byte[] endOfCoin = new byte[] {125,93,125};
                byte[] cLDc = new byte[] {99, 76, 68, 99};
                byte[] iEnd = new byte[] {73,69,78,68};

                //Check for the possitions.
                List<int> startCoin = returnPos(pngBytes, cLDc, length); //marks first "cLDc" tag in file.
                List<int> endCoin = returnPos(pngBytes, endOfCoin, length);
                List<int> endFile = returnPos(pngBytes, iEnd, length);

                Console.WriteLine("startCoin: " + startCoin[0]);
                Console.WriteLine("endCoin: " + endCoin[0]);
                Console.WriteLine("endFile: " + endFile[0]);

                if(startCoin.Any())
                {
                    int startC = startCoin[0] + 4; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                    int endC = endCoin[0] + 3; //Marks the first instace of the end sequence "}]}" + 3 for the matching bytes.
                    int diff = (endC - startC);
                    byte[] coinData = new byte[diff]; //the difference between the numbers will be the length of the coin.
                    for(int i = 0; i<=coinData.Length-1; i++)
                    {
                        coinData[i] = pngBytes[startC + i];
                    }//end for
                    try//save the png.
                    {
                        using (var fs = new FileStream("./Printouts/"+pngName+"."+mod+".stack", FileMode.Create, FileAccess.Write))
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
            byte[] endOfCoin = new byte[] {125,93,125}; //Change when moving from stack.
            byte[] cLDc = new byte[] {99, 76, 68, 99};
            byte[] iEnd = new byte[] {73,69,78,68};

            //Check for the possitions. if startCoin exists there is a coin in the file.
            List<int> startCoin = returnPos(pngBytes, cLDc, pngLength); //marks first "cLDc" tag in file.
            List<int> endCoin = returnPos(pngBytes, endOfCoin, pngLength);

            if(startCoin.Any())
            {
                int startC = startCoin[0] - 4 ; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                int endC = endCoin[0] + 6; //Marks the first instace of the end sequence "}]}" + 3 for the matching bytes + 4 for crc.
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
        static public List<int> returnPos(byte[] png, byte[] seque, int length) 
        {   //Return the starting position for the chunk given by 'seque'.
            List<int> positions = new List<int>();

            string name = System.Text.Encoding.Default.GetString(seque);
            int chunkLength = seque.Length;

            byte firstMatchByte = seque[0];

            for (int i = 0; i < length; i++)
            {
                if (firstMatchByte == png[i] && length - i >= chunkLength)
                {
                    byte[] match = new byte[chunkLength];
                    Array.Copy(png, i, match, 0, chunkLength);
                    if (match.SequenceEqual<byte>(seque))
                    {
                        positions.Add(i);
                        i += chunkLength - 1;
                    }
                }
            }
            return positions;
        }// end returnPos() 
    }
}