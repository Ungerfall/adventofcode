var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const answers = fs.readFileSync(inputFile).toString()
    .split("\n\n")
    .filter(s => s.trim().length > 0);

const intersect = (a, b) => {
    let intersection = new Set();
    for (const elem of b) {
        if (a.has(elem)) {
            intersection.add(elem);
        }
    }

    return intersection;
}

let sum = 0;
for (i in answers) {
    let set = new Set();
    const people = answers[i].split("\n").filter(s => s.trim().length > 0);
    for (const c of people[0]) {
        set.add(c);
    }

    for (let k = 1; k < people.length; k++) {
        set = intersect(set, people[k].split(""));
    }

    sum += set.size;
}


console.log(sum);