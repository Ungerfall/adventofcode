var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const answers = fs.readFileSync(inputFile).toString()
    .split("\n\n")
    .filter(s => s.trim().length > 0);

let sum = 0;
for (i in answers) {
    let set = new Set();
    const people = answers[i].split("\n").filter(s => s.trim().length > 0);
    for (j in people) {
        for (const c of people[j]) {
            set.add(c);
        }
    }

    sum += set.size;
}


console.log(sum);