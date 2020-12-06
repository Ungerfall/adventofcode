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

const seats = (new Array(128 * 8)).fill().map((_, i) => ({ id: i, taken: false }));

let minId = 128 * 8;
let maxId = 0;
for(i in passes) {
    const seat = passes[i];
    const row = findRow(seat.slice(0, 7), 0, 127);
    const col = findCol(seat.slice(7), 0, 7);

    const id = row * 8 + col;
    seats[id] = { ...seats[id], taken: true };
    minId = Math.min(minId, id);
    maxId = Math.max(maxId, id);
}

const mySeatId = seats.slice(minId, maxId + 1).filter(x => !x.taken);
console.log(mySeatId);