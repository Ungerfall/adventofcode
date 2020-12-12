var fs = require('fs');
var path = require('path');

String.prototype.replaceAt = function(index, replacement) {
    return this.substr(0, index) + replacement + this.substr(index + replacement.length);
}

const inputFile = path.join(__dirname, "input.txt");
let layout = fs.readFileSync(inputFile).toString()
/*
`L.LL.LL.LL
LLLLLLL.LL
L.L.L..L..
LLLL.LL.LL
L.LL.LL.LL
L.LLLLL.LL
..L.L.....
LLLLLLLLLL
L.LLLLLL.L
L.LLLLL.LL`
*/
    .split("\n")
    .filter(s => s.trim().length > 0)

const floor = ".";
const empty = "L";
const occupied = "#";
const w = layout[0].length;
const h = layout.length;

const lo = (x, y) => {
    if (
        x < 0
        || x >= w
        || y < 0
        || y >= h
    )
        return null;

    return layout[y][x];
};
const countAdjacent = (x, y) => {
    return (
        ((lo(x-1, y-1) == occupied) ? 1 : 0)
        + ((lo(x, y-1) == occupied) ? 1 : 0)
        + ((lo(x+1, y-1) == occupied) ? 1 : 0)
        + ((lo(x-1, y) == occupied) ? 1 : 0)
        + ((lo(x+1, y) == occupied) ? 1 : 0)
        + ((lo(x-1, y+1) == occupied) ? 1 : 0)
        + ((lo(x, y+1) == occupied) ? 1 : 0)
        + ((lo(x+1, y+1) == occupied) ? 1 : 0)
    );
};

const printAdjacent = (x, y) => {
    const section = new Array(3).fill(floor);
    section[0] = lo(x-1, y-1) + lo(x, y-1) + lo(x+1, y-1);
    section[1] = lo(x-1, y) + lo(x, y) + lo(x+1, y);
    section[2] = lo(x-1, y+1) + lo(x, y+1) + lo(x+1, y+1);
    console.log(section, {x,y, occupied: countAdjacent(x,y)});
};

const turn = (x, y) => {
    if (lo(x, y) == floor)
        return { new: floor, changed: false };

    const adj = countAdjacent(x, y);
    if (lo(x, y) == empty && adj == 0)
        return { new: occupied, changed: true };

    if (lo(x, y) == occupied && adj >= 4)
        return { new: empty, changed: true };

    return { new: lo(x, y), changed: false };
};

let changed = false;
let occupiedCount = 0;
let buffer = new Array(h).fill(floor).map((_, i) => layout[i]);
do {
    changed = false;
    occupiedCount = 0;
    for (let y = 0; y < h; y++) {
        for (let x = 0; x < w; x++) {
            const t = turn(x, y);
            if (t.changed) {
                buffer[y] = buffer[y].replaceAt(x, t.new);
                changed = true;
            }

            if (t.new == occupied)
                occupiedCount++;
        }
    }

    layout = layout.map((_, i) => buffer[i]);
} while (changed)

console.log(occupiedCount);