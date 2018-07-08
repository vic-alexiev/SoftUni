const Web3 = require('web3');
const solc = require('solc');
const fs = require('fs-extra');
const provider = 'https://ropsten.infura.io/R13BuegiIhZLUGVL3Qdq';
const privateKey = '1483416fdba6fe3904c58e15994339241bdb5912b1986103fe71a22be35f9586';
let web3 = new Web3(new Web3.providers.HttpProvider(provider));

function readFile(fileName) {
    return fs.readFileSync(fileName, 'utf8');
}

function writeFile(fileName, content) {
    return fs.outputFileSync(fileName, content);
}

function compileContract(fileName, contractName) {
    let contactSource = readFile(fileName);
    let output = solc.compile(contactSource);
    return output.contracts[':' + contractName];
}

function deployContract(privateKey, filePath, contractName) {
    web3.eth.accounts.wallet.add(privateKey);
    let compiledContract = compileContract(filePath, contractName);
    let abi = compiledContract.interface;
    let bytecode = '0x' + compiledContract.bytecode;

    let contract = new web3.eth.Contract(JSON.parse(abi), null,
    {
        data: bytecode,
        from: web3.eth.accounts.wallet[0].address,
        gasPrice: "44000000000",
        gas: "50000"
    });

    contract
        .deploy()
        .send()
        .then((contractInstance) => {
            console.log("Contract created at: " + contractInstance.options.address);
        });
}

deployContract(privateKey, './ArrayOfFacts.sol', 'ArrayOfFacts');

//let compiledContract = compileContract('./ArrayOfFacts.sol', 'ArrayOfFacts');
//writeFile('./ArrayOfFacts.json', JSON.stringify(compiledContract));
//console.log(JSON.stringify(compiledContract));