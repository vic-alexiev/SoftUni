pragma solidity ^0.4.23;

contract SimpleStorage {
    uint private _storedData;
    
    function get() public view returns (uint) {
        return _storedData;
    }
    
    function set(uint value) public {
        _storedData = value;
    }
}