let ethers = require('ethers');

function generateRandomHDWallet() {
    let wallet = ethers.Wallet.createRandom();
    return wallet;
}

function generateMnemonic() {
    let output = ethers.utils.randomBytes(16);
    return ethers.HDNode.entropyToMnemonic(output);
}

function restoreHDNode(mnemonic) {
    let hdNode = ethers.HDNode.fromMnemonic(mnemonic);
    return hdNode;
}

function generateRandomHDNode() {
    let mnemonic = generateMnemonic();
    let hdNode = restoreHDNode(mnemonic);
    return hdNode;
}

function deriveWalletsFromHDNode(mnemonic, derivationPath, count) {
    let wallets = [];

    for (let i = 0; i < count; i++) {
        let hdNode = restoreHDNode(mnemonic).derivePath(derivationPath + i);
        let wallet = new ethers.Wallet(hdNode.privateKey);
        wallets.push(wallet);
    }

    return wallets;
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

async function saveWalletAsJson(wallet, password) {
    return wallet.encrypt(password);
}

async function decryptWallet(json, password) {
    return ethers.Wallet.fromEncryptedWallet(json, password);
}

let mnemonic1 = "enact range stone boss alone october list vast laptop sunny iron price";
console.log("HD node: " + JSON.stringify(restoreHDNode(mnemonic1), null, 2));

console.log("Mnemonic: " + generateMnemonic());

console.log("Random HD node: " + JSON.stringify(generateRandomHDNode(), null, 2));

console.log("Random wallet: " + JSON.stringify(generateRandomHDWallet(), null, 2));

(async () => {
    let wallet = ethers.Wallet.createRandom();
    let password = "p@ssword";
    let json = await saveWalletAsJson(wallet, password);
    console.log("Wallet as JSON: " + json);

    let walletDecrypted = await decryptWallet(json, password);
    console.log("Decrypted wallet: " + JSON.stringify(walletDecrypted, null, 2));
})();

let mnemonic2 = generateMnemonic();
let derivationPath = "m/44'/60'/0'/0";
let wallets = deriveWalletsFromHDNode(mnemonic2, derivationPath, 5);
console.log("Derived wallets: " + JSON.stringify(wallets, null, 2));

let recipient = "0x933b946c4fec43372c5580096408d25b3c7936c5";
let value = "10.0";
let signedTransaction = signTransaction(wallets[1], recipient, value);
console.log("Signed transaction: " + signedTransaction);