pragma solidity ^0.4.23;

contract HungerGame {
    enum Gender { M, F }
    enum GameResult { Dead, Alive }

    event ParticipantAdded(
        string name,
        uint8 age,
        Gender gender
    );

    event Pair(
        string boy,
        string girl
    );

    struct Participant {
        string name;
        uint8 age;
        Gender gender;
    }

    Participant[] private _boys;
    Participant[] private _girls;
    uint256 private _startTime;
    uint256 private _duration;
    uint256 private _endTime;
    uint256 private _seed;

    modifier afterGameEnds() {
        require(now > _endTime, "Game hasn't ended yet.");
        _;
    }

    constructor(uint256 durationInMinutes) public {
        _duration = durationInMinutes * 1 minutes;
    }

    function addParticipant(string name, uint8 age, Gender gender) public {
        require(age >= 12 && age <= 18, "Participants must be between 12 and 18 years old.");
        Participant memory participant;
        participant.name = name;
        participant.age = age;
        participant.gender = gender;

        if (gender == Gender.M) {
            _boys.push(participant);
        } else {
            _girls.push(participant);
        }

        emit ParticipantAdded(name, age, gender);
    }

    function getBoysCount() public view returns (uint256) {
        return _boys.length;
    }

    function getGirlsCount() public view returns (uint256) {
        return _girls.length;
    }

    function choosePair() public returns(uint256 boy, uint256 girl) {
        require (_boys.length > 0, "No boys found to choose from.");
        require (_girls.length > 0, "No girls found to choose from.");

        uint256 boyIndex = generateRandom(_boys.length);
        uint256 girlIndex = generateRandom(_girls.length);

        _startTime = now;
        _endTime = _startTime + _duration;

        emit Pair(_boys[boyIndex].name, _girls[girlIndex].name);
        return (boyIndex, girlIndex);
    }

    function checkResult() public afterGameEnds returns (GameResult) {
        uint256 result = generateRandom(2);
        if (result == 0) {
            return GameResult.Dead;
        } else {
            return GameResult.Alive;
        }
    }

    function generateRandom(uint256 limit) private returns (uint256 randomNumber) {
        return uint256(keccak256(abi.encodePacked(blockhash(block.number-1), ++_seed))) % limit;
    }
}