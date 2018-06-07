var bitcoin = require('bitcoinjs-lib');

var keyPair = bitcoin.ECPair.makeRandom();

// Print your private key (in WIF format)
let privateKey = keyPair.toWIF();
console.log(`Private Key: ${privateKey}`);

// Print your public key
let publicKey = keyPair.getPublicKeyBuffer().toString('hex');
console.log(`Public Key: ${publicKey}`);

// Print your public key address
let address = keyPair.getAddress();
console.log(`Address: ${address}`);