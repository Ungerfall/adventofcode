var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const re = /(\w)(\d+)/;
let instructions = fs.readFileSync(inputFile).toString()
/*
`F10
N3
F7
R90
F11`
*/
    .split("\n")
    .filter(s => s.trim().length > 0)
    .map(x => {
        const r = x.match(re);
        return { action: r[1], v: +r[2] };
    })

const turn = (lORr, degrees, wp) => {
    // https://calcworkshop.com/transformations/rotation-rules/
    const rotations = {
        0: arr => arr,
        90: arr => [arr[1], -1 * arr[0]],
        180: arr => [-1 * arr[0], -1 * arr[1]],
        270: arr => [-1 * arr[1], arr[0]]
    }
    let change = degrees % 360;
    if (lORr == "L")
        change = (360 - change) % 360;

    const rot = rotations[change];

    return rot(wp);
};

const move = (coord, dir, v) => {
    [a, b] = coord;
    if (dir == "E")
        return [a + v, b];
    else if (dir == "N")
        return [a, b + v];
    else if (dir == "W")
        return [a - v, b];
    else
        return [a, b - v];
}

let coord = [0, 0]
let wp = [10, 1];

for (let i of instructions) {
    if (i.action == "F") {
        coord[0] += wp[0] * i.v;
        coord[1] += wp[1] * i.v;
    }
    else if (i.action == "L" || i.action == "R")
        wp = turn(i.action, i.v, wp)
    else
        wp = move(wp, i.action, i.v)

    console.log(coord, wp);
}

console.log(Math.abs(coord[0]) + Math.abs(coord[1]), coord);