pragma solidity ^0.4.23;

contract Payable {
    event Deposit(
        address indexed from,
        uint256 value
    );

    address internal owner;

    modifier ownerOnly() {
        require(msg.sender == owner, "Only owner can call this function.");
        _;
    }

    constructor() public {
        owner = msg.sender;
    }

    function deposit() public payable {
        emit Deposit(msg.sender, msg.value);
    }

    function getBalance() public view returns(uint256) {
        return address(this).balance;
    }
}

contract Terminable is Payable {
    event Send(
        address indexed recipient,
        uint256 balance
    );

    function terminate() public ownerOnly {
        selfdestruct(owner);
    }

    function terminateAndSend(address recipient) public ownerOnly {
        require(msg.sender != recipient, "Sender and recipient must be different.");
        emit Send(recipient, address(this).balance);
        selfdestruct(recipient);
    }
}