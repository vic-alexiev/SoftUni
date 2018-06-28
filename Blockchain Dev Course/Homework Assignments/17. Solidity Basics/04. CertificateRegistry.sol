pragma solidity ^0.4.23;

contract CertificateRegistry {
    mapping (string => uint8) private _certificates;
    address private _owner;

    modifier ownerOnly() {
        require(msg.sender == _owner);
        _;
    }
  
    constructor() public {
        _owner = msg.sender;
    }

    function add(string hash) public ownerOnly {
        _certificates[hash] = 1;
    }

    function verify(string hash) public view returns (bool) {
        return _certificates[hash] == 1;
    }
}