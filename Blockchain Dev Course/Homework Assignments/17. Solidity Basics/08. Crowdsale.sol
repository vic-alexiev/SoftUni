pragma solidity ^0.4.23;

contract Crowdsale {
    event Deposit(
        address indexed from,
        uint256 value
    );

    event Transfer(
        address indexed owner,
        uint256 value
    );

    event Withdrawal(
        address indexed owner,
        uint256 balance
    );

    address private _owner;

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }

    constructor() public {
        _owner = msg.sender;
    }

    function deposit() public payable {
        require(address(this).balance + msg.value >= address(this).balance, "Balance overflow.");
        emit Deposit(msg.sender, msg.value);
    }

    function transfer(address to, uint256 amount) public ownerOnly {
        require(amount <= address(this).balance, "Insufficient funds.");
        require(msg.sender != to, "Sender and recipient must be different.");
        to.transfer(amount);
        emit Transfer(msg.sender, amount);
    }

    function getBalance() public ownerOnly view returns (uint256) {
        return address(this).balance;
    }

    function destroyAndWithdraw() public ownerOnly {
        emit Withdrawal(msg.sender, address(this).balance);
        selfdestruct(_owner);
    }
}