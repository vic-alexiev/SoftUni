using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Text;
using HBitcoin.KeyManagement;
using NBitcoin;
using QBitNinja.Client;
using QBitNinja.Client.Models;
using static System.Console;

namespace BitcoinWallet
{
    internal class BitcoinWalletManager
    {
        private const string WalletFilePath = @"../../../Wallets"; // Path where you want to store the Wallets
        private const int NumberOfAddresses = 10;
        private static readonly Network TestNetwork = Network.TestNet;

        private static void Main(string[] args)
        {
            string[] avaliableOperations =
            {
                "create", "recover", "balance", "history", "receive", "send", "exit" //Allowed functionality
            };
            string input = string.Empty;
            while (!input.ToLower().Equals("exit"))
            {
                do
                {
                    Write(
                        "Enter operation [\"Create\", \"Recover\", \"Balance\", \"History\", \"Receive\", \"Send\", \"Exit\"]: ");
                    input = ReadLine().ToLower().Trim();
                } while (!((IList)avaliableOperations).Contains(input));

                switch (input)
                {
                    case "create":
                        CreateWallet();
                        break;
                    case "recover":
                        WriteLine(
                            "Please note the wallet cannot check if your password is correct or not. " +
                            "If you provide a wrong password a wallet will be recovered with your " +
                            "provided mnemonic AND password pair: ");
                        Write("Enter password: ");
                        string password = ReadLine();
                        Write("Enter mnemonic phrase: ");
                        string mnemonicWords = ReadLine();
                        Write("Enter date (yyyy-MM-dd): ");
                        string date = ReadLine();
                        Mnemonic mnemonic = new Mnemonic(mnemonicWords);
                        RecoverWallet(password, mnemonic, date);
                        break;
                    case "receive":
                        Write("Enter wallet's name: ");
                        String walletName = ReadLine();
                        Write("Enter password: ");
                        password = ReadLine();
                        Receive(password, walletName);
                        break;
                    case "balance":
                        Write("Enter wallet's name: ");
                        walletName = ReadLine();
                        Write("Enter password: ");
                        password = ReadLine();
                        Write("Enter wallet address: ");
                        string wallet = ReadLine();
                        ShowBalance(password, walletName, wallet);
                        break;
                    case "history":
                        Write("Enter wallet's name: ");
                        walletName = ReadLine();
                        Write("Enter password: ");
                        password = ReadLine();
                        Write("Enter wallet address: ");
                        wallet = ReadLine();
                        ShowHistory(password, walletName, wallet);
                        break;
                    case "send":
                        Write("Enter wallet's name: ");
                        walletName = ReadLine();
                        Write("Enter wallet password: ");
                        password = ReadLine();
                        Write("Enter wallet address: ");
                        wallet = ReadLine();
                        Write("Select outpoint (transaction ID): ");
                        string outPoint = ReadLine();
                        Send(password, walletName, wallet, outPoint);
                        break;
                }
            }
        }

