pragma solidity ^0.4.23;

contract NextToLastInvoker {
    address private _lastInvoker;
    
    function getNextToLastInvoker() public returns (bool, address) {
        address nextToLastInvoker = _lastInvoker;
        _lastInvoker = msg.sender;
        return (nextToLastInvoker != 0x0, nextToLastInvoker);
    }
}