using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    public class PngClass
    {
        // Class memebers.
        
        private byte[] ccStart             = new byte[] {99, 76, 68, 99}; // c L D c
        private byte[] ccEnd               = new byte[] {123, 63, 36, 125}; // { ? $ }

            //PNG specific
        private string name_, path_, designator_;
        private int length_, count_, value_;
        private byte[] data_;
        private List <CoinClass> listOfCoins_;
        private bool hasCoins_;

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
        public bool hasCoins{               get{      return hasCoins_; }
                                            set{    hasCoins_ = value; }
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
                Int32.TryParse(coin.val, out temp);
                value += temp;
            }
        }
        private void setListOfCoins(){
            
            int trac = 0;
            List<int> coinList = returnPos(data, ccStart); //marks first "cLDc" tag in file.
            List<int> endCoin = returnPos(data, ccEnd);//ending sequence for the CloudCoins
            List<CoinClass> coinlist = new List<CoinClass>();
            // List<CoinClass> coinlist = new List<CoinClass>();
            foreach(int coin in coinList)
                {
                    int startC = coin + 4; //Marks the first instance of cLDc in the png file + 4 to negate the cLDc type.
                    int endC = endCoin[trac]; //Marks the first instace of the end sequence.
                    int diff = (endC - startC);
                    byte[] coinData = data.Skip(startC).Take(diff).ToArray(); //the difference between the numbers will be the length of the coin.
                    trac++;
                    try//save the png.
                    {
                        CoinClass cc = new CoinClass(coinData);
                        coinlist.Add(cc);

                    }//end try
                    catch (Exception ex)
                    {
                        Console.WriteLine("Exception caught in process: {0}", ex);
                    }//end catch
                }//end if startcoin
                listOfCoins = coinlist;
        }//End setPngData()
        private void setHasCoins(){
            hasCoins = returnPos(data, ccStart).Any();
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

        //Class constructor
        public PngClass(string filepath){
            setpath(filepath); //done
            updatePNG();
        }//End png class constructor.

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
    }
}
