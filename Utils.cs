using System;
using System.Text;
using System.Text.RegularExpressions;

namespace AddToPng
{
    class Utils{
        
        public string[] printError = new string[1];
         
        // public static KeyboardReader reader = new KeyboardReader();

        //Methods accepts an array of strings. 
        //If indexed? indecese will be numbered 1 through selection.Length. 
        public static void consolePrintList(string[] selection, bool indexed, string message, bool goBack)
        {
            int index = 0;
            Console.Out.WriteLine("");
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Out.WriteLine(message);
            Console.Out.WriteLine("");
            foreach (string file in selection)
            {
                int fileLength = 120;
                if (indexed)
                {
                    string newFile = String.Format("{0, -25}", file);
                    string indexString = "     " + (index + 1).ToString() + ": ";
                    Console.Out.WriteLine("{0,-5} {1,3:N1}", indexString, newFile);
                    index++;
                }
                else
                {
                    Console.Out.WriteLine("{0, -4} {1," + fileLength + ":N1}", file, " ");
                }
            }//end foreach
            if (goBack)
            {
                Console.WriteLine();
                string backMsg = "Type '0' (Zero), then enter to go back.";
                Console.Out.WriteLine("{0, -4} {1," + (Console.WindowWidth - backMsg.Length - 1) + ":N1}", backMsg, " ");
            }// end if

            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;
        }//End consolePrintList();
        public static int getUserInput(int maxNum, string message)
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            int choice = 0;
            try{
                string returnStr = Console.ReadLine();
                Regex reg = new Regex("^[0-9]*$");
                
                if(reg.IsMatch(returnStr)){
                    int numb = 0;
                    Int32.TryParse(returnStr,out numb);
                    Console.WriteLine(numb);
                    if(numb <= maxNum)
                        return numb;
                    return getUserInput(maxNum, "Only 0-9 are allowed! max: " + maxNum);
                }
                else{
                    return getUserInput(maxNum, "Only 0-9 are allowed! max: " + maxNum);
                }
            }catch(Exception e){
                Console.WriteLine("Exception: 2" + e);
                choice = 0;
            }
            return choice;
        }
        //Method to prompt a user for input. 
        public static bool getEnter(string message)
        {
            Console.Out.WriteLine("");
            Console.Out.WriteLine(message);
            if (Console.ReadKey().Key == ConsoleKey.Enter)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int printOptions()//One of two possible dialogue screens the user is presented with.
        {
            string note = "Enter your selection: ";
            string[] userChoices = new string[] {
                "Add new png                                                           ",   //Option 1
                "Select your PNG                                                       ",   //Option 2
                "Select your CloudCoins                                                ",   //Option 3
                "Insert the CloudCoins into the PNG                                    ",   //Option 4
                "CloudCoins to Printouts folder                                        ",   //Option 5
                "Quit                                                                  "    //Option 6
            };
            consolePrintList(userChoices, true, note, false); //1st bool true? message is indexed. 2nd bool, no goBack.
            try{
                int selection = getUserInput(userChoices.Length, note);
                return selection;
            }catch(Exception e)
            {   
                Console.WriteLine("Exception Occured: "  + e);
                return 0;
            }
             //7? Range of inputs.
        } // End print welcome.

        public static void ReadBytes(string[] pngInfo, Encoding FileEncoding)
        {
            Console.OutputEncoding = FileEncoding; //set the console output.
            byte[] MyPng = System.IO.File.ReadAllBytes(pngInfo[1]);
            string ByteFile = Hex.Dump(MyPng); //Store Hex the Hex data from the Png file.
            System.IO.File.WriteAllText("./Printouts/HexPrintout_"+pngInfo[2]+".txt", ByteFile); //Create a document containing Png ByteFile (debugging).
        }
        public static string readString(int max, string message)
        {
            Console.WriteLine(message);
            string returnStr = Console.ReadLine();
            Regex reg = new Regex("^[a-zA-Z0-9]*$");

            if(reg.IsMatch(returnStr) && returnStr.Length <= max){
                if(returnStr.Length < 1)
                    return "tag";
                return returnStr;
            }
            else{
                return readString(max, "Only letters a-z,A-Z and numbers 0-9 are allowed! ");
            }
        }
    }
}
     
