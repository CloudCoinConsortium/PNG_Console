using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace AddToPng
{
    public class CoinClass
    {
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
        public void updateCoin(){
            setSn();
            setNn();
            setLength();
            setStrVal();
            setName();
            setTag();
            setIntVal();
            // Console.WriteLine("<CoinClass> ccPath Data is set now");
        }

 // Properties.
        byte[] snStart                     = new byte[] {34,115,110,34,58}; // " s n " :
        byte[] snEnd                       =  new byte[] {44,34,97,110,34}; // , " a n "
        byte[] nnStart                     = new byte[] {34,110,110,34,58}; // " n n " :  no end needed only 1 number

        private PngChunk pngChunk_; // the pngChunk of the coin itself.
        private string name_, path_, tag_, sn_, nn_, strVal_; 
        private int length_, intVal_; // the coins denomination. or the value of the stack.
        private byte[] ccData_;

        public PngChunk pngChunk{         get{    return pngChunk_;  }
                                    set{    pngChunk_ = value; }   
        }//end pngChunk
        public string name{         get{    return name_;  }
                                    set{    name_ = value; }   
        }//end name created from coins pngChunk.
        public string path{         get{    return path_;  }
                                    set{    path_ = value; }   
        }//end path to the destination folder.
        public string tag{          get{    return tag_;  }
                                    set{    tag_ = value; }   
        }//end tag added on to the coins file name.
        public string strVal{             get{    return strVal_;  }
                                    set{    strVal_ = value; }   
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
        public int intVal{              get{    return intVal_;  }
                                    set{    intVal_ = value; }   
        }//end length of coin file.
        public byte[] ccData{              get{    return ccData_;  }
                                    set{    ccData_ = value; }   
        }//end length of coin file.
   
// Mutators.
        void setData(byte[] cloudCoinBytes){ //Sets the pngChunk from the param.
            pngChunk = new PngChunk(cloudCoinBytes);
            ccData = cloudCoinBytes;
            // Console.WriteLine("Byte pngChunk: CoinClass" );
        }//end setData
        void setData(){ //Sets the pngChunk from the save location.
            pngChunk = new PngChunk(path);
            // Console.WriteLine("Path pngChunk: CoinClass" );
        }//end setData
        void setName(){
            name = strVal + ".CloudCoin." + nn + "." + sn + "." + tag + ".stack";
        }//end setName
        void setPath(string p){
            path = p;
        }//end setPath
        void setTag(){
           tag = "uniqueTag";
        }//end setTag 
        void setIntVal(){
           int tempVal = 0;
           Int32.TryParse(sn, out tempVal);
           intVal = tempVal;
        }//end setTag 
        void setStrVal(){
            strVal = "0";
            int intSn = 0; //Parse the serial number from string to int.
            Int32.TryParse(sn, out intSn);
            if(intSn >= 1 && intSn < 2097153)
                strVal = "1";
            else if(intSn >= 2097153 && intSn < 4194305)
                strVal = "5";
            else if(intSn >= 4194305 && intSn < 6291475)
                strVal = "25";
            else if(intSn >= 6291475 && intSn < 14680065)
                strVal = "100";
            else if(intSn >= 14680065 && intSn < 16777217)
                strVal = "250";
        }//end setVal
        void setSn(){
            List<int> snBegin = returnPos(pngChunk.chunk, snStart);
            List<int> snStop = returnPos(pngChunk.chunk, snEnd);
            int snLoc = snBegin[0] + snStart.Length;
            int snLength = snStop[0] - snLoc;
            sn = System.Text.Encoding.UTF8.GetString(pngChunk.chunk.Skip(snLoc).Take(snLength).ToArray());
        }//end setSn
        void setNn(){
            List<int> nnList = returnPos(pngChunk.chunk, nnStart);
            int nnLoc = nnList[0] + nnStart.Length;
            nn = System.Text.Encoding.UTF8.GetString(pngChunk.chunk.Skip(nnLoc).Take(1).ToArray());
        }//end setNn
        void setLength(){
            length = pngChunk.chunkLength;
        }//end setLength


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
