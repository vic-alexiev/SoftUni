pragma solidity ^0.4.23;

// import "github.com/OpenZeppelin/openzeppelin-solidity/contracts/ownership/Ownable.sol";
// import "github.com/OpenZeppelin/openzeppelin-solidity/contracts/token/ERC721/ERC721Token.sol";

import "./node_modules/openzeppelin-solidity/contracts/ownership/Ownable.sol";
import "./node_modules/openzeppelin-solidity/contracts/token/ERC721/ERC721Token.sol";

contract GiftLottery is Ownable, ERC721Token {
    struct Gift{
        string title;
        string description;
        string url;
    }

    event CreateGift(
        address indexed participant,
        uint256 giftId
    );

    event ReceiveGift(
        address indexed winner,
        uint256 giftId
    );

    address[] private _participants;
    uint256 private _endTime;
    mapping(uint256 => Gift) private _gifts;

    constructor(uint256 durationInMinutes) ERC721Token("GiftLottery", "GLT") public {
        _endTime = now + (durationInMinutes * 1 minutes);
    }

    function createGift(string title, string description, string url) external {
        require(now <= _endTime, "Game has already ended.");
        uint256 index = allTokens.length.add(1);
        _mint(owner, index);

        _gifts[index] = Gift(title, description, url);
        _participants.push(msg.sender);

        emit CreateGift(msg.sender, index);
    }

    function distributeGifts() public onlyOwner {
        require(now > _endTime, "Game hasn't ended yet.");
        for (uint256 giftId = 1; giftId <= allTokens.length; giftId++) {
            uint256 randomIndex = pseudoRandom(giftId);

            address winner = _participants[randomIndex];
            transferFrom(msg.sender, winner, giftId);
            emit ReceiveGift(winner, giftId);
        }
    }

    function pseudoRandom(uint256 giftId) private view returns (uint256 randomNumber) {
        Gift memory gift = _gifts[giftId];
        bytes32 hash = keccak256(abi.encodePacked(gift.title, gift.description, gift.url, giftId));
        return uint256(hash) % _participants.length;
    }
}