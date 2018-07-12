using System;
using Dexiom.QuickCrc32;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime;
using TagLib; //"taglib-sharp-netstandard2.0/2.1.0"

namespace AddToPng
{

    public class PngChunk {

        //Chunk layout
        //[length(4 bytes), type(4 bytes), data(), crc()]
        //  cLDc Chunk Data
			//
			// Byte 0-2     'CC CC CC' Marks start of the coin.
			// Byte 3       1 is internal use, 0 is transfer.
			// Byte 4-12    Plaintext name of coin.
			// Byte 13-14   Designates how the bytes in the 'AN' are used.
			// Byte 15      Network number
			// Byte 16-18   Serial number
			// Byte 19-31   Last POWN results.
			// Byte 32-431  Authentication Numbers.
			// 4 Bytes      Data end Marker
            // 4 Bytes      CRC data


        public PngChunk(string pngPath){
            // Console.WriteLine("get chunk using path: \r\n" + Encoding.Default.GetString(chunk));
            setPath(pngPath); //path to cloudcoin
            setData(); 
            tasks();
            //The first 4 bytes of the png cLDc chunk represent the length of the data and crc field in the chunk.
        }
        public PngChunk(byte[] pngBytes){
            // Console.WriteLine("get chunk using byte[] \r\n");
            setData(pngBytes); 
            tasks();
            //The first 4 bytes of the png cLDc chunk represent the length of the data and crc field in the chunk.
        }
        private void tasks(){
            setEnd();
            setLength();
            setType(); //4 byte chunk type
            setCrc();
            createChunk();
            setChunkLength();
        }

        string path_;
        int chunkLength_;
        int val_;
        byte[] end_; // 4 byte  data end designator. 
        byte[] data_;
        byte[] length_;    // length (4 bytes) representation of the chunks length
        byte[] type_;      //type (4 bytes)
        byte[] crc_;       //crc (4 bytes)
        byte[] chunk_;     //the completed data that gets inserted into the coin.



        private string path{             get{    return path_;       }
                                        set{    path_ = value;      }
                                }//end path to the cloudcoin.
        private byte[] end{             get{    return end_; }
                                        set{    end_ = value; }
                                }//end
        private byte[] data{            get{    return data_;  }
                                        set{    data_ = value; }
                                }//end

        private byte[] length{          get{    return length_;    } //4 byte representation of the chunks length
                                        set{    length_ = value; }
                                }//end
        private byte[] type{            get{    return type_;  }
                                        set{    type_ = value; }
                                }//end
        private byte[] crc{             get{    return crc_;   }
                                        set{    crc_ = value; }
                                }//end
        public byte[] chunk{           get{   return chunk_;  }
                                        set{    chunk_ = value;}
                                }//end chunk
        public int chunkLength{           get{   return chunkLength_;  }
                                        set{    chunkLength_ = value;}
                                }//end chunkLength
        public int val{           get{   return val_;  }
                                        set{    val_ = value;}
                                }//end val

    public void setPath(string p){
        path = p;
    }
    private void setEnd()
    {//returns a 4 byte sequence ?$?$ to mark the end of the data portion of the chunk.
        end = new byte[] {(byte)'{', (byte)'?', (byte)'$', (byte)'}'};
        // foreach(byte b in end)
            // Console.WriteLine("setEnd: " + b.ToString()); 
    }
    private void setData(){
        data = System.IO.File.ReadAllBytes(path);
        // Console.WriteLine("data: in pngChunk set using path"); 
     }
    private void setData(byte[] d){
        data =  d;
        // Console.WriteLine("data: in pngChunk set using bytes"); 
     }
    private void setLength(){
        //The first 4 bytes of the png cLDc chunk represent the length of the data and crc field in the chunk. 
        //get length of CloudCoin data and end marker. Used long for 4 bytes (size of png chunk length specifier.).
        //Convert length to binary.
        int initLength = data.Length + end.Length;
        byte[] len = BitConverter.GetBytes(initLength);
        length = len;
        // Console.WriteLine("length: " + Convert.ToString(length));
        // foreach(byte b in length)
            // Console.WriteLine("byteLength: " + b.ToString()); 
    }
    private void setType(){
        type = new byte[] {(byte)'c',(byte)'L',(byte)'D',(byte)'c'}; // 4 byte Chunk ID. cLDc
        // foreach(byte b in type)
            // Console.WriteLine("byteType: " + b.ToString()); 
    }
    private void setChunkLength(){
       chunkLength = chunk.Length;
    //    Console.WriteLine("chunkLength: " + chunkLength);
    }
    
    private void setCrc(){
        byte[] chunkInfo = new byte[data.Length + type.Length];
        for(int i = 0; i < chunkInfo.Length; i++)
        {
            if(i < type.Length)
                chunkInfo[i] = type[i]; //type "cLDc"
            else
                chunkInfo[i] = data[i-type.Length];
        }
        uint checksum = QuickCrc32.Compute(chunkInfo); //get the crc. 
        byte[] crcBytes = BitConverter.GetBytes(checksum);
        crc = crcBytes;
        // foreach(byte b in crc)
            // Console.WriteLine("byteLength: " + b.ToString()); 
    }
    private void createChunk(){

        int l1 = length.Length;
        int l2 = type.Length;
        int l3 = data.Length;
        int l4 = end.Length;
        int l5 = crc.Length;

        int totalLength = l1+l2+l3+l4+l5;
        byte[] ccChunk = new byte[totalLength];

        Buffer.BlockCopy(length, 0, ccChunk, 0, l1);
        Buffer.BlockCopy(type, 0, ccChunk, l1, l2);
        Buffer.BlockCopy(data, 0, ccChunk, l1+l2, l3);
        Buffer.BlockCopy(end, 0, ccChunk, l1+l2+l3, l4);
        Buffer.BlockCopy(crc, 0, ccChunk, l1+l2+l3+l4, l5);
        

        
        chunk = ccChunk;
        // Console.WriteLine("New Chunk: ");

    }

    }
}
     





        
