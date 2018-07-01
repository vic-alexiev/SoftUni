pragma solidity ^0.4.23;

contract PublicArticle {
    uint256 public expires;
    address public owner = msg.sender;
    string private articleName;
    string private articleText;

    modifier onlyOwner() {
        require(msg.sender == owner, "Only owner can call this function.");
        _;
    }

    modifier limitedTime() {
        require(now <= expires, "The article has expired.");
        _;
    }

    function setArticleName(string _articleName) public onlyOwner {
        articleName = _articleName;
    }

    function setArticleText(string _articleText) public onlyOwner {
        articleText = _articleText;
    }

    function setDuration(uint256 _duration) public onlyOwner {
        expires = now + (_duration * 1 minutes);
    }

    function getArticleName() public view limitedTime returns(string) {
        return articleName;
    }

    function getArticleText() public view limitedTime returns(string) {
        return articleText;
    }
}