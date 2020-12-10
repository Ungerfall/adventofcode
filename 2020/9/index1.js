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
                return ({ first: arr[i], second: arr[j] });
        }
    }

    return null;
}

const sumArr = arr => {
    return +arr.reduce((acc, v) => {
        acc += v;
        return acc;
    }, 0);
};

const minMaxArr = arr => {
    let min = arr[0];
    let max = arr[0];
    for (let i = 1; i < arr.length; i++) {
        min = Math.min(min, arr[i]);
        max = Math.max(max, arr[i]);
    }

    return { min, max };
};

const preamble = 25;
let invalid = null;
for (let i = preamble; i < numbers.length; i++) {
    const pair = findPair(numbers.slice(i - preamble, i), numbers[i])
    if (pair == null) {
        console.log(numbers[i])
        invalid = numbers[i];
        break;
    }
}

for (let i = 0; i < numbers.length - 1; i++) {
    for (let j = i + 1; j < numbers.length; j++) {
        const sub = numbers.slice(i, j + 1)
        const sum = sumArr(sub);
        if (sum == invalid) {
            const minMax = minMaxArr(sub);
            console.log(minMax.min + minMax.max);
            break;
        }
    }
}