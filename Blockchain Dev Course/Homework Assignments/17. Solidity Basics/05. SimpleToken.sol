pragma solidity ^0.4.23;

contract SimpleToken {
    mapping (address => uint256) private _balanceOf;
    
    constructor(uint256 initialSupply) public {
        _balanceOf[msg.sender] = initialSupply;
    }
    
    function transfer(address to, uint256 value) public {
        require(_balanceOf[msg.sender] >= value);
        require(_balanceOf[to] + value >= _balanceOf[to]);
        _balanceOf[msg.sender] -= value;
        _balanceOf[to] += value;
    }

    function getBalance() public view returns(uint256) {
        return _balanceOf[msg.sender];
    }
}