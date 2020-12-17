var fs = require('fs');
var path = require('path');
var { lcm } = require('mathjs');

const inputFile = path.join(__dirname, "input.txt");
let notes = fs.readFileSync(inputFile).toString()
/*
`939
7,13,x,x,59,x,31,19`
*/
    .split("\n");
let buses = notes[1]
    .split(",")
    .filter(x => x != "x")
    .map(x => +x);

console.log(lcm(67,6,57,58))