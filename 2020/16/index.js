var fs = require('fs');
var path = require('path');
var utils = require('../utils.js');

const inputFile = path.join(__dirname, "input.txt");
let doc = fs.readFileSync(inputFile).toString()
/*
`class: 1-3 or 5-7
row: 6-11 or 33-44
seat: 13-40 or 45-50

your ticket:
7,1,14

nearby tickets:
7,3,47
40,4,50
55,2,20
38,6,12`
*/
    .split("\n")
    .filter(x => x.trim().length > 0);

const headerRe = /\w+: (\d+)-(\d+) or (\d+)-(\d+)/;
const valid = new Set();
for (let i = 0; i < doc.length; i++) {
    let line = doc[i];
    let r = line.match(headerRe);
    if (r == null)
        break;

    for (let i = +r[1]; i <= +r[2]; i++) {
        valid.add(i);
    }

    for (let i = +r[3]; i <= +r[4]; i++) {
        valid.add(i);
    }
}

const nearby = doc.findIndex(x => x == "nearby tickets:");
let err = [];
for (let i = nearby+1; i < doc.length; i++) {
    line = doc[i];
    const tickets = line.split(",").map(x => +x);
    for (let n of tickets) {
        if (!valid.has(n))
            err.push(n);
    }
}

console.log(utils.sum(err));