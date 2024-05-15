using System.Collections;
using System.Text;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Drawing;
using System.Security.Policy;
using System.IO;
using System.Collections.Generic;
using System.Numerics;
using System.Globalization;
using System.Net.Sockets;
using System.Net;
using System.Reflection.Emit;

namespace DES
{
    public partial class Form1 : Form
    {
        System.Windows.Forms.Timer Tt = new System.Windows.Forms.Timer();

        String Path = @"C:\Users\dell\OneDrive\Desktop\Keyss.txt";

        Bitmap off;

        // Set the server IP address and port
        //bool RSA = false;
        //bool DES = true;
        public static IPAddress serverIP = IPAddress.Parse("192.168.1.7"); // Replace with the server IP address

        public static int serverPort = 9000; // Replace with the server port number
                                             // Create a TCP client
        TcpClient client = new TcpClient();

        public static IPAddress ipAddress = IPAddress.Parse("192.168.1.6"); // Replace with your server IP address
        public static int port = 9000; // Replace with your desired port number
        public static int port2 = 9000;
        int ct_tick = 0;

        // Create a TCP listener
        public TcpListener listener = new TcpListener(ipAddress, port);
        //public TcpListener listener2 = new TcpListener(ipAddress, port2);

        byte[] PK = new byte[8];

        String Left = "";

        String Right = "";

        String Original_Left_Key = "";

        String Original_Right_Key = "";

        String DES_Key = "";

        String Plain_Text = "";

        public static String Keyyy = "";

        String Output_OF_Each_Round = "";

        String Cipher_Text = "";

        String Binary_Plain_Text = "";

        int Sbox_Round_Num = 0;

        int Encryption_Round_Number = 0;

        int Decryption_Round_Number = 15;


        int[] Shifting_Numbers = new int[16] { 1, 1, 2, 2, 2, 2, 2, 2, 1, 2, 2, 2, 2, 2, 2, 1 };

        public List<String> Keys_Left = new List<String>();

        public List<String> Keys_Right = new List<String>();

        public List<String> All_Keys = new List<String>();

        private static RandomNumberGenerator RNG = RandomNumberGenerator.Create();
        private static byte[] GenerateKey(int size)
        {
            byte[] key = new byte[size];
            RNG.GetBytes(key);
            return key;
        }

        public static readonly int[] PC1 =
        {
            57, 49, 41, 33, 25, 17,  9, 1,
            58, 50, 42, 34, 26, 18, 10, 2,
            59, 51, 43, 35, 27, 19, 11, 3,
            60, 52, 44, 36, 63, 55, 47, 39,
            31, 23, 15,  7, 62, 54, 46, 38,
            30, 22, 14,  6, 61, 53, 45, 37,
            29, 21, 13,  5, 28, 20, 12, 4
        };

        public static readonly int[] PC2 =
        {
            14, 17, 11, 24,  1,  5,  3, 28,
            15,  6, 21, 10, 23, 19, 12,  4,
            26,  8, 16,  7, 27, 20, 13,  2,
            41, 52, 31, 37, 47, 55, 30, 40,
            51, 45, 33, 48, 44, 49, 39, 56,
            34, 53, 46, 42, 50, 36, 29, 32
        };

        public static readonly int[] Expansion =
        {
            32,  1,  2,  3,  4,  5,
             4,  5,  6,  7,  8,  9,
             8,  9, 10, 11, 12, 13,
            12, 13, 14, 15, 16, 17,
            16, 17, 18, 19, 20, 21,
            20, 21, 22, 23, 24, 25,
            24, 25, 26, 27, 28, 29,
            28, 29, 30, 31, 32,  1
        };

        public static readonly int[] Initial_Permutation =
        {
            58, 50, 42, 34, 26, 18, 10, 2,
            60, 52, 44, 36, 28, 20, 12, 4,
            62, 54, 46, 38, 30, 22, 14, 6,
            64, 56, 48, 40, 32, 24, 16, 8,
            57, 49, 41, 33, 25, 17,  9, 1,
            59, 51, 43, 35, 27, 19, 11, 3,
            61, 53, 45, 37, 29, 21, 13, 5,
            63, 55, 47, 39, 31, 23, 15, 7
        };

        public static readonly int[] Final_Permutation =
        {
            40, 8, 48, 16, 56, 24, 64, 32,
            39, 7, 47, 15, 55, 23, 63, 31,
            38, 6, 46, 14, 54, 22, 62, 30,
            37, 5, 45, 13, 53, 21, 61, 29,
            36, 4, 44, 12, 52, 20, 60, 28,
            35, 3, 43, 11, 51, 19, 59, 27,
            34, 2, 42, 10, 50, 18, 58, 26,
            33, 1, 41, 9, 49, 17, 57, 25
        };

