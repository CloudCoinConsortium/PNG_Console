using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime;
using TagLib;

namespace AddToPng
{
    class Program
    {
        public static KeyboardReader reader = new KeyboardReader();
        public string[] printError = new string[1];
        public string[] printUpdate = new string[] {"Image selected: ","Coins staged: ","Coins Saved: ","Coins Inserted: "};
        static void Main(string[] args){
            Program prog = new Program();
            prog.runProgram();
        }
        
        void runProgram(){
            bool makingChanges = true;
            string pngPath = ""; //to png file.
            string savedCoins = "";//coins currently in the png.
            string[] ccData = new string[3]; //to store the CloudCoins before insertion.
            
                while(makingChanges){
                    int choice = printOptions();
                    switch(choice){
                        case 1:
                            pngPath = SelectNewPNG();//Select the png file.
                            printUpdate[0] =  "Image selected: " + pngPath;
                            printUpdate[1] =  "Coins staged: ";
                            printUpdate[2] =  "Coins Saved: ";
                            printUpdate[3] =  "Coins Inserted: ";
                            CheckForCoins(pngPath, 1);
                        break;
                        case 2:
                            ccData = getStack();//Select the stack to insert.
                            printUpdate[1] = "Coins staged: " +  ccData[2];
                        break;
                        case 3:
                            try{
                                SaveCoinsToPNG(pngPath, ccData);//Select the png file.
                                printUpdate[2] = "Coins Saved: " + ccData[2];
                                CheckForCoins(pngPath, 2);
                            }
                           catch(Exception e)
                           {
                               printError[0] = e.ToString();
                               consolePrintList(printError, false, "ERROR: ", false);
                           }
                        break;
                        case 4:
                            try
                            {
                                savedCoins = getFromPNG(pngPath);//Select the png file.

                            }
                            catch(Exception e)
                            {
                               printError[0] = e.ToString();
                               consolePrintList(printError, false, "ERROR: ", false);
                               break;
                            }
                            string[] emptyCoins = ccData;
                            emptyCoins[1] = "No File";
                            emptyCoins[2] = "Empty";
                            SaveCoinsToPNG(pngPath, emptyCoins);
                            printUpdate[3] = "Coins Removed: " + ccData[2] + ", and saved to ./Printouts.";
                        break;
                        case 5:
                            makingChanges = false;//Select the png file.
                        break;
                  }
                  consolePrintList(printUpdate, false, "Updates: ", false);
              }

        }
        public string getFromPNG(string pngPath){
            string message = "Keys found in " + pngPath + ":";
            TagLib.Png.File pngFile = (TagLib.Png.File)TagLib.Png.File.Create(pngPath);
            TagLib.Png.PngTag tag = (TagLib.Png.PngTag)pngFile.GetTag(TagLib.TagTypes.Png, true);

            string[] tags = new string[2];
            tags[0] = tag.GetKeyword("CCName");
            tags[1] = tag.GetKeyword("CCValue");
            consolePrintList(tags, false, message, false); 

            if (tags[0] != null || tags[1] != null) 
            {
                      string filename = tags[0].ToString();
                      message = "Press enter to extract the Cloudcoin stack from "+ filename + ".";

                      if(getEnter(message)){
                          Console.Out.WriteLine("Stack: " + filename + " has been found");
                          string CloudCoinAreaValues = tags[1].ToString();
                          string path ="./Printouts/"+ filename;
                          try
                          {
                              System.IO.File.WriteAllText(path, CloudCoinAreaValues); //Create a document containing Mp3 ByteFile (debugging).
                          }
                          catch(Exception e)
                          {
                              Console.Out.WriteLine("Failed to save CloudCoin data {0}", e);
                          }
                          printUpdate[3] =  "Coins retrieved: " + filename;
                          return path;
                      }
                      return "null";
            }//end if 
            else
            {
                  Console.Out.WriteLine("no stack in file" + tags[0]);
                  return "no .stack in file";
            }//end else
        }
        public void CheckForCoins(string path, int update){
            string message = "Keys found in " + path + ":";
            TagLib.Png.File pngFile = (TagLib.Png.File)TagLib.Png.File.Create(path);
            TagLib.Png.PngTag tag = (TagLib.Png.PngTag)pngFile.GetTag(TagLib.TagTypes.Png, true);
            string[] tags = new string[2];
            tags[0] = tag.GetKeyword("CCName");
            tags[1] = tag.GetKeyword("CCValue");
            string coinName = tags[0];
            printUpdate[3] =  "Coins Inserted: " + coinName;
            consolePrintList(tags, false, message, false); 
        }

