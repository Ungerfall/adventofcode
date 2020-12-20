var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
let numbers = fs.readFileSync(inputFile).toString()
/*
`0,3,6`
*/
    .split(",")
    .filter(x => x.trim().length > 0)
    .map(s => +s);

const spoken = [];
for (let n of numbers)
    spoken.push(n);

while (spoken.length < 2020) {
    let last = spoken[spoken.length-1];
    let ix = spoken.lastIndexOf(last, spoken.length-2);
    if (ix == -1) {
        spoken.push(0);
        continue;
    }

    spoken.push(spoken.length - (ix + 1));
}
console.log(spoken[2020-1]);
