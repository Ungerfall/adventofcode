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

const turn = (lORr, degrees) => {
    const dic = {
        "E": 0,
        "N": 90,
        "W": 180,
        "S": 270,
        0: "E",
        90: "N",
        180: "W",
        270: "S"
    };
    const lORrMul = lORr == "R" ? -1 : 1;
    let change = (lORrMul * degrees + dic[dir]) % 360;
    if (change < 0)
        change = 360 + change;

    dir = dic[change];
};
const move = (coord, dir, v) => {
    if (dir == "E")
        return { e: coord.e + v, n: coord.n }
    else if (dir == "N")
        return { e: coord.e, n: coord.n + v }
    else if (dir == "W")
        return { e: coord.e - v, n: coord.n }
    else
        return { e: coord.e, n: coord.n - v }
}

let dir = "E";
let coord = { e: 0, n: 0 };

for (let i of instructions) {
    if (i.action == "F")
        coord = move(coord, dir, i.v);
    else if (i.action == "L" || i.action == "R")
        turn(i.action, i.v)
    else
        coord = move(coord, i.action, i.v)
}

console.log(Math.abs(coord.e) + Math.abs(coord.n), coord);