        public void SaveCoinsToPNG(string png, string[] ccValue){ // ccValue[path, data, name]
            string[] cc = new string[] {"   ", "    ", "    ", "    "} ; // CloudCoin[0] & CloudCoin[3] for buffer.
            cc[2] = ccValue[2];// name
            cc[1] = ccValue[1];// value
            TagLib.File image = TagLib.File.Create(png);//Selected png to taglib image object.
            TagLib.Png.PngTag tag = (TagLib.Png.PngTag)image.GetTag(TagLib.TagTypes.Png, true); //Target the pngFiles meta data.
            Console.WriteLine("pngFile: " + image);
            Console.WriteLine("pngTag: " + tag);
            // SetKeyword(keyword, value);
            tag.SetKeyword("CCName",cc[2]);
            tag.SetKeyword("CCValue",cc[1]);
            image.Save();
        }

        public string SelectNewPNG(){
            string message = "PNG files found: ";
            string note = "Select the file you wish to use.";
            try 
            {
                string[] pngFilePaths = Directory.GetFiles("./png", "*.png");
                consolePrintList(pngFilePaths, true, message, true);
                int selection = getUserInput(pngFilePaths.Length, note) - 1;
                if(selection > -1){
                    string choice = pngFilePaths[selection];
                    return choice;
                }
                else
                {
                    return "null";
                }
            } 
            catch (Exception e) 
            {
                printError[0] = e.ToString();
                consolePrintList(printError, false, "ERROR: ", false);
                return printError[0];
            }
            
        }
        public string[] getStack(){
            string message = "Stack files found: ";
            string[] myStack = new String[3];
            try 
            {
                string[] ccPaths = Directory.GetFiles("./Bank", "*.stack");
                string note = "Select the file you wish to use.";
                consolePrintList(ccPaths, true, message, true);
                int choice = getUserInput(ccPaths.Length, note) - 1;
                if(choice > -1)
                {
                    myStack[0] = ccPaths[choice]; //Choose the cloudcoin to be added to the mp3.
                    myStack[1] = System.IO.File.ReadAllText(myStack[0]);//save the cloudcoin stack data.
                    myStack[2] = System.IO.Path.GetFileName(myStack[0]);//save the stacks name.
                    return myStack;
                }//end if
                else
                {
                     myStack[0] = "null";
                     myStack[1] = "";
                     myStack[2] = "";
                    return myStack;
                }
            } 
            catch (Exception e) 
            {
                Console.Out.WriteLine("The process failed: {0}", e.ToString());
                myStack[0] = e.ToString();
                return myStack;
            }
        }

        //Method to prompt a user for input. 
        public static int getUserInput(int maxNum, string message)
        {     
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            int choice = reader.readInt(0, maxNum);
            return choice;
        }
                  //Method to prompt a user for input. 
        public static bool getEnter(string message)
        {     
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            if(Console.ReadKey().Key == ConsoleKey.Enter){
                return true;
            }else{
                return false;
            }
        }

        //Methods accepts an array of strings. 
        //If indexed? indecese will be numbered 1 through selection.Length. 
        public static void consolePrintList(string[] selection, bool indexed, string message, bool goBack){
            int index = 0;
            Console.Out.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine(message);
            Console.Out.WriteLine("");
            foreach (string file in selection) 
            {   
                
                int fileLength = 120;
                
                if(indexed)
                {
                 string newFile = String.Format("{0, -25}", file);
                 string indexString = "     "+(index+1).ToString()+": ";
                 Console.Out.WriteLine("{0,-5} {1,3:N1}", indexString, newFile);
                 index++;
                }
                else
                {
                    Console.Out.WriteLine("{0, -4} {1,"+fileLength+":N1}", file, " ");
                }
                
            }//end foreach
            if(goBack)
            {
                Console.WriteLine();
                string backMsg = "Type '0' (Zero), then enter to go back.";
                Console.Out.WriteLine("{0, -4} {1,"+(Console.WindowWidth - backMsg.Length-1)+":N1}", backMsg, " ");
            }// end if

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }//End consolePrintList();
        public static int printOptions()//One of two possible dialogue screens the user is presented with.
        {
            string note = "Enter your selection: ";
            string[] userChoices = new string[] {
                "Select your PNG                                                       ",   //Option 1
                "Select your CloudCoins                                                ",   //Option 2
                "Insert the CloudCoins into the PNG                                    ",   //Option 3
                "Get CloudCoins from PNG                                               ",   //Option 4
                "Quit                                                                  "    //Option 4
            };
            consolePrintList(userChoices, true, note, false); //1st bool true? message is indexed. 2nd bool, no goBack.
            return getUserInput(userChoices.Length,note);//7? Range of inputs.
        } // End print welcome.

    }
}