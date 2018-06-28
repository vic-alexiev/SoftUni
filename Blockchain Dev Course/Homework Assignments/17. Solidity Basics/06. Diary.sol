pragma solidity ^0.4.23;

contract Diary {
    event Fact(
        string text,
        address creator
    );
    string[] private _facts;
    address private _owner;
    mapping(address => uint8) private _allowedReaders;

    modifier ownerOnly() {
        require(msg.sender == _owner);
        _;
    }

    modifier allowedReadersOnly() {
        require(_allowedReaders[msg.sender] == 1);
        _;
    }

    constructor() public {
        _owner = msg.sender;
        _allowedReaders[0xCA35b7d915458EF540aDe6068dFe2F44E8fa733c] = 1;
        _allowedReaders[0x14723A09ACff6D2A60DcdF7aA4AFf308FDDC160C] = 1;
        _allowedReaders[0x4B0897b0513fdC7C541B6d9D7E929C4e5364D2dB] = 1;
    }
    
    function add(string fact) public ownerOnly {
        _facts.push(fact);
        emit Fact(fact, msg.sender);
    }
    
    function count() public view returns(uint) {
        return _facts.length;
    }
    
    function getFact(uint index) public allowedReadersOnly view returns(string) {
        require(index < _facts.length);
        return _facts[index];
    }
}