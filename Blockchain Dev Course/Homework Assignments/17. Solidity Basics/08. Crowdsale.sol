pragma solidity ^0.4.23;

contract Crowdsale {
    event Deposit(
        address indexed from,
        uint256 value
    );

    event Withdrawal(
        address indexed owner,
        uint256 value
    );

    event Closure(
        address indexed owner,
        uint256 remainingBalance
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
        require(msg.value > 0);
        emit Deposit(msg.sender, msg.value);
    }

    function withdraw() public ownerOnly {
        uint256 balanceBefore = address(this).balance;
        msg.sender.transfer(address(this).balance);
        emit Withdrawal(msg.sender, balanceBefore);
    }

    function getBalance() public ownerOnly view returns (uint256) {
        return address(this).balance;
    }

    function close() public ownerOnly {
        emit Closure(msg.sender, address(this).balance);
        selfdestruct(_owner);
    }
}