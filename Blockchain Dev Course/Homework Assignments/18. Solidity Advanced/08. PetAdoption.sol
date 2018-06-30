pragma solidity ^0.4.23;

contract PetSanctuary {
    enum Gender { M, F }

    event AnimalPurchase(
        address adopter,
        string animalKind
    );

    event AnimalReturned(
        address adopter,
        string animalKind
    );
    
    struct Adoption {
        uint256 timestamp;
        uint8 adopterAge;
        Gender adopterGender;
        string animalKind;
    }
    address private _owner;
    uint256 private _timeToReturn;
    mapping(string => bool) private _isAllowedAnimal;
    mapping(string => uint256) private _availableAnimals;
    mapping(address => bool) private _hasAdopted;
    mapping(address => Adoption) private _adoptions;

    modifier ownerOnly() {
        require(msg.sender == _owner, "Only owner can call this function.");
        _;
    }

    modifier allowedAnimalsOnly(string animal) {
        require(
            _isAllowedAnimal[animal], "This animal is not allowed in the pet sanctuary.");
        _;
    }

    constructor(uint256 timeToReturnInMinutes) public {
        _owner = msg.sender;
        _timeToReturn = timeToReturnInMinutes * 1 minutes;

        _isAllowedAnimal["Fish"] = true;
        _isAllowedAnimal["Cat"] = true;
        _isAllowedAnimal["Dog"] = true;
        _isAllowedAnimal["Rabbit"] = true;
        _isAllowedAnimal["Parrot"] = true;
    }

    function add(string animalKind, uint256 pieces) public ownerOnly allowedAnimalsOnly(animalKind) {
        _availableAnimals[animalKind] += pieces;
    }

    function buy(
        uint8 adopterAge,
        Gender adopterGender,
        string animalKind) public allowedAnimalsOnly(animalKind)
    {
        require(msg.sender != _owner, "Pet sanctuary owner cannot buy animals.");
        require(_hasAdopted[msg.sender] == false, "This person has already adopted an animal.");
        require(_availableAnimals[animalKind] > 0, "This animal is currently not available.");
        if (adopterGender == Gender.M) {
            if (!areEqual(animalKind, "Dog") && 
                !areEqual(animalKind, "Fish")) {
                revert("Men are only allowed to buy either dogs or fish.");
            } 
        } else {
            if (adopterAge < 40 && areEqual(animalKind, "Cat")) {
                revert("Women under 40 are not allowed to buy cats.");
            }
        }

        Adoption memory adoption;
        adoption.timestamp = now;
        adoption.adopterAge = adopterAge;
        adoption.adopterGender = adopterGender;
        adoption.animalKind = animalKind;
        _adoptions[msg.sender] = adoption;
        _hasAdopted[msg.sender] = true;
        _availableAnimals[animalKind] -= 1;
        emit AnimalPurchase(msg.sender, animalKind);
    }

    function returnAnimal() public {
        require(msg.sender != _owner, "Pet sanctuary owner cannot return animals.");
        require(_hasAdopted[msg.sender] == true, "This person has not adopted an animal yet.");
        Adoption memory adoption = _adoptions[msg.sender];
        if (now > adoption.timestamp + _timeToReturn) {
            revert("The time allowed to return an animal has run out.");
        } else {
            _availableAnimals[adoption.animalKind] += 1;
            _hasAdopted[msg.sender] = false;
            string memory animalKind = adoption.animalKind;
            delete _adoptions[msg.sender];
            emit AnimalReturned(msg.sender, animalKind);
        }
    }

    function areEqual(string a, string b) private pure returns (bool) {
        return keccak256(abi.encodePacked(a)) == keccak256(abi.encodePacked(b));
    }
}