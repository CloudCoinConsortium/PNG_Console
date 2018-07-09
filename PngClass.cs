using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    public class PngClass
    {
        Utils util = new Utils();
        //Class constructor
        public PngClass(){
            util = new Utils();
            string filepath = SelectNewPNG();
            setpath(filepath); //done
            updatePNG();
        }//End png class constructor.

        // Class memebers.

        private byte[] ccStart             = new byte[] {99, 76, 68, 99}; // c L D c
        private byte[] ccEnd               = new byte[] {123, 63, 36, 125}; // { ? $ }
        private byte[] iEnd                = new byte[] {73,69,78,68}; //IEND

            //PNG specific
        private string name_, path_, designator_;
        private int length_, count_, value_;
        private byte[] data_;
        private List <CoinClass> listOfCoins_;
        private List <CoinClass> listOfStagedCoins_;
        private CoinClass coin_;
        private bool hasCoins_;
        private bool hasStagedCoins_;

        // Property.
        public string path{                 get{    return path_; }
                                            set{    path_ = value; }   
            }//end path to png file
        public int length{                  get{      return length_; }
                                            set{    length_ = value; }
            }//end length of the resulting byte file
        public string name{                 get{    return name_; }
                                            set{    name_ = value; }   
            }//end name of the png file
        public byte[] data{                 get{    return data_; }
                                            set{    data_ = value; }
            }//end data of png stored as byte file 
        public string designator{           get{    return designator_; }
                                            set{    designator_ = value; }
            }//end designator currently .png
        public int count{                   get{      return count_; }
                                            set{    count_ = value; }
            }//end count of CloudCoin cLDc chunks in the png
        public int value{                   get{      return value_; }
                                            set{    value_ = value; }
            }//end total value of the png file
        public List<CoinClass> listOfCoins{    get{      return listOfCoins_; }
                                            set{    listOfCoins_ = value; }
            }//end listOfCoins currently stored
        public List<CoinClass> listOfStagedCoins{    get{      return listOfStagedCoins_; }
                                            set{    listOfStagedCoins_ = value; }
            }//end listOfCoins currently stored
        public CoinClass coin{              get{      return coin_; }
                                            set{    coin_ = value; }
            }//end listOfCoins currently stored
        public bool hasCoins{               get{      return hasCoins_; }
                                            set{    hasCoins_ = value; }
            }//end listOfCoins true false value.
        public bool hasStagedCoins{         get{      return hasStagedCoins_; }
                                            set{    hasStagedCoins_ = value; }
            }//end listOfCoins true false value.
        //End Properties.


        // Instance Constructor.
        private void setpath(String fp){
            path = fp;
        }//End setPngByteName()
        private void setlength(){
            length = data.Length;
        }//setlength()
        private void setname(){
            string[] spPath= path.Split('/');
            name = spPath[spPath.Length-1];
        }//End setname()
        private void setPngByteName(){
            // pngByteName = System.IO.File.ReadAllBytes(name); // wrong method being used?
        }//End setPngByteName()
        private void setdata(){
            data = System.IO.File.ReadAllBytes(path);
        }//End setPngData()
        private void setdesignator(){
            designator = "*.png";
        }//End setdesignator()
        private void setcount(){ //Need more logic
            if(listOfCoins.Any())
                count = listOfCoins.Count;
        }
        private void setvalue(){ //Need more logic
            value = 0;
            int temp = 0;
            foreach(CoinClass coin in listOfCoins){
                Int32.TryParse(coin.strVal, out temp);
                value += temp;
            }
        }
        private void setListOfCoins()
        {
            List<int> coinList = returnPos(data, ccStart); //marks first "cLDc" tag in file.
            coinList.Sort();
            List<int> endCoin = returnPos(data, ccEnd);//ending sequence for the CloudCoins
            endCoin.Sort();
            List<CoinClass> coinlist = new List<CoinClass>();
            
            using(var startEnum = coinList.GetEnumerator())
            using(var endEnum = endCoin.GetEnumerator())
            {
            while(startEnum.MoveNext() && endEnum.MoveNext())
                {
                    var start = startEnum.Current;
                    var end = endEnum.Current;
                    int startC = start + 4; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                    int endC = end; //Marks the first instace of the end sequence.
                    int diff = (endC - startC);
                    byte[] coinData = data.Skip(startC).Take(diff).ToArray(); //the difference between the numbers will be the length of the coin.

                    try//save the png.
                    {
                        CoinClass cc = new CoinClass(coinData);
                        coinlist.Add(cc);

                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch
                
                }
            }
            listOfCoins = coinlist;
        }//End setListOfCoins()

        private void setHasCoins(){
            hasCoins = returnPos(data, ccStart).Any();
        }//End setPngData()
        private void setHasStagedCoins(){
            hasStagedCoins = listOfStagedCoins.Any();
        }//End setPngData()
        private void updatePNG(){
            setname(); //done
            setPngByteName(); // possible wrong method.
            setdata(); //done
            setlength(); //done
            setdesignator(); //done
            setHasCoins(); //done
            if(hasCoins){
                setListOfCoins(); //done
                setcount(); //done
                setvalue(); //done
            } 
        }


        //Local class specific methods
        static public List<int> returnPos(byte[] subject, byte[] matchThis) 
        {   //Return the starting position for the byte pattern given by 'matchThis'.
            List<int> positions = new List<int>();
            string name = System.Text.Encoding.Default.GetString(matchThis);

            int length = subject.Length;
            int chunkLength = matchThis.Length;
            byte firstMatchByte = matchThis[0];

            for (int i = 0; i < length; i++)
            {
                if (firstMatchByte == subject[i] && length - i >= chunkLength)
                {
                    byte[] match = new byte[chunkLength];
                    Array.Copy(subject, i, match, 0, chunkLength);
                    if (match.SequenceEqual<byte>(matchThis))
                    {
                        positions.Add(i);
                        i += chunkLength - 1;
                    }
                }
            }
            return positions;
        }// end returnPos()
                //Methods to select and store the png file.
        public string SelectNewPNG()
        {
            string pngPath = "";
            try
            {
                string[] pngFilePaths = Directory.GetFiles("./png", "*.png");
                Utils.consolePrintList(pngFilePaths, true, "PNG files found: ", true);
                int selection = Utils.getUserInput(pngFilePaths.Length, "Select the file you wish to use.") - 1;
                if (selection > -1)
                {
                    pngPath = pngFilePaths[selection];//File path.
                    return pngPath;
                }
                else
                {
                    return pngPath;
                }
            }
            catch (Exception e)
            {
                util.printError[0] = e.ToString();
                Utils.consolePrintList(util.printError, false, "error: ", false);
                return util.printError[0];
            }
        }// end selectNewPNG()
        public void stageCoins()
        {
            listOfStagedCoins = new List<CoinClass>();
            string ccPath = "";
            bool addCoins = true;
            try
            {
                string[] ccPaths = Directory.GetFiles("./Bank", "*.stack");
                Utils.consolePrintList(ccPaths, true, "Coins found in bank folder: ", true); //print out a list of the available coins.
                bool getAll = Utils.getEnter("Press enter to select all files. or any other key to continue.");
                if(getAll)
                {
                    foreach(string coinPath in ccPaths)
                    {
                        // Console.WriteLine(coinPath);
                        coin = new CoinClass(coinPath);
                        Console.WriteLine("stage Coins: ");
                        Hex.Dump(coin.pngChunk.chunk);
                        listOfStagedCoins.Add(coin);
                    }
                    addCoins = false;  
                    setHasStagedCoins(); 
                }//end if
                else
                {
                    while(addCoins)
                    {   
                        int choice = Utils.getUserInput(ccPaths.Length, "Select the file you wish to use. or enter 0(zero when done)");
                        Console.WriteLine("Choice at switch: " + choice);
                        switch(choice)
                        {
                            case 0:
                                addCoins = false;
                            break;
                            default:
                                ccPath = ccPaths[choice]; //Choose the cloudcoin to be added to the png.
                                coin = new CoinClass(ccPath);
                                listOfStagedCoins.Add(coin);
                                Console.WriteLine("staged: ");
                            break;
                        }
                    setHasStagedCoins();
                    }//end while
                }//end else
                


                Console.WriteLine("--");
                Console.WriteLine("staged coins? " + hasStagedCoins);
                foreach(CoinClass coin in listOfStagedCoins){
                    Console.WriteLine("--");
                    Console.WriteLine(  "Name:       " + coin.name + "\r\n" +
                                    "Tag:       " + coin.tag + "\r\n" +
                                    "Sn:       " + coin.sn + "\r\n" +
                                    "Value:     " + coin.strVal + "              "
                    );
                Console.WriteLine("-staged-");
                }
                return;
            }//end try
            catch (Exception e)
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
            }//end catch
        }// end getCoins();
        public void SaveCoins()
        { 
            IEnumerable<byte> bf;
            //Separate the png into two files 
            //Upto and not including 'IEND chunk' -firstHalf
            //The cloudCoin data being inserted -coinBytes
            //Location, type, and crc of IEND) -secondHalf

            foreach(CoinClass coin in listOfStagedCoins)
            {
                updatePNG();
                //Location of the chunk type 'iend'
                List<int> iendPosition = returnPos(data,iEnd);
                //iend possition minus 4 bytes for the length. also first byte of coinfile.

                int firstBitLocation = iendPosition[0] - 4; 
                byte[] coinData = coin.pngChunk.chunk; //the png chunk being inserted.
                byte[] firstPart = data.Take(firstBitLocation).ToArray();
                byte[] secondPart = data.Skip(firstBitLocation).Take(12).ToArray();
                bf = firstPart.Concat(coinData).Concat(secondPart);
                byte[] finalFile = bf.Take(bf.Count()).ToArray();
                Console.WriteLine("Saving: ");
                try//save the png.
                {
                    using (var fs = new FileStream("./png/"+name, FileMode.Create, FileAccess.Write))
                    {
                        fs.Write(finalFile, 0, finalFile.Length);
                    }
                  updatePNG();
                }//end try
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);
                }//end catch
            }//end foreach
        }//end saveCoins()
        public void removeCoins()
        {
            int modifier = 0;

            while(hasCoins)
            {
                //Check for the possitions. if startCoin exists there is a coin in the file.
                List<int> startCoin = returnPos(data, ccStart); //marks first "cLDc" tag in file.
                startCoin.Sort();
                List<int> endCoin = returnPos(data, ccEnd); // {?$}
                endCoin.Sort();
                
                if(startCoin.Any())
                {
                    int startC = startCoin[0] - 3; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                    int endC = endCoin[0] + 8; //Marks the first instace of the end sequence "}]}" + 3 for the matching bytes + 4 for crc.
                    int diff = (endC - startC);

                    int n = 0;
                    byte[] newFile = new byte[length - diff - 1]; //the size of the new file. 
                    byte[] thisCoin = data.Skip(startC+7).Take(diff-15).ToArray();
                    for(int i = 0; i < length; i++)
                    {
                        if(i<startC || i > endC)
                        {
                            newFile[n] = data[i];
                            n++;
                        }//end if
                    }//end for


                     try//save the coin.
                    {
                        CoinClass cc = new CoinClass(thisCoin);
                        using (var fs = new FileStream("./Printouts/"+cc.name, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(cc.ccData, 0, cc.ccData.Length);
                        }
                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch


                    try//save the png.
                    {
                        using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(newFile, 0, newFile.Length);
                        }
                        updatePNG();
                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch

                }//end if startCoin
            }//end while.
        }//end remove coins
    }
}
