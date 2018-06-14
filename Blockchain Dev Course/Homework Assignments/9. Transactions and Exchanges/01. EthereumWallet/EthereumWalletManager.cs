using System;
using static System.Console;
using System.Collections;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.HdWallet;
using Nethereum.Web3;
using Nethereum.Web3.Accounts;
using Newtonsoft.Json;
using NBitcoin;
using Rijndael256;
using System.Numerics;
using Nethereum.Hex.HexTypes;

namespace EthereumWallet
{
    internal class EthereumWalletManager
    {
        private const string Network = "https://ropsten.infura.io/R13BuegiIhZLUGVL3Qdq";
        private const string WorkingDirectory = @"../../../Wallets"; // Path where you want to store the Wallets
        private const int NumberOfAddresses = 20;

        private static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            //Initial params.
            string[] availableOperations =
            {
                "create", "load", "recover", "exit" // Allowed functionality
            };
            string input = string.Empty;
            bool isWalletReady = false;
            Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);

            Web3 web3 = new Web3(Network);
            Directory.CreateDirectory(WorkingDirectory);

            while (!input.ToLower().Equals("exit"))
            {
                if (!isWalletReady) // User still doesn't have a wallet.
                {
                    do
                    {
                        input = ReceiveCommandCreateLoadOrRecover();

                    } while (!((IList)availableOperations).Contains(input));
                    switch (input)
                    {
                        // Create brand-new wallet. User will receive mnemonic phrase, public and private keys.
                        case "create":
                            wallet = CreateWalletDialog();
                            isWalletReady = true;
                            break;

                        // Load wallet from json file contains encrypted mnemonic phrase (words).
                        // This command will decrypt words and load wallet.
                        case "load":
                            wallet = LoadWalletDialog();
                            isWalletReady = true;
                            break;

                        /* Recover wallet from mnemonic phrase (words) which user must enter.
                         This is useful if user has wallet, but has no json file for it 
                         (for example if he uses this program for the first time).
                         Command will creates new json file containing encrypted mnemonic phrase (words)
                         for this wallet.
                         After encrypt words program will load wallet.*/
                        case "recover":
                            wallet = RecoverWalletDialog();
                            isWalletReady = true;
                            break;

                        // Exit from the program.
                        case "exit":
                            return;
                    }
                }
                else // Already having loaded Wallet
                {
                    string[] walletAvailableOperations = {
                        "balance", "receive", "send", "exit" //Allowed functionality
                    };

                    string inputCommand = string.Empty;

                    while (!inputCommand.ToLower().Equals("exit"))
                    {
                        do
                        {
                            inputCommand = ReceiveCommandForEthersOperations();

                        } while (!((IList)walletAvailableOperations).Contains(inputCommand));
                        switch (inputCommand)
                        {
                            // Send transaction from address to address
                            case "send":
                                await SendTransactionDialog(wallet);
                                break;

                            // Shows the balances of addresses and total balance.
                            case "balance":
                                await GetWalletBallanceDialog(web3, wallet);
                                break;

                            // Shows the addresses in which you can receive coins.
                            case "receive":
                                Receive(wallet);
                                break;
                            case "exit":
                                return;
                        }
                    }
                }

            }
        }

        // Provided Dialogs.
        private static Wallet CreateWalletDialog()
        {
            try
            {
                string password;
                string passwordConfirmed;
                do
                {
                    Write("Enter password for encryption: ");
                    password = ReadLine();
                    Write("Confirm password: ");
                    passwordConfirmed = ReadLine();
                    if (password != passwordConfirmed)
                    {
                        WriteLine("Passwords did not match!");
                        WriteLine("Try again.");
                    }
                } while (password != passwordConfirmed);

                // Creating new Wallet with the provided password.
                Wallet wallet = CreateWallet(password, WorkingDirectory);
                return wallet;
            }
            catch (Exception)
            {
                WriteLine($"ERROR! Wallet in path {WorkingDirectory} can`t be created!");
                throw;
            }
        }

        private static Wallet LoadWalletDialog()
        {
            Write("Name of the file containing wallet: ");
            string nameOfWallet = ReadLine();
            Write("Password: ");
            string pass = ReadLine();
            try
            {
                // Loading the Wallet from an JSON file. Using the Password to decrypt it.
                Wallet wallet = LoadWalletFromJsonFile(nameOfWallet, WorkingDirectory, pass);
                return wallet;

            }
            catch (Exception)
            {
                WriteLine($"ERROR! Wallet {nameOfWallet} in path {WorkingDirectory} can`t be loaded!");
                throw;
            }
        }

        private static Wallet RecoverWalletDialog()
        {
            try
            {
                Write("Enter: Mnemonic words with single space separator: ");
                string mnemonicPhrase = ReadLine();
                Write("Enter: password for encryption: ");
                string passForEncryptionInJsonFile = ReadLine();

                // Recovering the Wallet from Mnemonic Phrase
                Wallet wallet = RecoverFromMnemonicPhraseAndSaveToJson(
                    mnemonicPhrase, passForEncryptionInJsonFile, WorkingDirectory);
                return wallet;
            }
            catch (Exception)
            {
                WriteLine("ERROR! Wallet can`t be recovered! Check your mnemonic phrase.");
                throw;
            }
        }

        private static async Task GetWalletBallanceDialog(Web3 web3, Wallet wallet)
        {
            WriteLine("Balance:");
            try
            {
                // Getting the Balance and Displaying the Information.
                await Balance(web3, wallet);
            }
            catch (Exception)
            {
                WriteLine("An error occurred! Check your wallet.");
            }
        }

        private static async Task SendTransactionDialog(Wallet wallet)
        {
            WriteLine("Enter: Address sending ethers.");
            string fromAddress = ReadLine();
            WriteLine("Enter: Address receiving ethers.");
            string toAddress = ReadLine();
            WriteLine("Enter: Amount of coins in ETH.");
            decimal amount = decimal.Zero;
            try
            {
                amount = decimal.Parse(ReadLine());
            }
            catch (Exception)
            {
                WriteLine("Unacceptable input for amount of coins.");
            }
            if (amount > decimal.Zero)
            {
                WriteLine($"You will send {amount} ETH from {fromAddress} to {toAddress}");
                WriteLine($"Are you sure? yes/no");
                string answer = ReadLine();
                if (answer.ToLower() == "yes")
                {
                    // Send the Transaction.
                    await Send(wallet, fromAddress, toAddress, amount);
                }
            }
            else
            {
                WriteLine("Amount of coins for transaction must be positive number!");
            }
        }

        private static string ReceiveCommandCreateLoadOrRecover()
        {
            WriteLine("Choose working wallet.");
            WriteLine("Choose [create] to create new Wallet.");
            WriteLine("Choose [load] to load existing Wallet from file.");
            WriteLine("Choose [recover] to recover Wallet with Mnemonic Phrase.");
            Write("Enter operation [\"Create\", \"Load\", \"Recover\", \"Exit\"]: ");
            string input = ReadLine().ToLower().Trim();
            return input;
        }

        private static string ReceiveCommandForEthersOperations()
        {
            Write("Enter operation [\"Balance\", \"Receive\", \"Send\", \"Exit\"]: ");
            string inputCommand = ReadLine().ToLower().Trim();
            return inputCommand;
        }

        private static Wallet CreateWallet(string password, string filePath)
        {
            Wallet wallet = new Wallet(Wordlist.English, WordCount.Twelve);
            string words = string.Join(" ", wallet.Words);
            string fileName = string.Empty;
            try
            {
                fileName = SaveWalletToJsonFile(wallet, password, filePath);
            }
            catch (Exception e)
            {
                WriteLine($"ERROR! The file can`t be saved! {e}");
                throw;
            }

            WriteLine("New Wallet was created successfully!");
            WriteLine("Write down the following mnemonic words and keep them in a safe place.");
            WriteLine(words);
            WriteLine("Seed:");
            WriteLine(wallet.Seed);
            WriteLine();
            PrintAddressesAndKeys(wallet);
            return wallet;
        }

        private static void PrintAddressesAndKeys(Wallet wallet)
        {
            WriteLine("Addresses:");
            for (int i = 0; i < NumberOfAddresses; i++)
            {
                WriteLine(wallet.GetAccount(i).Address);
            }

            WriteLine("Private keys:");
            for (int i = 0; i < NumberOfAddresses; i++)
            {
                WriteLine(wallet.GetAccount(i).PrivateKey);
            }

            WriteLine();
        }

        private static string SaveWalletToJsonFile(Wallet wallet, string password, string filePath)
        {
            string words = string.Join(" ", wallet.Words);
            var encryptedWords = Rijndael.Encrypt(words, password, KeySize.Aes256); // Encrypt the mnemonic phrase
            string date = DateTime.Now.ToString();
            // Anonymous object containing encrypted words and date will be written in the json file
            var walletJsonData = new { encryptedWords, date };
            string json = JsonConvert.SerializeObject(walletJsonData);
            Random random = new Random();
            var fileName =
                "EthereumWallet_" +
                DateTime.Now.Year + "-" +
                DateTime.Now.Month + "-" +
                DateTime.Now.Day + "_" +
                DateTime.Now.Hour + "_" +
                DateTime.Now.Minute + "_" +
                DateTime.Now.Second + "_" +
                random.Next(0, 10000) + ".json";
            File.WriteAllText(Path.Combine(filePath, fileName), json);
            WriteLine($"Wallet saved in file: {fileName}");
            return fileName;
        }

        private static Wallet LoadWalletFromJsonFile(string fileName, string path, string password)
        {
            string fileFullName = Path.Combine(path, fileName);
            string words = string.Empty;
            // Read from file
            WriteLine($"Read from {fileFullName}");
            try
            {
                string content = File.ReadAllText(fileFullName);
                dynamic json = JsonConvert.DeserializeObject<dynamic>(content);
                string encryptedWords = json.encryptedWords;
                words = Rijndael.Decrypt(encryptedWords, password, KeySize.Aes256);
            }
            catch (Exception e)
            {
                WriteLine("ERROR! " + e);
                throw;
            }

            return Recover(words);
        }

        private static Wallet Recover(string words)
        {
            Wallet wallet = new Wallet(words, null);
            WriteLine("Wallet was successfully recovered.");
            WriteLine("Words: " + string.Join(" ", wallet.Words));
            WriteLine("Seed: " + wallet.Seed);
            WriteLine();
            PrintAddressesAndKeys(wallet);
            return wallet;
        }

        private static Wallet RecoverFromMnemonicPhraseAndSaveToJson(string words, string password, string filePath)
        {
            Wallet wallet = Recover(words);
            string fileName = string.Empty;
            try
            {
                fileName = SaveWalletToJsonFile(wallet, password, filePath);
            }
            catch (Exception)
            {
                WriteLine($"ERROR! The file {fileName} with recovered wallet can`t be saved!");
                throw;
            }

            return wallet;
        }

        private static void Receive(Wallet wallet)
        {
            if (wallet.GetAddresses().Count() > 0)
            {
                for (int i = 0; i < NumberOfAddresses; i++)
                {
                    WriteLine(wallet.GetAccount(i).Address);
                }

                WriteLine();
            }
            else
            {
                WriteLine("No addresses found!");
            }
        }

        private static async Task Send(Wallet wallet, string fromAddress, string toAddress, decimal amount)
        {
            Account fromAccount = wallet.GetAccount(fromAddress);
            string fromPrivateKey = fromAccount.PrivateKey;
            if (string.IsNullOrWhiteSpace(fromPrivateKey))
            {
                WriteLine("Invalid sending wallet address!");
                throw new Exception("Invalid sending wallet address!");
            }

            Web3 web3 = new Web3(fromAccount, Network);
            BigInteger wei = Web3.Convert.ToWei(amount);
            try
            {
                var transaction = await web3.TransactionManager
                    .SendTransactionAsync(
                        fromAccount.Address,
                        toAddress,
                        new HexBigInteger(wei));
                WriteLine($"Transaction \"{transaction}\" has been sent successfully!");
            }
            catch (Exception e)
            {
                WriteLine($"ERROR! The transaction can`t be completed! {e}");
                throw;
            }
        }

        private static async Task Balance(Web3 web3, Wallet wallet)
        {
            decimal totalBalance = 0.0m;
            for (int i = 0; i < NumberOfAddresses; i++)
            {
                string address = wallet.GetAccount(i).Address;
                HexBigInteger balance = await web3.Eth.GetBalance
                    .SendRequestAsync(address);
                decimal etherAmount = Web3.Convert.FromWei(balance.Value);
                totalBalance += etherAmount;
                WriteLine($"{address}: {etherAmount} ETH");
            }

            WriteLine($"Total balance: {totalBalance} ETH \n");
        }
    }
}
