var fs = require('fs');
var path = require('path');
var utils = require('../utils.js');

const inputFile = path.join(__dirname, "input.txt");
let doc = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(x => x.trim().length > 0);

const headerRe = /\w+: (\d+)-(\d+) or (\d+)-(\d+)/;
const valid = [];
for (let i = 0; i < doc.length; i++) {
    let line = doc[i];
    let r = line.match(headerRe);
    if (r == null)
        break;

    const set = new Set();
    for (let i = +r[1]; i <= +r[2]; i++) {
        set.add(i);
    }

    for (let i = +r[3]; i <= +r[4]; i++) {
        set.add(i);
    }

    valid[i] = set;
}

const myix = doc.findIndex(x => x.startsWith("your ticket:"));
const my = doc[myix+1].split(",");

const nearby = doc.findIndex(x => x == "nearby tickets:");
for (let i = nearby+1; i < doc.length; i++) {
    line = doc[i];
    const tickets = line.split(",").map(parseInt);
    for (j in valid) {
        if (valid[j].has())
    }
}

//console.log(my);
//console.log(valid);