        public static readonly int[] Permutation =
        {
            16,  7, 20, 21, 29, 12, 28, 17,
             1, 15, 23, 26,  5, 18, 31, 10,
             2,  8, 24, 14, 32, 27,  3,  9,
            19, 13, 30,  6, 22, 11,  4, 25
        };

        int[][][] sBoxes = new int[][][]
        {
            new int[][] // S-box 1
            {
                new int[] { 14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7 },
                new int[] { 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8 },
                new int[] { 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0 },
                new int[] { 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13 }
            },
            new int[][] // S-box 2
            {
                new int[] { 15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10 },
                new int[] { 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5 },
                new int[] { 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15 },
                new int[] { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 }
            },
            new int[][] // S-box 3
            {
                new int[] { 10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8 },
                new int[] { 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1 },
                new int[] { 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7 },
                new int[] { 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12 }
            },
            new int[][] // S-box 4
            {
                new int[] { 7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15 },
                new int[] { 2, 15, 0, 8, 13, 3, 4, 7, 5, 10, 6, 12, 11, 9, 14, 1 },
                new int[] { 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9 },
                new int[] { 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4 }
            },
            new int[][] // S-box 5
            {
                new int[] { 2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9 },
                new int[] { 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6 },
                new int[] { 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14 },
                new int[] { 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3 }
            },
            new int[][] // S-box 6
            {

                new int[] { 12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11 },
                new int[] { 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8 },
                new int[] { 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6 },
                new int[] { 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13 }
            },
            new int[][] // S-box 7
            {
                new int[] { 4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1 },
                new int[] { 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6 },
                new int[] { 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2 },
                new int[] { 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12 }
            },
            new int[][] // S-box 8
            {
                new int[] { 13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7 },
                new int[] { 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2 },
                new int[] { 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8 },
                new int[] { 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11 }
            }

        // Continue adding S-boxes in a similar manner
    };

        public static string GetKey(Byte[] PK)
        {
            for (int i = 0; i < PK.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Keyyy += (PK[i] & 0x80) > 0 ? "1" : "0";
                    PK[i] <<= 1;
                }
            }
            return Keyyy;
        }
        public String Permutation_Choice_1(String DES_Key)
        {
            String Modified_DES_Key = "";

            for (int i = 0; i < PC1.Length; i++)
            {
                if (DES_Key[PC1[i]] == '1')
                {
                    Modified_DES_Key += DES_Key[PC1[i]];
                }
                else
                {
                    Modified_DES_Key += DES_Key[PC1[i]];
                }
            }
            return Modified_DES_Key;
        }
        public Form1()
        {
            InitializeComponent();

            Tt.Tick += Tt_Tick;

            this.Load += Form1_Load;

            PK = GenerateKey(8);

            DES_Key = GetKey(PK);

            //MessageBox.Show("DES Key: " + DES_Key + " Key Length: " + DES_Key.Length + " Bit");

            //File.AppendAllText(Path, "DES Key: " + DES_Key + " Key Length: " + DES_Key.Length + " Bit" + '\n');

            String Key_After_PC1 = Permutation_Choice_1(DES_Key);

            //MessageBox.Show("DES_Key_After_PC1: " + Key_After_PC1 + " Key Length: " + Key_After_PC1.Length + " Bit");

            //File.AppendAllText(Path, "DES_Key_After_PC1: " + Key_After_PC1 + " Key Length: " + Key_After_PC1.Length + " Bit" + '\n' + '\n');

            String Left = Key_After_PC1.Substring(0, 28);

            String Right = Key_After_PC1.Substring(28, 28);

            Original_Left_Key = Left;

            Original_Right_Key = Right;

            //MessageBox.Show("Left Key: " + Left);

            //MessageBox.Show("Right Key: " + Right);

            Prepare_Keys(Left, Right);

            All_Keys = Permutation_Choice_2(All_Keys);

            listener.Start();

            Tt.Start();
        }

        private void Form1_Load(object? sender, EventArgs e)
        {
            off = new Bitmap(ClientSize.Width, ClientSize.Height);
            DrawDubb(this.CreateGraphics());
        }

        private void Tt_Tick(object? sender, EventArgs e)
        {
            //Awaiting_Connection_DES();
            Awaiting_Connection_RSA();
            DrawDubb(this.CreateGraphics());
        }

        public void Awaiting_Connection_RSA()
        {
            if (listener.Pending())
            {
                // Accept client connections
                TcpClient clientt = listener.AcceptTcpClient();

                // Get the network stream
                NetworkStream stream = clientt.GetStream();
                // Receive data from the client
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Receive the data from the server
                byte[] buffer2 = new byte[4096];
                int bytesRead2 = stream.Read(buffer2, 0, buffer2.Length);
                string dataReceived2 = Encoding.ASCII.GetString(buffer2, 0, bytesRead2);

                // Receive the data from the server
                byte[] buffer3 = new byte[4096];
                int bytesRead3 = stream.Read(buffer3, 0, buffer3.Length);
                string dataReceived3 = Encoding.ASCII.GetString(buffer3, 0, bytesRead3);
                MessageBox.Show(" String dataReceived3: " + dataReceived3);
                // Close the connection
                RSA_Decryption(Convert.ToInt16(dataReceived3), dataReceived, Encoding.ASCII.GetBytes(dataReceived2));
                clientt.Close();
            }
        }

