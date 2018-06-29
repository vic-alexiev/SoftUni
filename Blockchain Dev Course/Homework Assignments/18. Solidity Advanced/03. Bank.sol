pragma solidity ^0.4.23;

contract Bank {
    event Deposit(
        address indexed account,
        uint256 amount,
        uint256 balanceAfter
    );

    event Withdrawal(
        address indexed account,
        uint256 amount,
        uint256 balanceAfter
    );

    address private _owner;
    mapping(address => uint256) private _balanceOf;

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }
    
    constructor() public {
        _owner = msg.sender;
    }

    function deposit() public payable returns(uint256) {
        require(_balanceOf[msg.sender] + msg.value >= _balanceOf[msg.sender], "Balance overflow.");

        uint256 amount = msg.value;
        _balanceOf[msg.sender] += amount;
        emit Deposit(msg.sender, amount, _balanceOf[msg.sender]);
        return _balanceOf[msg.sender];
    }

    function withdraw(uint256 amount) public returns(uint256) {
        require(amount <= _balanceOf[msg.sender], "Insufficient funds.");
        require(amount <= address(this).balance, "Insufficient bank liquidity.");
        _balanceOf[msg.sender] -= amount;
        msg.sender.transfer(amount);
        emit Withdrawal(msg.sender, amount, _balanceOf[msg.sender]);
        return _balanceOf[msg.sender];
    }

    function getBalance() public view returns(uint256) {
        return _balanceOf[msg.sender];
    }
}

contract BankAccount {
    address private _owner;

    constructor() public {
        _owner = msg.sender;
    }

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }

    function store() public payable {}

    function withdraw(uint256 amount) public ownerOnly {
        require(amount <= address(this).balance, "Insufficient funds.");
        msg.sender.transfer(amount);
    }

    function getBalance() public view returns(uint256){
        return address(this).balance;
    }
}
