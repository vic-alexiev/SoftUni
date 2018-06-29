pragma solidity ^0.4.23;

contract CompanyShares {
    event SharePurchase(
        address account,
        uint256 amount,
        uint256 pricePerShare
    );

    event Withdrawal(
        address account,
        uint256 shares,
        uint256 amount,
        uint256 pricePerShare,
        uint256 dividend
    );

    address private _owner;
    uint256 private _pricePerShare;
    uint256 private _dividend;
    mapping(address => uint256) private _sharesOf;
    mapping(address => bool) private _allowedToParticipate;
    mapping(address => bool) private _allowedToWithdraw;
    address[] private _shareholders;

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }

    constructor(uint256 initialSupply, uint256 pricePerShare, uint256 dividend) public {
        _owner = msg.sender;
        _sharesOf[_owner] = initialSupply;
        _pricePerShare = pricePerShare;
        _dividend = dividend;

        _allowedToParticipate[0x14723A09ACff6D2A60DcdF7aA4AFf308FDDC160C] = true;
        _allowedToParticipate[0x4B0897b0513fdC7C541B6d9D7E929C4e5364D2dB] = true;
        _allowedToParticipate[0x583031D1113aD414F02576BD6afaBfb302140225] = true;
        _allowedToParticipate[0xdD870fA1b7C4700F2BD7f44238821C26f7392148] = true;

        _allowedToWithdraw[0x14723A09ACff6D2A60DcdF7aA4AFf308FDDC160C] = true;
        _allowedToWithdraw[0x4B0897b0513fdC7C541B6d9D7E929C4e5364D2dB] = true;
        _allowedToWithdraw[0x583031D1113aD414F02576BD6afaBfb302140225] = true;
        _allowedToWithdraw[0xdD870fA1b7C4700F2BD7f44238821C26f7392148] = true;
    }

    function getShareholders() public ownerOnly view returns (address[]) {
        return _shareholders;
    }

    function getShares(address account) public ownerOnly view returns (uint256) {
        return _sharesOf[account];
    }

    function deposit() public ownerOnly payable returns(uint256) {
        require (address(this).balance + msg.value >= address(this).balance, "Balance overflow.");
        return address(this).balance;
    }

    function getBalance() public ownerOnly view returns(uint256) {
        return address(this).balance;
    }

    function setPricePerShare(uint256 value) public ownerOnly {
        _pricePerShare = value;
    }

    function setDividend(uint256 value) public ownerOnly {
        _dividend = value;
    }

    function allowToParticipate(address addr) public ownerOnly {
        _allowedToParticipate[addr] = true;
    }

    function allowToWithdraw(address addr) public ownerOnly {
        _allowedToWithdraw[addr] = true;
    }

    function buyShares() public payable {
        require(msg.sender != _owner, "Contract owner cannot buy shares.");
        require(_allowedToParticipate[msg.sender], "Account is not allowed to buy shares.");
        require(_sharesOf[_owner] * _pricePerShare >= msg.value, "Amount to buy is greater than what is currently available.");
        require(_sharesOf[msg.sender] + msg.value / _pricePerShare >= _sharesOf[msg.sender], "Shares balance overflow.");

        uint256 shares = msg.value / _pricePerShare;
        _sharesOf[_owner] -= shares;
        _sharesOf[msg.sender] += shares;
        _shareholders.push(msg.sender);
        emit SharePurchase(msg.sender, shares, _pricePerShare);
    }

    function getShares() public view returns (uint256) {
        return _sharesOf[msg.sender];
    }

    function withdraw() public returns (uint256) {
        require(msg.sender != _owner, "Contract owner cannot withdraw funds.");
        require(_allowedToWithdraw[msg.sender], "Account is not allowed to withdraw their funds.");
        require(_sharesOf[msg.sender] > 0, "Nothing to withdraw.");
        require(shareholderExists(msg.sender), "Shareholder not found.");
        
        uint256 shares = _sharesOf[msg.sender];
        uint256 amountToWithdraw = shares * _pricePerShare - _dividend;
        
        assert(amountToWithdraw <= address(this).balance);
        
        msg.sender.transfer(amountToWithdraw);
        _sharesOf[msg.sender] = 0;
        _sharesOf[_owner] += shares;
        removeShareholder(msg.sender);
        emit Withdrawal(msg.sender, shares, amountToWithdraw, _pricePerShare, _dividend);
    }

    function shareholderExists(address addr) private view returns(bool){
        for (uint256 i = 0; i < _shareholders.length; i++) {
            if (_shareholders[i] == addr) {
                return true;
            }
        }
        return false;
    }

    function findShareholder(address addr) private view returns(uint256) {
        uint256 i = 0;
        while (_shareholders[i] != addr) {
            i++;
        }
        return i;
    }

    function removeShareholderByIndex(uint256 index) private {
        uint256 i = index;
        while (i < _shareholders.length - 1) {
            _shareholders[i] = _shareholders[i + 1];
            i++;
        }
        delete _shareholders[_shareholders.length - 1];
        _shareholders.length--;
    }

    function removeShareholder(address addr) private {
        uint256 index = findShareholder(addr);
        removeShareholderByIndex(index);
    }
}