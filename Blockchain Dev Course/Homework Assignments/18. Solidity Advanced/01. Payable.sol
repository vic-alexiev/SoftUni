pragma solidity ^0.4.23;

contract Payable {
    event Deposit(
        address indexed from,
        uint256 value
    );

    event Send(
        address indexed from,
        address indexed to,
        uint256 value
    );

    address private _owner;

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }
    
    constructor() public {
        _owner = msg.sender;
    }

    function () public payable {
        emit Deposit(msg.sender, msg.value);
    }
    
    function getBalance() public view returns(uint256) {
        return address(this).balance;
    }

    function destroy() public ownerOnly {
        selfdestruct(_owner);
    }

    function destroyAndSend(address to) public ownerOnly {
        require(msg.sender != to, "Sender and recipient must be different.");
        emit Send(msg.sender, to, address(this).balance);
        selfdestruct(to);
    }
}