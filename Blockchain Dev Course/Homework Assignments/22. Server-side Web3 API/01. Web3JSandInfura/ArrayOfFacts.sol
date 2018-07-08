pragma solidity ^0.4.23;

contract ArrayOfFacts {
    string[] private facts;
    address private contractOwner;

    modifier onlyOwner() {
        require(msg.sender == contractOwner);
        _;
    }

    constructor() public {
        contractOwner = msg.sender;
    }

    function add(string newFact) public onlyOwner {
        facts.push(newFact);
    }

    function count() public view returns(uint256) {
        return facts.length;
    }

    function getFact(uint256 index) public view returns(string) {
        return facts[index];
    }
}