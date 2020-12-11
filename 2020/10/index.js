var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const adapters = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => s.trim().length > 0)
    .map(s => ({ jolts: +s, seen: false }))
    .sort((a, b) => b.jolts - a.jolts);

let j1 = 0;
let j3 = 1;
let adapter = adapters.pop();
if (adapter.jolts == 1)
    j1++;
else if (adapter.jolts == 3)
    j3++;

while (adapters.length > 0) {
    let adapterPrev = adapter;
    adapter = adapters.pop();
    if (adapter.jolts - adapterPrev.jolts == 3)
        j3++;
    else if (adapter.jolts - adapterPrev.jolts == 1)
        j1++;
}

console.log(j1 * j3, j1, j3);