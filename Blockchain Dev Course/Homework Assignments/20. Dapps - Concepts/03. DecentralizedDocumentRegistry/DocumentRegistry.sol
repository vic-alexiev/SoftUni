pragma solidity ^0.4.23;

contract DocumentRegistry {
    struct Document {
        string hash;
        uint256 dateAdded;
    }

    Document[] documents;
    address contractOwner;

    modifier onlyOwner() {
        require(contractOwner == msg.sender);
        _;
    }

    constructor() public {
        contractOwner = msg.sender;
    }

    function add(string hash) public onlyOwner returns (uint256 dateAdded) {
        dateAdded = block.timestamp;
        documents.push(Document(hash, dateAdded));
        return dateAdded;
    }

    function getDocumentsCount() public view returns (uint32 length) {
        return uint32(documents.length);
    }

    function getDocument(uint32 index) public view returns (string, uint256) {
        Document memory document = documents[index];
        return (document.hash, document.dateAdded);
    }
}
