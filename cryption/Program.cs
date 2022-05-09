using System;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using Nethereum.Hex.HexConvertors.Extensions;
using Nethereum.Signer;
using Nethereum.Web3;

namespace crypto
{
    class Program
    {
        Random rn = new Random();
        static void Main(string[] args)
        {
            String firstMacAddress = NetworkInterface
                .GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault();

            string title = @"

                         ▄████▄   ██▀███ ▓██   ██▓ ██▓███  ▄▄▄█████▓ ██▓ ▒█████   ███▄    █ 
                        ▒██▀ ▀█  ▓██ ▒ ██▒▒██  ██▒▓██░  ██▒▓  ██▒ ▓▒▓██▒▒██▒  ██▒ ██ ▀█   █ 
                        ▒▓█    ▄ ▓██ ░▄█ ▒ ▒██ ██░▓██░ ██▓▒▒ ▓██░ ▒░▒██▒▒██░  ██▒▓██  ▀█ ██▒
                        ▒▓▓▄ ▄██▒▒██▀▀█▄   ░ ▐██▓░▒██▄█▓▒ ▒░ ▓██▓ ░ ░██░▒██   ██░▓██▒  ▐▌██▒
                        ▒ ▓███▀ ░░██▓ ▒██▒ ░ ██▒▓░▒██▒ ░  ░  ▒██▒ ░ ░██░░ ████▓▒░▒██░   ▓██░
                        ░ ░▒ ▒  ░░ ▒▓ ░▒▓░  ██▒▒▒ ▒▓▒░ ░  ░  ▒ ░░   ░▓  ░ ▒░▒░▒░ ░ ▒░   ▒ ▒ 
                          ░  ▒     ░▒ ░ ▒░▓██ ░▒░ ░▒ ░         ░     ▒ ░  ░ ▒ ▒░ ░ ░░   ░ ▒░
                        ░          ░░   ░ ▒ ▒ ░░  ░░         ░       ▒ ░░ ░ ░ ▒     ░   ░ ░ 
                        ░ ░         ░     ░ ░                        ░      ░ ░           ░ 
                        ░                 ░ ░                                               

";
            Console.WriteLine(title);

            string providerQuestion = "Enter your provider (Default: 'https://main-rpc.linkpool.io/')";
            writeCenter(providerQuestion);
            string provider = Console.ReadLine();
            if (provider == "")
            {
                provider = "https://main-rpc.linkpool.io/";
            }

            Random rn = new Random();
            Console.ForegroundColor = ConsoleColor.Red;
            bool success = false;
            string validPublicKey = "";
            string validPrivateKey = "";
            double validEtherAmount = 0;
            while (!success)
            {
                writeKey();
            }

            Thread.Sleep(1500);
            Console.ForegroundColor = ConsoleColor.Green;

            writeCenter($"Public key: {validPublicKey} | Private key: {validPrivateKey} | Balance: {validEtherAmount}");
            ConsoleSpinner spinner = new ConsoleSpinner();
            spinner.Delay = 300;
            while (spinner.counter < rn.Next(20, 30))
            {
                spinner.Turn("Securing funds ", 0);
            }

            Console.WriteLine("--- Funds secured ---");
            string end = Console.ReadLine();



            async void writeKey()
            {

                string privateKey = getKey();
                string publicKey = "";
                try
                {
                    var key = new EthECKey(privateKey.HexToByteArray(), true);
                    publicKey = key.GetPublicAddress();

                    var web3 = new Nethereum.Web3.Web3(provider);
                    var balance = await web3.Eth.GetBalance.SendRequestAsync(publicKey);
                    double etherAmount = (double)Web3.Convert.FromWei(balance.Value);
                    if (etherAmount != 0)
                    {
                        
                    }
                    if (etherAmount > 0.001)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        success = true;
                        validPrivateKey = privateKey;
                        validPublicKey = publicKey;
                        validEtherAmount = etherAmount;
                    }
                    else
                    {
                        if (success) return;
                        string keyResult = $"Public key: {publicKey} | Private key: {privateKey} | Balance: {etherAmount}";
                        Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (keyResult.Length / 2)) + "}", keyResult));
                        Console.ForegroundColor = ConsoleColor.Red;
                    }



                }
                catch (Exception)
                {
                }
            }

            string getKey()
            {
                string kars = "0123456789abcdef";
                string hex = "";
                for (int i = 0; i < 64; i++)
                {
                    hex += kars.Substring(rn.Next(kars.Length - 1), 1);
                }
                return hex;
            }
        }

        private static void writeCenter(string text)
        {
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (text.Length / 2)) + "}", text));
        }

        public class ConsoleSpinner
        {
            static string[,] sequence = null;

            public int Delay { get; set; } = 200;

            int totalSequences = 0;
            public int counter;

            public ConsoleSpinner()
            {
                Console.ForegroundColor = ConsoleColor.Green;
                counter = 0;
                sequence = new string[,] {
                { "/", "-", "\\", "|" },
                { ".", "o", "0", "o" },
                { "+", "x","+","x" },
                { "V", "<", "^", ">" },
                { ".   ", "..  ", "... ", "...." },
                { "=>   ", "==>  ", "===> ", "====>" },
            };

                totalSequences = sequence.GetLength(0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sequenceCode"> 0 | 1 | 2 |3 | 4 | 5 </param>
            public void Turn(string displayMsg = "", int sequenceCode = 0)
            {
                counter++;

                Thread.Sleep(Delay);

                sequenceCode = sequenceCode > totalSequences - 1 ? 0 : sequenceCode;

                int counterValue = counter % 4;

                string fullMessage = displayMsg + sequence[sequenceCode, counterValue];
                int msglength = fullMessage.Length;

                Console.Write(fullMessage);

                Console.SetCursorPosition(Console.CursorLeft - msglength, Console.CursorTop);
            }
        }
    }
}
