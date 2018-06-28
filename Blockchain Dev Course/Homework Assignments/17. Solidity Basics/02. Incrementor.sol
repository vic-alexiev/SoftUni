pragma solidity ^0.4.23;

contract Incrementor {
    uint private _value;
    
    function get() public view returns (uint) {
        return _value;
    }
    
    function increment(uint delta) public {
        _value += delta;
    }
}