let ethers = require('ethers');

function createWalletFromPrivateKey(privateKey) {
    return new ethers.Wallet(privateKey);
}

function createRandomWallet() {
    let wallet = ethers.Wallet.createRandom();
    return wallet;
}

function signTransaction(wallet, toAddress, value) {
    let transaction = {
        nonce: 0,
        gasLimit: 21000,
        gasPrice: ethers.utils.bigNumberify("2000000000"),
        to: toAddress,
        value: ethers.utils.parseEther(value),
        data: "0x"
    };

    return wallet.sign(transaction);
}

async function saveWalletToJSON(wallet, password) {
    return wallet.encrypt(password);
}

async function getWalletFromEncryptedJSON(json, password) {
    return ethers.Wallet.fromEncryptedWallet(json, password);
}

(async () => {
    let randomWallet = createRandomWallet();
    console.log("Random wallet: " + JSON.stringify(randomWallet, null, 2));

    let privateKey = "0x495d5c34c912291807c25d5e8300d20b749f6be44a178d5c50f167d495f3315a";
    let password = "p@ssw0rd~3";
    let wallet = createWalletFromPrivateKey(privateKey);
    console.log("Wallet: " + JSON.stringify(wallet, null, 2));

    let json = await saveWalletToJSON(wallet, password);
    console.log("Wallet encrypted as JSON: " + json);

    let decryptedWallet = await getWalletFromEncryptedJSON(json, password);
    console.log("Wallet decrypted from JSON: " + JSON.stringify(decryptedWallet, null, 2));

    let toAddress = "0x7725f560672A512e0d6aDFE7a761F0DbD8336aA7";
    let value = "1";
    let signedTransaction = signTransaction(wallet, toAddress, value);
    console.log("Signed transaction: " + signedTransaction);
})();