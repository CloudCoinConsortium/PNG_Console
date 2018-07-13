using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;

namespace AddToPng
{
    public class PngClass
    {
        Utils util = new Utils();
        KeyboardReader k = new KeyboardReader();
        //Class constructor
        public PngClass(){
            util = new Utils();
            string filepath = SelectNewPNG("./Images");
            setpath(filepath); //done
            updatePNG();
        }//End png class constructor.
        public PngClass(bool t){
            util = new Utils();
            string filepath = SelectNewPNG("./ImageBank");
            setpath(filepath); //done
            updatePNG();
        }//End png class constructor.
        public void updatePNG(){
            setname(); //done
            setPngByteName(); // possible wrong method.
            setdata(); //done
            setlength(); //done
            setHasCoins(); //done
            if(hasCoins){
                setListOfCoins(); //done
                setcount(); //done
                setStoredValue(); //done
            }
            if(hasStagedCoins)
                setStagedVal(); 
        }
        // Class memebers.

        private byte[] ccStart             = new byte[] {99, 76, 68, 99}; // c L D c
        private byte[] ccEnd               = new byte[] {123, 63, 36, 125}; // { ? $ }
        private byte[] iEnd                = new byte[] {73,69,78,68}; //IEND

            //PNG specific
        private string name_, workingname_, path_, destpath_, tag_;
        private int length_, count_, storedVal_, stagedVal_;
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
        public string destpath{                 get{    return destpath_; }
                                            set{    destpath_ = value; }   
            }//end path to png file
        public int length{                  get{      return length_; }
                                            set{    length_ = value; }
            }//end length of the resulting byte file
        public string name{                 get{    return name_; }
                                            set{    name_ = value; }   
            }//end name of the png file
        private string workingname{                 get{    return workingname_; }
                                            set{    workingname_ = value; }   
            }//end name of the png file
        public byte[] data{                 get{    return data_; }
                                            set{    data_ = value; }
            }//end data of png stored as byte file 
        public string tag{           get{    return tag_; }
                                            set{    tag_ = value; }
            }//end designator currently .png
        public int count{                   get{      return count_; }
                                            set{    count_ = value; }
            }//end count of CloudCoin cLDc chunks in the png
        public int storedVal{                   get{      return storedVal_; }
                                            set{    storedVal_ = value; }
            }//end total value of the png file
        public int stagedVal{                   get{      return stagedVal_; }
                                            set{    stagedVal_ = value; }
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
        private void setdestpath(){
            destpath =  path.Take(path.Length-1).ToArray().ToString();
            Console.WriteLine("Destpath: " + destpath);
        }//End setPngByteName()
        private void setlength(){
            length = data.Length;
        }//setlength()
        private void setname(){
            Console.WriteLine("Path: " + path);
            string[] spPath = path.Split('/');
            string[] spName = spPath[spPath.Length-1].Split('.');
            name = spName[spName.Length - 2];


        }//End setname()
        private void setworkingname(){
            workingname = "ImageBank/\u00A4"+stagedVal.ToString() + ".CloudCoin." + name + tag + ".png";
                if(hasStagedCoins)
                {
                    workingname = "ImageBank/\u00A4"+(stagedVal + storedVal).ToString() + ".CloudCoin." + name + tag + ".png";
                    Console.WriteLine("WN: " + workingname);
                }
        }//End setname()
        private void setPngByteName(){
            // pngByteName = System.IO.File.ReadAllBytes(name); // wrong method being used?
        }//End setPngByteName()
        private void setdata(){
            data = System.IO.File.ReadAllBytes(path);
        }//End setPngData()
        private void settag(){
            tag = "."+Utils.getUserInput(999, "Enter a number for the tag: ");
        }//End setdesignator()
        private void setcount(){ //Need more logic
            if(listOfCoins.Any())
                count = listOfCoins.Count;
        }
        private void setStoredValue(){ //Need more logic
            storedVal = 0;
            int temp = 0;
            foreach(CoinClass coin in listOfCoins){
                Int32.TryParse(coin.strVal, out temp);
                storedVal += temp;
            }
        }
        private void setStagedVal(){ //Need more logic
            stagedVal = 0;
            int temp = 0;
            foreach(CoinClass coin in listOfStagedCoins){
                Int32.TryParse(coin.strVal, out temp);
                stagedVal += temp;
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
        public string SelectNewPNG(string path)
        {
            string pngPath = "";
            try
            {
                string[] pngFilePaths = Directory.GetFiles(path, "*.png");
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
                                ccPath = ccPaths[choice - 1]; //Choose the cloudcoin to be added to the png.
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
                setStagedVal();
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
            //Separate the png into two files 
            //Upto and not including 'IEND chunk' -firstHalf
            //The cloudCoin data being inserted -coinBytes
            //Location, type, and crc of IEND) -secondHalf
            try//save the png.
                {
                    setworkingname();
                    string workname = workingname;
                    if(!File.Exists(workname)) 
                    {
                        System.IO.File.Copy(path, workname);
                        setpath(workname);
                    }  
                    
                    
                    updatePNG();
           

                    List<int> iendPosition = returnPos(data,iEnd);//Location of the chunk type 'iend'
                    int bitLocation = iendPosition[0] - 4; // Location of first byte.
                    int chunksLength = 0;

                    byte[] pngByteFile = data.Take(bitLocation).ToArray();
                    byte[] pngByteFileEnd = data.Skip(bitLocation).Take(12).ToArray();
  
                    foreach(CoinClass coin in listOfStagedCoins)
                    {
                        chunksLength += coin.pngChunk.chunkLength;//end foreach
                    }
                    byte[] chunkFile = new byte[chunksLength];
                    int n = 0;
                    foreach(CoinClass coin in listOfStagedCoins)
                    {
                        foreach(byte b in coin.pngChunk.chunk)
                        {
                            chunkFile[n] = b;
                            n++;
                        }
                    }
                    byte[] temp = new byte[chunksLength + data.Length];
                    
                    n = 0;
                    foreach(byte b in temp)
                    {
                        if(n < bitLocation){
                            temp[n]  =   pngByteFile[n];
                        }
                        if(n >= bitLocation && n < bitLocation + chunksLength ){
                            temp[n]  =   chunkFile[n-bitLocation];
                        }        
                        if(n > bitLocation + chunksLength){
                            temp[n]  =   pngByteFileEnd[n - (bitLocation + chunksLength)];
                        }
                            
                        n++; 
                    }
                        
//r.SelectMany(i => i).ToArray();
                    byte[] returnFile = temp;
                    Console.WriteLine("arrayLength: " + returnFile.Length);


                    write(returnFile, workname);
                    listOfStagedCoins.Clear();
                      
                }//end try
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught in process: {0}", ex);
                }//end catch
        }//end saveCoins()
        private void write(byte[] bytes, string path)
        {
            Console.WriteLine("save " +path);
            File.WriteAllBytes(path, bytes);
        }

        public void removeCoins()
        {
            updatePNG();

            if(hasCoins)
            {
                foreach(CoinClass coin in listOfCoins)
                {
                     try//save the coin.
                    {
                        write(coin.ccData, "./Printouts/"+coin.name);
                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch
                }//end if startCoinv
            }
            int[] takeTo = returnPos(data, ccStart).ToArray();
            int start = length;
            int[] stopAt = returnPos(data, iEnd).ToArray();
            int stop = 0;
            Console.WriteLine("len: " + length); 
            foreach(int i in takeTo)
            {
                Console.WriteLine("tt: " + i); 
                if(i < start){
                    start = i;
                }
                    
            }
            foreach(int i in stopAt)
            {
                Console.WriteLine("tt: " + i); 
                if(i > stop)
                    stop = i;
            }
            start -= 4;
            stop -= 4;
            int skip = stop - start;
             Console.WriteLine("Start: " + start);   
             Console.WriteLine("Skip: " + skip);   
             Console.WriteLine("Stop: " + stop);   
            byte[] arr = data.Take(start).ToArray();
            arr = arr.Concat(data.Skip(stop).Take(12).ToArray()).ToArray();

            // Console.WriteLine(Hex.Dump(arr.ToArray()));
            write(arr.ToArray(), "./ImageBank/"+name+".png");
        }//end remove coins
    }
}
