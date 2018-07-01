pragma solidity ^0.4.23;

contract Counter {
    uint256 private count = 0;
    function incrementCounter() public {
        count += 1;
    }
    function decrementCounter() public {
        count -= 1;
    }
    function getCount() public view returns (uint256) {
        return count;
    }
}