        public void Awaiting_Connection_DES()
        {
            if (listener.Pending())
            {
                // Accept client connections
                TcpClient clientt = listener.AcceptTcpClient();

                // Get the network stream
                NetworkStream stream = clientt.GetStream();

                // Receive data from the client
                byte[] buffer = new byte[4096];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string dataReceived = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                // Receive the list from the server
                byte[] dataListBuffer = new byte[4096];
                bytesRead = stream.Read(dataListBuffer, 0, dataListBuffer.Length);
                string receivedDataList = Encoding.ASCII.GetString(dataListBuffer, 0, bytesRead);
                List<string> dataList = new List<string>(receivedDataList.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries));
                MessageBox.Show(dataList.Count + " List Size");
                Prepare_Decryption(dataReceived, dataList);
                //DES = false;
                // Close the connection
                clientt.Close();
            }
        }
        public void Prepare_Decryption(String Cipher_Text, List<string> All_Keys)
        {
            Output_OF_Each_Round = "";

            Decryption_Round_Number = 15;

            this.All_Keys = All_Keys;

            Cipher_Text = InitialPermutation(Cipher_Text);

            for (int i = 15; i >= 0; i--)
            {
                if (i == 15)
                {
                    Decrypt(Cipher_Text.Substring(0, 32), Cipher_Text.Substring(32, 32), this.All_Keys[i]);
                }
                else
                {
                    Decrypt(Output_OF_Each_Round.Substring(0, 32), Output_OF_Each_Round.Substring(32, 32), this.All_Keys[i]);
                }
                Decryption_Round_Number--;
            }

            Binary_Plain_Text = Final__Permutation(Output_OF_Each_Round);

            string asciiString = BinaryToAscii(Binary_Plain_Text);

            asciiString = asciiString.Replace("\0", " ");

            asciiString = asciiString.Replace(" ", "");

            //File.AppendAllText(Path, "" + '\n');

            //File.AppendAllText(Path, "Plain Text: " + Binary_Plain_Text + " " + Binary_Plain_Text.Length + " Bits" + '\n');

            //File.AppendAllText(Path, "Plain Text: " + asciiString + '\n');

            richTextBox1.AppendText("Recieved: " + asciiString + Environment.NewLine);
        }
        public List<string> Permutation_Choice_2(List<string> all_Keys)
        {
            String Modified_All_Keys = "";

            for (int Key = 0; Key < all_Keys.Count; Key++)
            {
                Modified_All_Keys = "";
                for (int i = 0; i < PC2.Length; i++)
                {
                    for (int index = 0; index < all_Keys[Key].Length; index++)
                    {
                        if (PC2[i] - 1 == index)
                        {
                            if (all_Keys[Key][index] == '1')
                            {
                                Modified_All_Keys += all_Keys[Key][index];
                            }
                            else
                            {
                                Modified_All_Keys += all_Keys[Key][index];
                            }
                        }
                    }
                }
                All_Keys[Key] = Modified_All_Keys;
                //MessageBox.Show("new All Key: " + All_Keys[Key] + " " + All_Keys[Key].Length + " Bit");
            }
            for (int i = 0; i < All_Keys.Count; i++)
            {
                try
                {
                    // Write content to the file
                    //writer.WriteLine(Keys_Left[i]);
                    //File.AppendAllText(Path, "All Keys_After_PC2: " + All_Keys[i] + " Key Length: " + All_Keys[i].Length + " Bit" + '\n' + '\n');
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            //File.AppendAllText(Path, "" + '\n');

            return All_Keys;
        }

        private void Prepare_Keys(String Left, String Right)
        {
            for (int i = 0; i < Shifting_Numbers.Length; i++)
            {
                int k = 1;
                if (i > 0)
                {
                    Left = Keys_Left[Keys_Left.Count - 1];
                    Right = Keys_Right[Keys_Right.Count - 1];
                }
                if (Shifting_Numbers[i] == 2)
                {
                    String Remaining_Bits_Left_Key = "";
                    String Left_Bits_Left_Key = "";
                    String Remaining_Bits_Right_Key = "";
                    String Left_Bits_Right_Key = "";

                    while (k < Shifting_Numbers[i])
                    {

                        Remaining_Bits_Left_Key = Left.Remove(k - 1, Shifting_Numbers[i]);

                        Left_Bits_Left_Key = Left.Remove(Shifting_Numbers[i]);

                        Remaining_Bits_Right_Key = Right.Remove(k - 1, Shifting_Numbers[i]);

                        Left_Bits_Right_Key = Right.Remove(Shifting_Numbers[i]);

                        k++;
                    }
                    //MessageBox.Show("Left After Shifting " + Left);
                    //MessageBox.Show(" Left Bits are " + Left_Bits_Left_Key);
                    //MessageBox.Show(" Remaining Bytes: " + Remaining_Bits_Left_Key);
                    Remaining_Bits_Left_Key += Left_Bits_Left_Key;
                    Remaining_Bits_Right_Key += Left_Bits_Right_Key;
                    //MessageBox.Show(" After Left Shift For Left Key: " + Remaining_Bits_Left_Key);
                    //MessageBox.Show(" After Left Shift For Right Key: " + Remaining_Bits_Right_Key);
                    Keys_Left.Add(Remaining_Bits_Left_Key);
                    Keys_Right.Add(Remaining_Bits_Right_Key);
                    All_Keys.Add(Keys_Left[Keys_Left.Count - 1] + Keys_Right[Keys_Right.Count - 1]);

                }
                else
                {
                    String Remaining_Bits_Left_Key = "";
                    String Left_Bits_Left_Key = "";
                    String Remaining_Bits_Right_Key = "";
                    String Left_Bits_Right_Key = "";

                    while (k <= Shifting_Numbers[i])
                    {

                        Remaining_Bits_Left_Key = Left.Remove(k - 1, Shifting_Numbers[i]);

                        Left_Bits_Left_Key = Left.Remove(Shifting_Numbers[i]);

                        Remaining_Bits_Right_Key = Right.Remove(k - 1, Shifting_Numbers[i]);

                        Left_Bits_Right_Key = Right.Remove(Shifting_Numbers[i]);

                        k++;
                    }
                    //MessageBox.Show("Left After Shifting " + Left);
                    //MessageBox.Show(" Left Bits are " + Left_Bits_Left_Key);
                    //MessageBox.Show(" Remaining Bytes: " + Remaining_Bits_Left_Key);
                    Remaining_Bits_Left_Key += Left_Bits_Left_Key;

                    Remaining_Bits_Right_Key += Left_Bits_Right_Key;
                    //MessageBox.Show(" After Left Shift For Left Key: " + Remaining_Bits_Left_Key);
                    //MessageBox.Show(" After Left Shift For Right Key: " + Remaining_Bits_Right_Key);
                    Keys_Left.Add(Remaining_Bits_Left_Key);
                    Keys_Right.Add(Remaining_Bits_Right_Key);
                    All_Keys.Add(Keys_Left[Keys_Left.Count - 1] + Keys_Right[Keys_Right.Count - 1]);
                }
            }
            //MessageBox.Show("Keys_Left Length:" + Keys_Left.Count);
            //MessageBox.Show("Keys_Right Length:" + Keys_Right.Count);

            //File.AppendAllText(Path, "Original Left Keys: " + Original_Left_Key + '\n');
            //File.AppendAllText(Path, "Original Right Keys: " + Original_Right_Key + '\n' + '\n');

            for (int i = 0; i < Keys_Left.Count; i++)
            {
                try
                {
                    // Write content to the file
                    //writer.WriteLine(Keys_Left[i]);
                    //File.AppendAllText(Path, "Left Keys: " + Keys_Left[i] + '\n');
                    //File.AppendAllText(Path, "Right Keys: " + Keys_Right[i] + '\n');
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }

            //File.AppendAllText(Path, "" + '\n');

            for (int i = 0; i < All_Keys.Count; i++)
            {
                try
                {
                    // Write content to the file
                    //writer.WriteLine(Keys_Left[i]);
                    //File.AppendAllText(Path, "All Keys_Before_PC2: " + All_Keys[i] + " Key Length: " + All_Keys[i].Length + " Bit" + '\n' + '\n');
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            //File.AppendAllText(Path, "" + '\n');
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public string InitialPermutation(string Text)
        {
            //MessageBox.Show("" + Text.Length);

            String Modified_Text = "";

            for (int i = 0; i < Initial_Permutation.Length; i++)
            {
                Modified_Text += Text[Initial_Permutation[i] - 1];
            }

            return Modified_Text;
        }
        public String Padding(String Converted)
        {
            String Zeros = "0";
            int pad = 64 - Converted.Length;
            for (int i = 0; i < pad; i++)
            {
                Converted = Zeros + Converted;
            }

            //MessageBox.Show(Converted + " Length : " + Converted.Length + " Bit");
            return Converted;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            String Message = textBox1.Text;

            richTextBox1.AppendText(textBox1.Text + Environment.NewLine);

            string Converted = String.Empty;

            String Left_Half_Plain_Text = "";

            String Right_Half_Plain_Text = "";

            byte[] messageBytes = Encoding.ASCII.GetBytes(Message);

            for (int i = 0; i < messageBytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Converted += (messageBytes[i] & 0x80) > 0 ? "1" : "0";

                    messageBytes[i] <<= 1;
                }
            }

            if (Converted.Length < 64)
            {
                Converted = Padding(Converted);
            }

            //File.AppendAllText(Path, "Plain Text_Converted: " + Converted + " " + Converted.Length + " Bits" + '\n');

            Plain_Text = InitialPermutation(Converted);

            //MessageBox.Show("Modified_Plain_Text_After IP: " + Plain_Text + " Key Length: " + Plain_Text.Length + " Bit" + '\n');

            //File.AppendAllText(Path, "Modified_Plain_Text_After IP: " + Plain_Text + " " + Plain_Text.Length + " Bits" + '\n');

            Left_Half_Plain_Text = Plain_Text.Substring(0, 32);

            Right_Half_Plain_Text = Plain_Text.Substring(32, 32);

            Encryption_Round_Number = 0;

            for (int i = 0; i < 16; i++)
            {
                if (i == 0)
                {
                    Encrypt(Left_Half_Plain_Text, Right_Half_Plain_Text, All_Keys[i]);
                }
                else
                {
                    Encrypt(Output_OF_Each_Round.Substring(0, 32), Output_OF_Each_Round.Substring(32, 32), All_Keys[i]);
                }
                Encryption_Round_Number++;
            }

            Cipher_Text = Final__Permutation(Output_OF_Each_Round);

            try
            {
                // Create a TCP client
                TcpClient client = new TcpClient();

                Console.WriteLine("Connecting to the server...");

                // Connect to the server
                client.Connect(serverIP, serverPort);
                Console.WriteLine("Connected to the server.");

                // Get the network stream
                NetworkStream stream = client.GetStream();

                // Send data to the server
                string dataToSend = Cipher_Text;
                byte[] buffer = Encoding.ASCII.GetBytes(dataToSend);
                stream.Write(buffer, 0, buffer.Length);

                // Send the list to the client
                MessageBox.Show(this.All_Keys.Count + " List Size");
                byte[] dataListBuffer = Encoding.ASCII.GetBytes(string.Join("|", this.All_Keys));
                stream.Write(dataListBuffer, 0, dataListBuffer.Length);

                textBox1.Clear();

                // Close the connection
                client.Close();

                Console.WriteLine("Connection closed. Press any key to exit.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            //MessageBox.Show(" Cipher Text: " + Cipher_Text + " " + Cipher_Text.Length + " Bits");

            //File.AppendAllText(Path, "" + '\n');

            //File.AppendAllText(Path, "Cipher Text: " + Cipher_Text + " " + Cipher_Text.Length + " Bits" + '\n');

            //Cipher_Text = InitialPermutation(Cipher_Text);

            //Output_OF_Each_Round = "";

            //Decryption_Round_Number = 15;

            //for (int i = 15; i >= 0; i--)
            //{
            //    if (i == 15)
            //    {
            //        Decrypt(Cipher_Text.Substring(0, 32), Cipher_Text.Substring(32, 32), All_Keys[i]);
            //    }
            //    else
            //    {
            //        Decrypt(Output_OF_Each_Round.Substring(0, 32), Output_OF_Each_Round.Substring(32, 32), All_Keys[i]);
            //    }
            //    Decryption_Round_Number--;
            //}

            //Binary_Plain_Text = Final__Permutation(Output_OF_Each_Round);


            //File.AppendAllText(Path, "" + '\n');

            //File.AppendAllText(Path, "Plain Text: " + Binary_Plain_Text + " " + Binary_Plain_Text.Length + " Bits" + '\n');

            //string asciiString = BinaryToAscii(Binary_Plain_Text);

            //File.AppendAllText(Path, "Plain Text: " + asciiString + '\n');
        }
        static string BinaryToAscii(string binary)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < binary.Length; i += 8)
            {
                string binaryByte = binary.Substring(i, 8);
                int decimalValue = Convert.ToInt32(binaryByte, 2);
                char asciiCharacter = (char)decimalValue;
                sb.Append(asciiCharacter);
            }

            return sb.ToString();
        }
        public string Final__Permutation(string output_OF_Each_Round)
        {
            String Text = "";

            for (int i = 0; i < Final_Permutation.Length; i++)
            {
                Text += output_OF_Each_Round[Final_Permutation[i] - 1];
            }

            return Text;
        }

        public void Encrypt(string left_Half_Text, string right_Half_Text, String Key)
        {
            String Expanded_Text = "";

            for (int i = 0; i < Expansion.Length; i++)
            {
                if (right_Half_Text[Expansion[i] - 1] == '1')
                {
                    Expanded_Text += right_Half_Text[Expansion[i] - 1];
                }
                else
                {
                    Expanded_Text += right_Half_Text[Expansion[i] - 1];
                }
            }
            //File.AppendAllText(Path, "Key: " + Key + '\n' + " Expanded_Plain_Text: " + Expanded_Plain_Text + '\n');

            BigInteger Exp_Text = BinaryStringToBigInteger(Expanded_Text);

            BigInteger Keyy = BinaryStringToBigInteger(Key);

            BigInteger Result = Exp_Text ^ Keyy;

            string binaryResult = BigIntegerToBinaryString(Result);

            for (int i = 0; i < binaryResult.Length; i++)
            {
                if (binaryResult.Length < 48)
                {
                    binaryResult = "0" + binaryResult;
                }
            }

            //File.AppendAllText(Path, "Binary Result is: " + binaryResult + '\n');

            int constant = 5;

            String row = "";
            String col = "";

            //MessageBox.Show("row: " + row);
            /////////////     AREA 51 HELPPPPPPPPPPPPPP     /////////////////

            String Output_From_Sbox = "";

            Sbox_Round_Num = 0;

            for (int k = 1; k <= binaryResult.Length;)
            {
                row = "";
                col = "";

                if (k == 48)
                {
                    break;
                }

                for (int j = k; j < constant; j++)
                {
                    if (j < 47)
                    {
                        col += binaryResult[j];
                    }

                    if (j == constant - 1 && k < 47)
                    {
                        if (k == 1)
                        {
                            k = 0;
                            row += binaryResult[k];
                            row += binaryResult[constant];
                        }
                        else
                        {
                            row += binaryResult[k];
                            row += binaryResult[constant];
                        }
                        int intRow = Convert.ToInt32(row, 2);
                        int intCol = Convert.ToInt32(col, 2);
                        string binary = Convert.ToString(sBoxes[Sbox_Round_Num][intRow][intCol], 2).PadLeft(4, '0');
                        Output_From_Sbox += binary;
                        k = j + 2;
                        j = k;
                        constant += 6;
                        //MessageBox.Show("k: " + k + " " + "Constant: " + constant + "my column: " + col);
                        col = "";
                        row = "";
                        Sbox_Round_Num++;
                    }
                }
            }

            //MessageBox.Show("OutPut From S-Box: " + Output_From_Sbox + " " + Output_From_Sbox.Length + "Bits ");

            String Permuted = Permute(Output_From_Sbox);

            //MessageBox.Show("Result After Permutation: " + Permuted);

            string XOR_Result = XOR_With_Left(Permuted, left_Half_Text);

            //MessageBox.Show("XOR Result: " + XOR_Result + " " + XOR_Result.Length + " Bits");

            String Output = "";

            if (Encryption_Round_Number == 15)
            {
                Output = XOR_Result + right_Half_Text;
            }
            else
            {
                Output = right_Half_Text + XOR_Result;
            }

            Output_OF_Each_Round = Output;
        }
        public void Decrypt(string left_Half_Text, string right_Half_Text, String Key)
        {
            String Expanded_Text = "";

            for (int i = 0; i < Expansion.Length; i++)
            {
                if (right_Half_Text[Expansion[i] - 1] == '1')
                {
                    Expanded_Text += right_Half_Text[Expansion[i] - 1];
                }
                else
                {
                    Expanded_Text += right_Half_Text[Expansion[i] - 1];
                }
            }
            //File.AppendAllText(Path, "Key: " + Key + '\n' + " Expanded_Plain_Text: " + Expanded_Plain_Text + '\n');

            BigInteger Exp_Text = BinaryStringToBigInteger(Expanded_Text);

            BigInteger Keyy = BinaryStringToBigInteger(Key);

            BigInteger Result = Exp_Text ^ Keyy;

            string binaryResult = BigIntegerToBinaryString(Result);

            for (int i = 0; i < binaryResult.Length; i++)
            {
                if (binaryResult.Length < 48)
                {
                    binaryResult = "0" + binaryResult;
                }
            }

            //File.AppendAllText(Path, "Binary Result is: " + binaryResult + '\n');

            int constant = 5;

            String row = "";
            String col = "";

            //MessageBox.Show("row: " + row);
            /////////////     AREA 51 HELPPPPPPPPPPPPPP     /////////////////

            String Output_From_Sbox = "";

            Sbox_Round_Num = 0;

            for (int k = 1; k <= binaryResult.Length;)
            {
                row = "";
                col = "";

                if (k == 48)
                {
                    break;
                }

                for (int j = k; j < constant; j++)
                {
                    if (j < 47)
                    {
                        col += binaryResult[j];
                    }

                    if (j == constant - 1 && k < 47)
                    {
                        if (k == 1)
                        {
                            k = 0;
                            row += binaryResult[k];
                            row += binaryResult[constant];
                        }
                        else
                        {
                            row += binaryResult[k];
                            row += binaryResult[constant];
                        }
                        int intRow = Convert.ToInt32(row, 2);
                        int intCol = Convert.ToInt32(col, 2);
                        string binary = Convert.ToString(sBoxes[Sbox_Round_Num][intRow][intCol], 2).PadLeft(4, '0');
                        Output_From_Sbox += binary;
                        k = j + 2;
                        j = k;
                        constant += 6;
                        //MessageBox.Show("k: " + k + " " + "Constant: " + constant + "my column: " + col);
                        col = "";
                        row = "";
                        Sbox_Round_Num++;
                    }
                }
            }

            //MessageBox.Show("OutPut From S-Box: " + Output_From_Sbox + " " + Output_From_Sbox.Length + "Bits ");

            String Permuted = Permute(Output_From_Sbox);

            //MessageBox.Show("Result After Permutation: " + Permuted);

            string XOR_Result = XOR_With_Left(Permuted, left_Half_Text);

            //MessageBox.Show("XOR Result: " + XOR_Result + " " + XOR_Result.Length + " Bits");

            String Output = "";

            if (Decryption_Round_Number == 0)
            {
                Output = XOR_Result + right_Half_Text;
            }
            else
            {
                Output = right_Half_Text + XOR_Result;
            }

            Output_OF_Each_Round = Output;
        }
        private string XOR_With_Left(string permuted, string left_Half)
        {
            BigInteger permutedd = BinaryStringToBigInteger(permuted);
            BigInteger left_Halff = BinaryStringToBigInteger(left_Half);
            BigInteger Result = permutedd ^ left_Halff;
            string binaryResult = BigIntegerToBinaryString(Result);
            for (int i = 0; i < binaryResult.Length; i++)
            {
                if (binaryResult.Length < 32)
                {
                    binaryResult = "0" + binaryResult;
                }
            }
            return binaryResult;
        }

        public String Permute(String Output_From_Sbox)
        {
            String Result = "";
            for (int i = 0; i < Permutation.Length; i++)
            {

                if (Output_From_Sbox[Permutation[i] - 1] == '1')
                {
                    Result += Output_From_Sbox[Permutation[i] - 1];
                }
                else
                {
                    Result += Output_From_Sbox[Permutation[i] - 1];
                }
            }
            return Result;
        }
        private static BigInteger BinaryStringToBigInteger(string binaryString)
        {
            BigInteger result = 0;
            foreach (char c in binaryString)
            {
                result <<= 1; // Left shift the result by 1 bit
                result += c == '1' ? 1 : 0; // Add 1 if the current bit is '1'
            }
            return result;
        }
        private static string BigIntegerToBinaryString(BigInteger number)
        {
            string binaryString = string.Empty;
            BigInteger quotient = number;

            while (quotient > 0)
            {
                BigInteger remainder;
                quotient = BigInteger.DivRem(quotient, 2, out remainder);
                binaryString = remainder.ToString() + binaryString;
            }

            return binaryString;
        }
        public void DrawScene(Graphics g)
        {
            g.Clear(Color.White);
        }
        public void DrawDubb(Graphics g)
        {
            Graphics g2 = Graphics.FromImage(off);
            DrawScene(g2);
            g.DrawImage(off, 0, 0);
        }
        public void RSA_Decryption(int i, string message1send, Byte[] encryptedsend)
        {
            string de = "";
            string[] nn = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            // Get input values

            string message1 = textBox1.Text;

            BigInteger r = 66;
            BigInteger p = 3;
            BigInteger q = 11;
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);
            BigInteger e = 7;
            BigInteger d = Aprismatic.BigIntegerExt.ModInverse(e, phi);
            //MessageBox.Show($"p={p}, q={q}, n={n}, phi={phi}, e={e}, d={d}");
            //MessageBox.Show("Private Key for encryption: " + "{ " + e + " , " + n + " } ");
            //MessageBox.Show("Public Key for decryption: " + "{ " + d + " , " + n + " } ");

            int flag = 0;

            r = 66;
            string message = "";

            message += message1send[i];
            //MessageBox.Show($"p={p}, q={q}, n={n}, phi={phi}, e={e}, d={d}");
            for (int j = 0; j < nn.Length; j++)
            {
                if (nn[j] == Convert.ToString(message1send[i]))
                {
                    flag = 1;
                    break;
                }
                else
                {
                    flag = 0;

                }
            }
            if (flag == 0)
            {

                if (message1send[i] > 'b')
                {
                    r += 33;
                }
                if (message1send[i] == 'A')
                {
                    r -= 33;
                }
                string decrypted = RSADecrypt2(encryptedsend, d, n, r);
                de += decrypted;
            }
            if (flag == 1)
            {
                r = 0;
                string decrypted = RSADecrypt(encryptedsend, d, n);
                de += decrypted;

            }

            richTextBox1.AppendText(de);
        }
        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs ee)
        {
            string ce = "";
            string[] nn = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
            // Get input values
            string message1 = textBox1.Text;
            richTextBox1.AppendText(Environment.NewLine + "Recieved:  " + textBox1.Text + Environment.NewLine);
            BigInteger r = 66;
            BigInteger p = 3;
            BigInteger q = 11;
            BigInteger n = p * q;
            BigInteger phi = (p - 1) * (q - 1);
            BigInteger e = 7;
            BigInteger d = Aprismatic.BigIntegerExt.ModInverse(e, phi);
            MessageBox.Show($"p={p}, q={q}, n={n}, phi={phi}, e={e}, d={d}");
            MessageBox.Show("Private Key for encryption: " + "{ " + e + " , " + n + " } ");
            MessageBox.Show("Public Key for decryption: " + "{ " + d + " , " + n + " } ");
            byte[] encrypted;
            for (int i = 0; i < message1.Length; i++)
            {
                int flag = 0;

                r = 66;
                string message = "";
                message += message1[i];

                for (int j = 0; j < nn.Length; j++)
                {
                    if (nn[j] == Convert.ToString(message1[i]))
                    {
                        flag = 1;
                        break;
                    }
                    else
                    {
                        flag = 0;
                    }
                }
                if (flag == 0)
                {
                    encrypted = RSAEncrypt2(message, e, n);
                    ce += Encoding.ASCII.GetString(encrypted);

                }
                if (flag == 1)
                {
                    r = 0;

                    encrypted = RSAEncrypt(message, e, n);
                    ce += Encoding.ASCII.GetString(encrypted);

                }
                try
                {
                    // Create a TCP client
                    TcpClient client = new TcpClient();

                    Console.WriteLine("Connecting to the server...");

                    // Connect to the server
                    client.Connect(serverIP, serverPort);
                    Console.WriteLine("Connected to the server.");

                    // Get the network stream
                    NetworkStream stream = client.GetStream();

                    // Send data to the server
                    string dataToSend = message1;
                    byte[] buffer = Encoding.ASCII.GetBytes(dataToSend);
                    stream.Write(buffer, 0, buffer.Length);

                    // Send data to the server

                    string dataToSend2 = ce;
                    byte[] buffer2 = Encoding.ASCII.GetBytes(dataToSend2);
                    stream.Write(buffer2, 0, dataToSend2.Length);


                    // Send data to the server
                    string dataToSend3 = i.ToString();
                    byte[] buffer3 = Encoding.ASCII.GetBytes(dataToSend3);
                    MessageBox.Show("Sent.");
                    stream.Write(buffer3, 0, buffer3.Length);

                    textBox1.Clear();
                    // Close the connection
                    client.Close();


                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                ce = "";

            }

        }
        static byte[] RSAEncrypt(string plaintext, BigInteger e, BigInteger n)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(plaintext);
            String aa = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    aa += (bytes[i] & 0x80) > 0 ? "1" : "0";

                    bytes[i] <<= 1;
                }
            }
            MessageBox.Show(aa + "in binary");

            int decimalValue = Convert.ToInt32(aa, 2);

            BigInteger m = new BigInteger(decimalValue);

            m = m - 48;
            MessageBox.Show("m " + m);
            BigInteger c = BigInteger.ModPow(m, e, n);
            MessageBox.Show("c " + c);

            return c.ToByteArray();
        }


        static string RSADecrypt(byte[] ciphertext, BigInteger d, BigInteger n)
        {
            BigInteger c = new BigInteger(ciphertext);
            MessageBox.Show("cc " + c);

            BigInteger m = BigInteger.ModPow(c, d, n);


            MessageBox.Show("m " + m);
            string character = m.ToString();


            return character;
        }

        static string RSADecrypt2(byte[] ciphertext, BigInteger d, BigInteger n, BigInteger r)
        {
            BigInteger c = new BigInteger(ciphertext);
            MessageBox.Show("cc " + c);

            BigInteger m = BigInteger.ModPow(c, d, n);

            m += r;
            MessageBox.Show("m " + m);
            int decimalCode = int.Parse(Convert.ToString(m), null);
            byte[] byteArray = new byte[] { (byte)decimalCode }; // create a byte array with the decimal code
            string character = Encoding.ASCII.GetString(byteArray);

            return character;
        }

        static byte[] RSAEncrypt2(string plaintext, BigInteger e, BigInteger n)
        {

            byte[] bytes = Encoding.ASCII.GetBytes(plaintext);
            String aa = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    aa += (bytes[i] & 0x80) > 0 ? "1" : "0";

                    bytes[i] <<= 1;
                }
            }
            MessageBox.Show(aa + "in binary");

            int decimalValue = Convert.ToInt32(aa, 2);


            BigInteger m = new BigInteger(decimalValue);



            BigInteger c = BigInteger.ModPow(m, e, n);


            return c.ToByteArray();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}