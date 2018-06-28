pragma solidity ^0.4.23;

contract StudentInfoTracker {
    event Profile(
        string indexed name,
        address addr
    );

    struct Student {
        string name;
        address addr;
        mapping(string => uint8) marks;
        uint8 numberInClass;
    }

    Student[] private _students;
    address private _teacher;
    
    modifier teacherOnly() {
        require(msg.sender == _teacher);
        _;
    }
    
    constructor() public {
        _teacher = msg.sender;
    }
    
    function createProfile(string name, address addr) public teacherOnly {
        Student memory student;
        student.name = name;
        student.addr = addr;
        _students.push(student);
        emit Profile(name, addr);
    }

    function addMark(uint8 index, string subject, uint8 value) public teacherOnly {
        require(index < _students.length);
        _students[index].marks[subject] = value;
    }

    function getMark(uint8 index, string subject) public view returns(uint8) {
        require(index < _students.length);
        return _students[index].marks[subject];
    }
    
    function get(uint8 index) public view returns (string, address) {
        Student memory student = _students[index];
        return (student.name, student.addr);
    }
}