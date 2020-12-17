var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let notes = fs.readFileSync(inputFile).toString()
/*
`939
7,13,x,x,59,x,31,19`
*/
    .split("\n");
let ts = +notes[0];
let buses = notes[1]
    .split(",")
    .filter(x => x != "x")
    .map(x => ({ id: +x, rem: +x - (ts % +x)}));

let min = buses[0];
for (let i = 1; i < buses.length; i++) {
    if (buses[i].rem < min.rem)
        min = buses[i];
}

console.log(min, min.rem * min.id);