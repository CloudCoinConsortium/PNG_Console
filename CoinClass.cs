using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    public class CoinClass
    {

        byte[] snStart                     = new byte[] {34,115,110,34,58}; // " s n " :
        byte[] snEnd                       =  new byte[] {44,34,97,110,34}; // , " a n "
        byte[] nnStart                     = new byte[] {34,110,110,34,58}; // " n n " :  no end needed only 1 number

        private byte[] data_; // the data of the coin itself.
        private string name_, path_, tag_, sn_, nn_, val_; 
        private int length_; // the coins denomination. or the value of the stack.


 
 
 // Properties.
        public byte[] data{         get{    return data_;  }
                                    set{    data_ = value; }   
        }//end data
        public string name{         get{    return name_;  }
                                    set{    name_ = value; }   
        }//end name created from coins data.
        public string path{         get{    return path_;  }
                                    set{    path_ = value; }   
        }//end path to the destination folder.
        public string tag{          get{    return tag_;  }
                                    set{    tag_ = value; }   
        }//end tag added on to the coins file name.
        public string val{             get{    return val_;  }
                                    set{    val_ = value; }   
        }//end val the coins denomination. or the value of the stack.
        public string sn{              get{    return sn_;  }
                                    set{    sn_ = value; }   
        }//end sn Coins serial number
        public string nn{              get{    return nn_;  }
                                    set{    nn_ = value; }   
        }//end nn Coins network number
        public int length{              get{    return length_;  }
                                    set{    length_ = value; }   
        }//end length of coin file.
   
// Mutators.
        void setData(byte[] cc){ //Sets the data from the param.
            data = cc;
        }//end setData
        void setData(){ //Sets the data from the save location.
            data = System.IO.File.ReadAllBytes(path);
        }//end setData
        void setName(){
            name = val + ".CloudCoin." + nn + "." + sn + "." + tag + ".Stack";
        }//end setName
        void setPath(string p){
            path = p;
        }//end setPath
        void setTag(){
           tag = "uniqueTag";
        }//end setTag
        void setVal(){
            val = "0";
            int intSn = 0; //Parse the serial number from string to int.
            Int32.TryParse(sn, out intSn);
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
        }//end setVal
        void setSn(){
            List<int> snBegin = returnPos(data, snStart);
            List<int> snStop = returnPos(data, snEnd);
            int snLoc = snBegin[0] + snStart.Length;
            int snLength = snStop[0] - snLoc;
            sn = System.Text.Encoding.UTF8.GetString(data.Skip(snLoc).Take(snLength).ToArray());
        }//end setSn
        void setNn(){
            List<int> nnList = returnPos(data, nnStart);
            int nnLoc = nnList[0] + nnStart.Length;
            nn = System.Text.Encoding.UTF8.GetString(data.Skip(nnLoc).Take(1).ToArray());
        }//end setNn
        void setLength(){
            length = data.Length;
        }//end setLength
        void updateCoin(){
            setSn();
            setNn();
            setLength();
            setVal();
            setName();
            setTag();
        }


//Constructors
        public CoinClass(byte[] CloudCoin){
            setData(CloudCoin);
            updateCoin();
        }
        public CoinClass(string ccPath){
            setPath(ccPath);
            setData();
            updateCoin();
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
    }
}
