var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const adapters = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => s.trim().length > 0)
    .map(s => +s);

adapters.push(0);
adapters.sort((a, b) => a - b);
adapters.push(adapters[adapters.length - 1] + 3);

const memo = [];
const traverse = (adapter, i) => {
    if (adapter == adapters[adapters.length - 1])
        return 1;

    if (memo[adapter] != null)
        return memo[adapter];

    let s = 0;
    for (let j = i + 1; j < adapters.length; j++) {
        if (adapters[j] - adapter <= 3) {
            s += traverse(adapters[j], j);
        }
    }

    memo[adapter] = s;
    return s;
}

console.log(traverse(0, 0));