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

let ixs = new Map();
for (i in numbers) {
    if (i != numbers.length-1)
        ixs.set(numbers[i], +i);
}

let end = 30 * 10 ** 6;
let last = numbers[numbers.length - 1];
for (let i = numbers.length - 1; i < end - 1; i++) {
    let ix = ixs.get(last);
    if (ix == null) {
        ixs.set(last, i);
        last = 0;
        continue;
    }

    ixs.set(last, i);
    last = i - ix;
    if (i % 10000 == 0)
        console.log(i);
}

console.log(last);
