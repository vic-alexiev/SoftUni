pragma solidity ^0.4.23;

contract TimedAuction {
    event TokensSold(
        address indexed account,
        uint256 amount
    );

    mapping (address => uint256) private _tokenBalances;
    uint256 private _closingTime;
    address private _owner;

    constructor(uint256 tokenSupply, uint256 durationInMinutes) public {
        _owner = msg.sender;
        _closingTime = now + durationInMinutes * 1 minutes;
        _tokenBalances[_owner] = tokenSupply;
    }
    
    function buyTokens(uint256 amount) public {
        require(now <= _closingTime, "The auction has been closed.");
        require(msg.sender != _owner, "Auction owner isn't allowed to buy tokens.");
        require(amount <= _tokenBalances[_owner], "Amount is greater than what is currently available.");
        _tokenBalances[msg.sender] += amount;
        _tokenBalances[_owner] -= amount;
        emit TokensSold(msg.sender, amount);
    }

    function getTokenBalance() public view returns(uint256) {
        return _tokenBalances[msg.sender];
    }
}