        private static void Send(string password, string walletName, string wallet, string outPoint)
        {
            BitcoinExtKey privateKey = null;
            try
            {
                Safe loadedSafe = Safe.Load(password, Path.Combine(WalletFilePath, $"{walletName}.json"));
                for (int i = 0; i < NumberOfAddresses; i++)
                {
                    BitcoinAddress address = loadedSafe.GetAddress(i);
                    if (address.ToString() == wallet)
                    {
                        Write("Enter private key: ");
                        privateKey = new BitcoinExtKey(ReadLine());
                        if (!privateKey.Equals(loadedSafe.FindPrivateKey(address)))
                        {
                            WriteLine("Wrong private key!");
                            return;
                        }
                        break;
                    }
                }
            }
            catch
            {
                WriteLine("Wrong wallet or password!");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(TestNetwork);
            BalanceModel balance = client.GetBalance(
                BitcoinAddress.Create(wallet, TestNetwork)).Result;

            OutPoint outpointToSpend = null;
            foreach (BalanceOperation entry in balance.Operations)
            {
                foreach (ICoin coin in entry.ReceivedCoins)
                {
                    string coinOutpoint = coin.Outpoint.ToString();
                    if (coinOutpoint.Substring(0, coinOutpoint.Length - 2) == outPoint)
                    {
                        outpointToSpend = coin.Outpoint;
                        break;
                    }
                }
            }

            Transaction transaction = new Transaction();
            TxIn spendTxIn = new TxIn
            {
                PrevOut = outpointToSpend,
                ScriptSig = privateKey.ScriptPubKey
            };
            transaction.Inputs.Add(spendTxIn);
            ICoin[] coins = new[]
            {
                new Coin(spendTxIn.PrevOut, new TxOut()
                {
                    ScriptPubKey = spendTxIn.ScriptSig
                })
            };

            Write("Enter address to send to: ");
            string addressToSendTo = ReadLine();
            BitcoinAddress hallOfTheMakersAddress =
                BitcoinAddress.Create(addressToSendTo, TestNetwork);

            Write("Enter amount to send: ");
            decimal amountToSend = decimal.Parse(ReadLine());
            TxOut hallOfTheMakersTxOut = new TxOut
            {
                Value = new Money(amountToSend, MoneyUnit.BTC),
                ScriptPubKey = hallOfTheMakersAddress.ScriptPubKey
            };
            Write("Enter amount to get back: ");
            decimal amountToGetBack = decimal.Parse(ReadLine());
            TxOut getBackTxOut = new TxOut
            {
                Value = new Money(amountToGetBack, MoneyUnit.BTC),
                ScriptPubKey = privateKey.ScriptPubKey
            };

            Write("Enter message: ");
            string message = ReadLine();
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            TxOut messageTxOut = new TxOut
            {
                Value = Money.Zero,
                ScriptPubKey = TxNullDataTemplate.Instance.GenerateScriptPubKey(messageBytes)
            };
            transaction.Outputs.AddRange(new[] { hallOfTheMakersTxOut, getBackTxOut, messageTxOut });

            transaction.Sign(privateKey, coins);

            BroadcastResponse broadcastResponse = client.Broadcast(transaction).Result;
            if (broadcastResponse.Success)
            {
                WriteLine("Transaction sent successfully!");
            }
            else
            {
                WriteLine("something went worng!:-(");
            }
        }

        private static void ShowHistory(string password, string walletName, string wallet)
        {
            try
            {
                Safe loadedSafe = Safe.Load(
                    password, Path.Combine(WalletFilePath, $"{walletName}.json"));
            }
            catch
            {
                WriteLine("Wrong wallet or password!");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(TestNetwork);
            BalanceModel coinsReceived = client.GetBalance(
                BitcoinAddress.Create(wallet, TestNetwork), true).Result;

            string header = "-----COINS RECEIVED-----";
            WriteLine(header);

            foreach (BalanceOperation entry in coinsReceived.Operations)
            {
                foreach (ICoin coin in entry.ReceivedCoins)
                {
                    Money amount = (Money)coin.Amount;
                    WriteLine(
                        $"Transaction ID: {coin.Outpoint}; Received coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                }
            }

            WriteLine(new string('-', header.Length));

            string footer = "-----COINS SPENT-----";
            WriteLine(footer);

            BalanceModel coinsSpent = client.GetBalance(
                BitcoinAddress.Create(wallet, TestNetwork)).Result;
            foreach (BalanceOperation entry in coinsSpent.Operations)
            {
                foreach (ICoin coin in entry.SpentCoins)
                {
                    Money amount = (Money)coin.Amount;
                    WriteLine(
                        $"Transaction ID: {coin.Outpoint}; Spent coins: {amount.ToDecimal(MoneyUnit.BTC)}");
                }
            }

            WriteLine(new string('-', footer.Length));
        }

        private static void ShowBalance(string password, string walletName, string wallet)
        {
            try
            {
                Safe loadedSafe = Safe.Load(
                    password, Path.Combine(WalletFilePath, $"{walletName}.json"));
            }
            catch
            {
                WriteLine("Wrong wallet or password!");
                return;
            }

            QBitNinjaClient client = new QBitNinjaClient(TestNetwork);
            decimal totalBalance = 0;

            BalanceModel balance = client.
                GetBalance(BitcoinAddress.Create(wallet, TestNetwork), true).Result;
            foreach (BalanceOperation entry in balance.Operations)
            {
                foreach (ICoin coin in entry.ReceivedCoins)
                {
                    Money coinAmount = (Money)coin.Amount;
                    decimal amount = coinAmount.ToDecimal(MoneyUnit.BTC);
                    totalBalance += amount;
                }
            }

            WriteLine($"Balance of wallet: {totalBalance}");
        }

        private static void Receive(string password, string walletName)
        {
            try
            {
                Safe loadedSafe = Safe.Load(
                    password, Path.Combine(WalletFilePath, $"{walletName}.json"));
                for (int i = 0; i < NumberOfAddresses; i++)
                {
                    WriteLine(loadedSafe.GetAddress(i));
                }
            }
            catch (Exception)
            {
                WriteLine("Wallet with such name does not exist!");
            }
        }

        private static void RecoverWallet(string password, Mnemonic mnemonic, string date)
        {
            Random random = new Random();
            Safe safe = Safe.Recover(
                mnemonic,
                password,
                Path.Combine(WalletFilePath, $"RecoveredWalletNum{random.Next()}.json"),
                TestNetwork,
                DateTimeOffset.ParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture));

            WriteLine("Wallet successfully recovered");
        }

        private static void CreateWallet()
        {
            string password;
            string passwordConfirmed;
            do
            {
                Write("Enter password: ");
                password = ReadLine();
                Write("Confirm password: ");
                passwordConfirmed = ReadLine();
                if (password != passwordConfirmed)
                {
                    WriteLine("Passwords did not match!");
                    WriteLine("Try again.");
                }
            } while (password != passwordConfirmed);

            bool failure = true;
            while (failure)
            {
                try
                {
                    Write("Enter wallet name: ");
                    string walletName = ReadLine();
                    Mnemonic mnemonic;

                    Safe safe = Safe.Create(
                        out mnemonic,
                        password,
                        Path.Combine(WalletFilePath, $"{walletName}.json"),
                        TestNetwork);

                    WriteLine("Wallet created successfully");
                    WriteLine("Write down the following mnemonic words.");
                    WriteLine("With the mnemonic words AND the password you can recover this wallet.");
                    WriteLine();
                    WriteLine("----------");
                    WriteLine(mnemonic);
                    WriteLine("----------");
                    WriteLine(
                        "Write down and keep in a SECURE place your private keys. Only through them you can access your coins!");

                    for (int i = 0; i < NumberOfAddresses; i++)
                    {
                        BitcoinAddress address = safe.GetAddress(i);
                        WriteLine(
                            $"Address: {address} -> Private key: {safe.FindPrivateKey(address)}");
                    }

                    failure = false;
                }
                catch
                {
                    WriteLine("Wallet already exists");
                }
            }
        }
    }
}
