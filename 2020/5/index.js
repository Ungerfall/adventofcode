var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const passes = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => s.trim().length > 0);

const findRow = (seat, min, max) => {
    if (seat.length == 0)
        return max;

    let c = seat[0];
    let mid = (max - min + 1) / 2;
    if (c === "F") {
        max -= mid;
    } else {
        min += mid;
    }

    return findRow(seat.slice(1), min, max);
};

const findCol = (seat, min, max) => {
    if (seat.length == 0)
        return max;

    let c = seat[0];
    let mid = (max - min + 1) / 2;
    if (c === "L") {
        max -= mid;
    } else {
        min += mid;
    }

    return findCol(seat.slice(1), min, max);
};

const seats = new Array(128);
for(i in seats) {
    seats[i] = new Array(8);
}

let maxId = 0;
for(i in passes) {
    const seat = passes[i];
    const row = findRow(seat.slice(0, 7), 0, 127);
    const col = findCol(seat.slice(7), 0, 7);
    maxId = Math.max(maxId, row * 8 + col)
}

console.log(maxId);