var fs = require('fs');
var path = require('path');

const inputFile = path.join(__dirname, "input.txt");
const numbers = fs.readFileSync(inputFile).toString()
    .split("\n")
    .filter(s => s.trim().length > 0)
    .map(s => +s);

const findPair = (arr, candidate) => {
    for (i in arr) {
        for (j in arr) {
            if (arr[i] == arr[j])
                continue;
            if (arr[i] + arr[j] == candidate)
                return ({first: arr[i], second: arr[j]});
        }
    }

    return null;
}

const preamble = 25;
let invalid = null;
for (let i = preamble; i < numbers.length; i++) {
    const pair = findPair(numbers.slice(i-preamble, i), numbers[i])
    if (pair == null) {
        console.log(numbers[i])
        invalid = numbers[i];
        break;
    }
}
